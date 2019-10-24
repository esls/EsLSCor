using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EsLSCor.Entities;
using EsLSCor.Models;
using EsLSCor.Services;

namespace EsLSCor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IUserService _service;

        public AuthController(ILogger<AuthController> logger, IUserService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody]FrontAuthModel frontAuth)
        {
            if (!User.IsInRole(Role.Admin) && !_service.IsDbEmpty())
                return Unauthorized();
            var creationResult = _service.CreateNew(frontAuth.Username, frontAuth.Password);
            switch (creationResult)
            {
                case UserCreationResult.BadUsername:
                    return BadRequest("Username doesn't meet requirements");
                case UserCreationResult.WeakPassword:
                    return BadRequest("Password doesn't meet requirements");
                case UserCreationResult.AlreadyExists:
                    return Conflict("User already exists");

            }

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody]FrontAuthModel frontAuth)
        {
            var token = _service.GetToken(frontAuth.Username, frontAuth.Password);
            if (token != null)
                return Ok(token);
            else
                return Unauthorized();
        }
    }
}
