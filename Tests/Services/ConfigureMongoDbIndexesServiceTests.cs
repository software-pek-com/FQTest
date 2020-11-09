using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using MoWebApp.Data;
using MoWebApp.Services;
using NUnit.Framework;

namespace Tests.Services
{
    public class ConfigureMongoDbIndexesServiceTests
    {
        private Mock<IMongoClient> clientMock;
        private Mock<ILogger<ConfigureMongoDbIndexesService>> loggerMock;
        private Mock<IMongoDatabase> databaseMock = new Mock<IMongoDatabase>();
        private Mock<IMongoCollection<User>> collectionMock = new Mock<IMongoCollection<User>>();
        private Mock<IMongoIndexManager<User>> indexManagerMock = new Mock<IMongoIndexManager<User>>();

        #region Helpers

        private ConfigureMongoDbIndexesService CreateTarget()
        {
            return new ConfigureMongoDbIndexesService(clientMock.Object, loggerMock.Object);
        }

        private ServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddSingleton(loggerMock.Object);
            services.AddSingleton(clientMock.Object);
            services.AddHostedService<ConfigureMongoDbIndexesService>();
            return services.BuildServiceProvider();
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
            clientMock = new Mock<IMongoClient>();
            loggerMock = new Mock<ILogger<ConfigureMongoDbIndexesService>>();
            
            databaseMock = new Mock<IMongoDatabase>();
            collectionMock = new Mock<IMongoCollection<User>>();
            indexManagerMock = new Mock<IMongoIndexManager<User>>();

            clientMock
                .Setup(s => s.GetDatabase(It.IsAny<string>(), null))
                .Returns(databaseMock.Object);

            databaseMock
                .Setup(s => s.GetCollection<User>(nameof(User), null))
                .Returns(collectionMock.Object);

            collectionMock
                .SetupGet(s => s.Indexes)
                .Returns(indexManagerMock.Object);

            indexManagerMock
                .Setup(s => s.CreateOneAsync(It.IsAny<CreateIndexModel<User>>(), null, CancellationToken.None))
                .Returns(Task.FromResult("OK"));
        }

        [Test]
        public void ConfigureMongoDbIndexesService_Can_Construct()
        {
            var target = CreateTarget();

            Assert.IsNotNull(target);
        }

        [Test]
        public void ConfigureMongoDbIndexesService_Cannot_Construct_With_Null_MongoClient()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ConfigureMongoDbIndexesService(null, loggerMock.Object));
        }

        [Test]
        public void ConfigureMongoDbIndexesService_Cannot_Construct_With_Null_Logger()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ConfigureMongoDbIndexesService(clientMock.Object, null));
        }

        [Test]
        public async Task ConfigureMongoDbIndexesService_Executes_Task()
        {
            var serviceProvider = CreateServiceProvider();
            var target = serviceProvider.GetService<IHostedService>() as ConfigureMongoDbIndexesService;
            var task = target.StartAsync(CancellationToken.None);
            await task;

            await Task.Delay(1000);

            Assert.IsTrue(task.IsCompleted);

            await target.StopAsync(CancellationToken.None);

            VerifyInformationWasLogged(loggerMock);

            clientMock.Verify(s => s.GetDatabase(It.IsAny<string>(), null), Times.Once);
            databaseMock.Verify(
                s => s.GetCollection<User>(nameof(User), null), Times.Once);
            collectionMock.VerifyGet(s => s.Indexes, Times.Once);
            indexManagerMock.Verify(
                s => s.CreateOneAsync(It.IsAny<CreateIndexModel<User>>(), null, CancellationToken.None),
                Times.Once);
        }
    }
}
