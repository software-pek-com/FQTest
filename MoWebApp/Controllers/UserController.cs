using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MoWebApp.Core;
using MoWebApp.Data;
using MoWebApp.Documents;
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

        [HttpPut]
        [Route("find")]
        public IEnumerable<UserSummary> Find(
            [FromBody] UserSearchFilter filter,
            [FromQuery(Name = "orderByLastName")] bool orderByLastName,
            [FromQuery(Name = "orderByCreationDate")] bool orderByCreationDate,
            [FromQuery(Name = "orderByLastConnectionDate")] bool orderByLastConnectionDate)
        {
            var orderBy = new UserSearchOrderBy
            {
                LastName = orderByLastName,
                CreationDate = orderByCreationDate,
                LastConnectionDate = orderByLastConnectionDate
            };

            return service.Find(filter, orderBy);
        }
    }
}
