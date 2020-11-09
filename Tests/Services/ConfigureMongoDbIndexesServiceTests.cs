using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MongoDB.Driver;
using Moq;
using MoWebApp.Models;
using MoWebApp.Services;
using NUnit.Framework;

namespace Tests.Services
{
    public class ConfigureMongoDbIndexesServiceTests
    {
        private Mock<IMongoClient> clientMock;
        private Mock<ILogger<ConfigureMongoDbIndexesService>> loggerMock;

        #region Helpers

        private ConfigureMongoDbIndexesService CreateTarget()
        {
            return new ConfigureMongoDbIndexesService(clientMock.Object, loggerMock.Object);
        }

        #endregion

        [SetUp]
        public void Setup()
        {
            clientMock = new Mock<IMongoClient>();
            loggerMock = new Mock<ILogger<ConfigureMongoDbIndexesService>>();
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
            Mock<IMongoDatabase> databaseMock = new Mock<IMongoDatabase>();
            Mock<IMongoCollection<UserDocument>> collectionMock = new Mock<IMongoCollection<UserDocument>>();
            Mock<IMongoIndexManager<UserDocument>> indexManagerMock = new Mock<IMongoIndexManager<UserDocument>>();

            clientMock
                .Setup(s => s.GetDatabase(It.IsAny<string>(), null))
                .Returns(databaseMock.Object);

            databaseMock
                .Setup(s => s.GetCollection<UserDocument>(nameof(UserDocument), null))
                .Returns(collectionMock.Object);

            collectionMock
                .SetupGet(s => s.Indexes)
                .Returns(indexManagerMock.Object);

            indexManagerMock
                .Setup(s => s.CreateOneAsync(It.IsAny<CreateIndexModel<UserDocument>>(), null, CancellationToken.None))
                .Returns(Task.FromResult("OK"));

            var services = new ServiceCollection();
            services.AddSingleton(loggerMock.Object);
            services.AddSingleton(clientMock.Object);
            services.AddHostedService<ConfigureMongoDbIndexesService>();
            var serviceProvider = services.BuildServiceProvider();

            var service = serviceProvider.GetService<IHostedService>() as ConfigureMongoDbIndexesService;

            var task = service.StartAsync(CancellationToken.None);
            await task;

            await Task.Delay(1000);
            Assert.IsTrue(task.IsCompleted);

            await service.StopAsync(CancellationToken.None);

            clientMock.Verify(s => s.GetDatabase(It.IsAny<string>(), null), Times.Once);
            databaseMock.Verify(s => s.GetCollection<UserDocument>(nameof(UserDocument), null), Times.Once);
            collectionMock.VerifyGet(s => s.Indexes, Times.Once);
            indexManagerMock.Verify(s => s.CreateOneAsync(It.IsAny<CreateIndexModel<UserDocument>>(), null, CancellationToken.None), Times.Once);
        }
    }
}
