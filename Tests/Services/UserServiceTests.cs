using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using MoWebApp;
using MoWebApp.Data;
using MoWebApp.Services;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Tests.Services
{
    public class UserServiceTests : TestBase
    {
        private Mock<IOptions<AppSettings>> optionsMock;
        private Mock<IMongoClient> clientMock;
        private Mock<IMongoDatabase> databaseMock;
        private Mock<IMongoCollection<User>> collectionMock;

        public UserServiceTests() { }

        #region Helpers

        private class UserServiceMock : UserService
        {
            public UserServiceMock(IOptions<AppSettings> options, IQueryable<User> users, IMongoClient client, IMapper mapper)
                : base(options, client, mapper)
            {
                DataStore = users;
            }

            public IQueryable<User> DataStore { get; private set; }

            protected override IQueryable<User> GetUserQueryable()
            {
                return DataStore;
            }
        }

        private AppSettings CreateAppSettings()
        {
            return new AppSettings { DbUrl = "UnitTests", DbName = "MoWebAppDB" };
        }

        private UserService CreateTarget()
        {
            return new UserService(optionsMock.Object, clientMock.Object, Mapper);
        }

        private UserService CreateTarget(IEnumerable<User> dataStore)
        {
            return new UserServiceMock(optionsMock.Object, dataStore.AsQueryable(), clientMock.Object, Mapper);
        }

        #endregion

        [SetUp]
        public void Setup()
        {
            optionsMock = new Mock<IOptions<AppSettings>>();
            optionsMock.SetupGet(s => s.Value).Returns(CreateAppSettings());

            clientMock = new Mock<IMongoClient>();
            databaseMock = new Mock<IMongoDatabase>();
            collectionMock = new Mock<IMongoCollection<User>>();

            clientMock
                .Setup(s => s.GetDatabase(It.IsAny<string>(), null))
                .Returns(databaseMock.Object);

            databaseMock
                .Setup(s => s.GetCollection<User>(nameof(User), null))
                .Returns(collectionMock.Object);
        }

        #region Constructor

        [Test]
        public void UserService_Can_Construct()
        {
            var target = CreateTarget();

            Assert.IsNotNull(target);
        }

        [Test]
        public void UserService_Cannot_Construct_With_Null_Options()
        {
            Assert.Throws<ArgumentNullException>(() => new UserService(null, clientMock.Object, Mapper));
        }

        [Test]
        public void UserService_Cannot_Construct_With_Null_Client()
        {
            Assert.Throws<ArgumentNullException>(() => new UserService(optionsMock.Object, null, Mapper));
        }

        [Test]
        public void UserService_Cannot_Construct_With_Null_Mapper()
        {
            Assert.Throws<ArgumentNullException>(() => new UserService(optionsMock.Object, clientMock.Object, null));
        }

        #endregion

        #region IUserService

        [Test]
        public void UserService_Can_GetById()
        {
            var dataStore = new List<User>
            {
                new User { Id = "filip", FirstName = "Filip", LastName = "Fodemski" }
            };
            var expectedId = "filip";
            var target = CreateTarget(dataStore);

            var result = target.GetById(expectedId);

            Assert.NotNull(result);
            Assert.AreEqual(expectedId, result.Id);
        }

        #endregion

    }
}
