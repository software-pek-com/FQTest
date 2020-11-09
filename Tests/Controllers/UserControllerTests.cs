using Microsoft.Extensions.Logging;
using MoWebApp.Controllers;
using NUnit.Framework;
using Moq;
using System;

namespace Tests.MoWebApp.Controllers
{
    public class UserControllerTests
    {
        private Mock<ILogger<UserController>> loggerMock;

        #region Helpers

        private UserController CreateTarget()
        {
            return new UserController(loggerMock.Object);
        }

        #endregion

        [SetUp]
        public void Setup()
        {
            loggerMock = new Mock<ILogger<UserController>>();
        }

        [Test]
        public void UserController_Can_Construct()
        {
            var userController = CreateTarget();

            Assert.IsNotNull(userController);
        }

        [Test]
        public void UserController_Cannot_Construct_With_Null_Logger()
        {
            Assert.Throws<ArgumentNullException>(() => new UserController(null));
        }
    }
}