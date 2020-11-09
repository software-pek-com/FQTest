using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MoWebApp.Core;
using MoWebApp.Data;
using MoWebApp.Services;

namespace MoWebApp.Controllers
{
    /// <summary>
    /// Represents a WebApi controller for performing database operations on <see cref="User">users</see>.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService service;

        /// <summary>
        /// Creates an instance of this class.
        /// </summary>
        public UserController(IUserService service)
        {
            Guard.ArgumentNotNull(service, nameof(service));

            this.service = service;
        }

        /// <summary>
        /// Returns all known <see cref="IEnumerable<UserDocument>">users</see>.
        /// </summary>
        [HttpGet]
        public IEnumerable<User> Get() => service.GetAll();
    }
}
