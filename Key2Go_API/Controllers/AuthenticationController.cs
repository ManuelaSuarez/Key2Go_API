using Application.Abstraction.ExternalService;
using Contract.Auth.Request;
using Contract.Auth.Response;
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
        public async Task<ActionResult<string>> Login([FromBody] LoginRequest request)
        {
            var response = new LoginResponse();
            var token = await _authenticationService.Login(request);

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(response.Message);
            }
            return Ok(token);
        }
    }
}
