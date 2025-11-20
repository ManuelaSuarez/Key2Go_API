using Application.Abstraction.ExternalService;
using Contract.External.Auth.Request;
using Contract.External.Auth.Response;
using Contract.User.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            var result = await _authenticationService.Login(request);

            if (result == null || string.IsNullOrEmpty(result.Token))
            {
                return Unauthorized("Credenciales inválidas.");
            }

            return Ok(result);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authenticationService.Register(request);

            if (result == null)
                return BadRequest("Email already in use");

            return Ok(result);
        }
    }
}
