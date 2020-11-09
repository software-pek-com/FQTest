using System;
using MongoDB.Driver;
using Moq;
using MoWebApp.Data;
using MoWebApp.Services;
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

        private UserService CreateTarget()
        {
            return new UserService(clientMock.Object, Mapper);
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
            var target = CreateTarget();

            Assert.Throws<NotImplementedException>(() => target.GetById("abc"));
        }

        #endregion

    }
}
