using System;
using Moq;
using MoWebApp.Controllers;
using MoWebApp.Services;
using NUnit.Framework;

namespace Tests.MoWebApp.Controllers
{
    public class UserControllerTests
    {
        private Mock<IUserService> userServiceMock;

        #region Helpers

        private UserController CreateTarget()
        {
            return new UserController(userServiceMock.Object);
        }

        #endregion

        [SetUp]
        public void Setup()
        {
            userServiceMock = new Mock<IUserService>();
        }

        [Test]
        public void UserController_Can_Construct()
        {
            var target = CreateTarget();

            Assert.IsNotNull(target);
        }

        [Test]
        public void UserController_Cannot_Construct_With_Null_Logger()
        {
            Assert.Throws<ArgumentNullException>(() => new UserController(null));
        }
    }
}