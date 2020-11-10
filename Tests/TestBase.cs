using System;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using MoWebApp;
using MoWebApp.Data;
using RabbitMQ.Client;

namespace Tests
{
    public class TestBase
    {
        public IConnectionFactory ConnectionFactory { get; private set; }
        public Mock<IHttpContextAccessor> HttpContextAccessorMock { get; private set; }
        public Mock<IOptions<AppSettings>> OptionsMock { get; private set; }
        public Mock<IMongoClient> ClientMock { get; protected set; }
        public Mock<IMongoDatabase> DatabaseMock { get; private set; }
        public Mock<IMongoCollection<User>> CollectionMock { get; private set; }

        public TestBase() { }

        public void CreateServiceProvider()
        {
            var services = new ServiceCollection();

            ConfigureMocksInternal(services);
            ConfigureMocks(services);

            Mapper = Startup.ConfigureMapper();
            services.AddSingleton(Mapper);

            ConfigureServices(services);

            ServiceProvider = services.BuildServiceProvider();
        }

        protected virtual void ConfigureServices(ServiceCollection services) { }
        protected virtual void ConfigureMocks(ServiceCollection services) { }

        private void ConfigureMocksInternal(ServiceCollection services)
        {
            services.AddOptions();

            var appSettings = CreateAppSettings();
            OptionsMock = new Mock<IOptions<AppSettings>>();
            OptionsMock.SetupGet(s => s.Value).Returns(appSettings);
            services.AddSingleton(OptionsMock.Object);

            HttpContextAccessorMock = new Mock<IHttpContextAccessor>();
            services.AddSingleton(HttpContextAccessorMock.Object);

            ConnectionFactory = new ConnectionFactory { Uri = new Uri(appSettings.EventBusUrl) };
            services.AddSingleton(ConnectionFactory);

            ClientMock = new Mock<IMongoClient>();
            services.AddSingleton(ClientMock.Object);

            DatabaseMock = new Mock<IMongoDatabase>();
            CollectionMock = new Mock<IMongoCollection<User>>();

            ClientMock
                .Setup(s => s.GetDatabase(It.IsAny<string>(), null))
                .Returns(DatabaseMock.Object);

            DatabaseMock
                .Setup(s => s.GetCollection<User>(nameof(User), null))
                .Returns(CollectionMock.Object);
        }

        private AppSettings CreateAppSettings()
        {
            return new AppSettings
            {
                DbUrl = "UnitTests",
                DbName = "MoWebAppDB",
                EventBusQueue = "NewUsers",
                EventBusUrl = "amqp://localhost:1234",
                LoginUrl = "http://localhost/login",
                WelcomeMessageFolder = "test" };
        }

        public ServiceProvider ServiceProvider { get; private set; }

        public IMapper Mapper { get; private set; }
    }
}
