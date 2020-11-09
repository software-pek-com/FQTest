using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MoWebApp.Core;

namespace MoWebApp.Controllers
{
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
