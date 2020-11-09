using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MoWebApp.Core;
using MoWebApp.Models;

namespace MoWebApp.Controllers
{
    /// <summary>
    /// Represents a WebApi controller for performing database operations on <see cref="UserDocument">users</see>.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            Guard.ArgumentNotNull(logger, nameof(logger));

            _logger = logger;
        }
    }
}
