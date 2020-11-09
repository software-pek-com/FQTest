using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MongoDB.Driver;
using Moq;
using MoWebApp.Data;
using MoWebApp.Services;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Tests.Services
{
    public class UserServiceTests : TestBase
    {
        private Mock<IMongoClient> clientMock;
        private Mock<IMongoDatabase> databaseMock;
        private Mock<IMongoCollection<User>> collectionMock;

        public UserServiceTests() { }

        #region Helpers

        private class UserServiceMock : UserService
        {
            public UserServiceMock(IQueryable<User> users, IMongoClient client, IMapper mapper)
                : base(client, mapper)
            {
                DataStore = users;
            }

            public IQueryable<User> DataStore { get; private set; }

            protected override IQueryable<User> GetUserQueryable()
            {
                return DataStore;
            }
        }

        private UserService CreateTarget()
        {
            return new UserService(clientMock.Object, Mapper);
        }

        private UserService CreateTarget(IEnumerable<User> dataStore)
        {
            return new UserServiceMock(dataStore.AsQueryable(), clientMock.Object, Mapper);
        }

        #endregion

        [SetUp]
        public void Setup()
        {
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

        [Test]
        public void UserService_Can_Construct()
        {
            var target = CreateTarget();

            Assert.IsNotNull(target);
        }


        [Test]
        public void UserService_Cannot_Construct_With_Null_Client_And_Null_Mapper()
        {
            Assert.Throws<ArgumentNullException>(() => new UserService(null, null));
        }

        [Test]
        public void UserService_Cannot_Construct_With_Null_Client()
        {
            Assert.Throws<ArgumentNullException>(() => new UserService(null, Mapper));
        }

        [Test]
        public void UserService_Cannot_Construct_With_Null_Mapper()
        {
            Assert.Throws<ArgumentNullException>(() => new UserService(clientMock.Object, null));
        }


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
