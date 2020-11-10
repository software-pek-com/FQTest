using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using MoWebApp;
using MoWebApp.Data;
using MoWebApp.Documents;
using MoWebApp.Services;
using NUnit.Framework;
using RabbitMQ.Client;

namespace Tests.Services
{
    public class UserServiceTests : TestBase
    {
        #region Helpers

        private class UserServiceMock : UserService
        {
            private readonly Mock<IMongoCollection<User>> collectionMock;

            /// <summary>
            /// TODO: Move to mock config to service provider and Setup and TestBase.
            /// </summary>
            public UserServiceMock(
                IConnectionFactory connectionFactory,
                IHttpContextAccessor httpContextAccessor,
                IOptions<AppSettings> options,
                IQueryable<User> users,
                Mock<IMongoCollection<User>> collectionMock,
                IMongoClient client,
                IMapper mapper)
                : base(connectionFactory, httpContextAccessor, options, client, mapper)
            {
                this.collectionMock = collectionMock;
                DataStore = users;
            }

            public UserServiceMock(
                IConnectionFactory connectionFactory,
                IHttpContextAccessor httpContextAccessor,
                IOptions<AppSettings> options,
                Mock<IMongoCollection<User>> collectionMock,
                IMongoClient client, IMapper mapper)
                : base(connectionFactory, httpContextAccessor, options, client, mapper)
            {
                this.collectionMock = collectionMock;
            }

            public IQueryable<User> DataStore { get; private set; }

            protected override IMongoCollection<User> AsCollection()
            {
                return collectionMock.Object;
            }

            protected override IQueryable<User> AsQueryable()
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
            return new UserService(
                ConnectionFactory,
                HttpContextAccessorMock.Object,
                OptionsMock.Object,
                ClientMock.Object,
                Mapper);
        }

        private UserServiceMock CreateTarget(IEnumerable<User> dataStore)
        {
            return new UserServiceMock(
                ConnectionFactory,
                HttpContextAccessorMock.Object,
                OptionsMock.Object,
                dataStore.AsQueryable(),
                CollectionMock,
                ClientMock.Object,
                Mapper);
        }

        private UserServiceMock CreateTarget(Mock<IMongoCollection<User>> collectionMock)
        {
            return new UserServiceMock(
                ConnectionFactory,
                HttpContextAccessorMock.Object,
                OptionsMock.Object,
                collectionMock,
                ClientMock.Object,
                Mapper);
        }

        #endregion

        [SetUp]
        public void Setup()
        {
            CreateServiceProvider();
        }

        #region Constructor

        [Test]
        public void UserService_Can_Construct()
        {
            var target = CreateTarget();

            Assert.IsNotNull(target);
        }

        [Test]
        public void UserService_Cannot_Construct_With_Null_ConnectionFactory()
        {
            Assert.Throws<ArgumentNullException>(() => new UserService(null, HttpContextAccessorMock.Object, OptionsMock.Object, ClientMock.Object, Mapper));
        }

        [Test]
        public void UserService_Cannot_Construct_With_Null_HttpContextAccessor()
        {
            Assert.Throws<ArgumentNullException>(() => new UserService(ConnectionFactory, null, OptionsMock.Object, ClientMock.Object, Mapper));
        }

        [Test]
        public void UserService_Cannot_Construct_With_Null_Options()
        {
            Assert.Throws<ArgumentNullException>(() => new UserService(ConnectionFactory, HttpContextAccessorMock.Object, null, ClientMock.Object, Mapper));
        }

        [Test]
        public void UserService_Cannot_Construct_With_Null_Client()
        {
            Assert.Throws<ArgumentNullException>(() => new UserService(ConnectionFactory, HttpContextAccessorMock.Object, OptionsMock.Object, null, Mapper));
        }

        [Test]
        public void UserService_Cannot_Construct_With_Null_Mapper()
        {
            Assert.Throws<ArgumentNullException>(() => new UserService(ConnectionFactory, HttpContextAccessorMock.Object, OptionsMock.Object, ClientMock.Object, null));
        }

        #endregion

        #region IUserService

        [Test]
        public void UserService_GetById()
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

        [Test]
        public void UserService_GetById_Throws_When_Id_Null()
        {
            var dataStore = new List<User>
            {
                new User { Id = "filip", FirstName = "Filip", LastName = "Fodemski" }
            };
            var target = CreateTarget(dataStore);

            Assert.Throws<ArgumentNullException>(() => target.GetById(null));
        }

        [Test]
        public void UserService_GetById_Throws_When_Id_Empty()
        {
            var dataStore = new List<User>
            {
                new User { Id = "filip", FirstName = "Filip", LastName = "Fodemski" }
            };
            var target = CreateTarget(dataStore);

            Assert.Throws<ArgumentException>(() => target.GetById(""));
        }

        [Test]
        public void UserService_GetById_Returns_Null_When_Not_Found()
        {
            var dataStore = new List<User>();
            var expectedId = "filip";
            var target = CreateTarget(dataStore);

            Assert.Null(target.GetById(expectedId));
        }

        [Test]
        public void UserService_Delete()
        {
            var expectedId = "filip";
            
            CollectionMock
                .Setup(s => s.DeleteOne(It.IsAny<FilterDefinition<User>>(), CancellationToken.None))
                .Returns(new DeleteResult.Acknowledged(1));

            var target = CreateTarget(CollectionMock);

            target.Delete(expectedId);

            CollectionMock.Verify(s => s.DeleteOne(It.IsAny<FilterDefinition<User>>(), CancellationToken.None), Times.Once);
        }

        [Test]
        public void UserService_Delete_Throws_When_Id_Null()
        {
            CollectionMock
                .Setup(s => s.DeleteOne(It.IsAny<FilterDefinition<User>>(), CancellationToken.None))
                .Returns(new DeleteResult.Acknowledged(1));

            var target = CreateTarget(CollectionMock);

            Assert.Throws<ArgumentNullException>(() => target.Delete(null));
        }

        [Test]
        public void UserService_Delete_Throws_When_Id_Empty()
        {
            CollectionMock
                .Setup(s => s.DeleteOne(It.IsAny<FilterDefinition<User>>(), CancellationToken.None))
                .Returns(new DeleteResult.Acknowledged(1));

            var target = CreateTarget(CollectionMock);

            Assert.Throws<ArgumentException>(() => target.Delete(""));
        }

        [Test]
        public void UserService_Delete_Is_Silent_When_Id_Not_Found()
        {
            var expectedId = "filip";

            CollectionMock
                .Setup(s => s.DeleteOne(It.IsAny<FilterDefinition<User>>(), CancellationToken.None))
                .Returns(new DeleteResult.Acknowledged(0));

            var target = CreateTarget(CollectionMock);

            Assert.False(target.Delete(expectedId));

            CollectionMock.Verify(s => s.DeleteOne(It.IsAny<FilterDefinition<User>>(), CancellationToken.None), Times.Once);
        }

        [Test]
        public void UserService_Find_Throws_When_Filter_Null()
        {
            var dataStore = new List<User>
            {
                new User { Id = "filip", FirstName = "Filip", LastName = "Fodemski" }
            };
            var target = CreateTarget(dataStore);

            Assert.Throws<ArgumentNullException>(() => target.Find(null, new UserSearchOrderBy()));
        }

        [Test]
        public void UserService_Find_Throws_When_OrderBy_Null()
        {
            var dataStore = new List<User>
            {
                new User { Id = "filip", FirstName = "Filip", LastName = "Fodemski" }
            };
            var target = CreateTarget(dataStore);

            Assert.Throws<ArgumentNullException>(() => target.Find(new UserSearchFilter(), null));
        }

        /* Missing unit tests for the Create, Find and Update methods should be added here. */

        #endregion

    }
}
