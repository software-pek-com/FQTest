using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using MoWebApp;
using MoWebApp.Data;
using MoWebApp.Services;
using NUnit.Framework;

namespace Tests.Services
{
    public class ConfigureMongoDbIndexesServiceTests : TestBase
    {
        public Mock<ILogger<ConfigureMongoDbIndexesService>> LoggerMock { get; private set; }
        public Mock<IMongoIndexManager<User>> IndexManagerMock { get; private set; }
        public Mock<IMongoClient> FailingClientMock { get; private set; }

        protected override void ConfigureMocks(ServiceCollection services)
        {
            // Override [Setup] above.
            FailingClientMock = new Mock<IMongoClient>();
            FailingClientMock
                .Setup(s => s.GetDatabase(It.IsAny<string>(), null))
                .Throws(new MongoClientException("Something bad happened"));

            LoggerMock = new Mock<ILogger<ConfigureMongoDbIndexesService>>();
            IndexManagerMock = new Mock<IMongoIndexManager<User>>();

            CollectionMock
                .SetupGet(s => s.Indexes)
                .Returns(IndexManagerMock.Object);

            IndexManagerMock
                .Setup(s => s.CreateOneAsync(It.IsAny<CreateIndexModel<User>>(), null, CancellationToken.None))
                .Returns(Task.FromResult("OK"));
        }

        protected override void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<ILogger<ConfigureMongoDbIndexesService>>(LoggerMock.Object);
            services.AddSingleton<IMongoIndexManager<User>>(IndexManagerMock.Object);

            services.AddHostedService<ConfigureMongoDbIndexesService>();
        }

        #region Helpers

        private AppSettings CreateAppSettings()
        {
            return new AppSettings { DbUrl = "UnitTests", DbName = "MoWebAppDB" };
        }

        private ConfigureMongoDbIndexesService CreateTarget()
        {
            var target = ServiceProvider.GetService<IHostedService>() as ConfigureMongoDbIndexesService;
            return target;
        }

        private static Mock<ILogger<T>> VerifyInformationWasLogged<T>(Mock<ILogger<T>> logger)
        {
            logger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));

            return logger;
        }

        #endregion

        [SetUp]
        public void Setup()
        {
            CreateServiceProvider();
        }

        #region Constructor

        [Test]
        public void ConfigureMongoDbIndexesService_Can_Construct()
        {
            var target = new ConfigureMongoDbIndexesService(OptionsMock.Object, ClientMock.Object, LoggerMock.Object);

            Assert.IsNotNull(target);
        }

        [Test]
        public void ConfigureMongoDbIndexesService_Cannot_Construct_With_Null_Options()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ConfigureMongoDbIndexesService(null, ClientMock.Object, LoggerMock.Object));
        }

        [Test]
        public void ConfigureMongoDbIndexesService_Cannot_Construct_With_Null_Client()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ConfigureMongoDbIndexesService(OptionsMock.Object, null, LoggerMock.Object));
        }

        [Test]
        public void ConfigureMongoDbIndexesService_Cannot_Construct_With_Null_Logger()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ConfigureMongoDbIndexesService(OptionsMock.Object, ClientMock.Object, null));
        }

        #endregion

        [Test]
        public async Task ConfigureMongoDbIndexesService_Executes_Task()
        {
            var target = CreateTarget();
            var task = target.StartAsync(CancellationToken.None);
            await task;

            await Task.Delay(1000);

            Assert.IsTrue(task.IsCompleted);

            await target.StopAsync(CancellationToken.None);

            VerifyInformationWasLogged(LoggerMock);

            ClientMock.Verify(s => s.GetDatabase(It.IsAny<string>(), null), Times.Once);
            DatabaseMock.Verify(
                s => s.GetCollection<User>(nameof(User), null), Times.Once);
            CollectionMock.VerifyGet(s => s.Indexes, Times.Once);
            IndexManagerMock.Verify(
                s => s.CreateOneAsync(It.IsAny<CreateIndexModel<User>>(), null, CancellationToken.None),
                Times.Once);
        }
    }
}
