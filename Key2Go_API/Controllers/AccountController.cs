using Application.Service;
using Contract.Account.Request;
using Contract.User.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {

        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<ActionResult<UserResponse>> GetProfile()
        {
            var userId = GetCurrentUserId();
            var response = await _accountService.GetProfileAsync(userId);

            if (response == null)
            {
                return NotFound("User not found");
            }
            return Ok(response);
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
        {
            var userId = GetCurrentUserId();

            var updated = await _accountService.UpdateProfileAsync(userId, request);
            if (!updated) return BadRequest("No se pudo actualizar el perfil.");

            return Ok("Se actualizo el perfil");
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var userId = GetCurrentUserId();

            var changed = await _accountService.ChangePasswordAsync(userId, request);
            if (!changed) return BadRequest("Contraseña actual incorrecta.");

            return Ok("Se actualizo la contraseña");
        }


        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                throw new Exception("User id not found in token");

            return int.Parse(userIdClaim);
        }
    }
}
