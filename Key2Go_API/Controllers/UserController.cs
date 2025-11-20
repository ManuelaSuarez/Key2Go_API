using Application.Service;
using Contract.User.Request;
using Contract.User.Response;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

//PUT/api/account/update-profile
//PUT/api/account/change-password


namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Policy = nameof(RoleType.Admin))]
        public async Task<ActionResult<List<UserResponse>>> GetAll()
        {
            var response = await _userService.GetAll();

            if (!response.Any())
            {
                return NotFound("No users found");
            }
            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = nameof(RoleType.Admin))]
        public async Task<ActionResult<UserResponse>> GetById([FromRoute] int id)
        {
            var response = await _userService.GetById(id);

            if (response == null)
            {
                return NotFound("User not found");
            }
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Policy = nameof(RoleType.SuperAdmin))]
        public async Task<IActionResult> Create([FromBody] UserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.Create(request);

            if (result == null)
                return BadRequest("Email already in use");

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = nameof(RoleType.SuperAdmin))]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userService.Delete(id);
            if (!result)
                return NotFound("User not found");

            return NoContent();
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = nameof(RoleType.SuperAdmin))]
        public async Task<IActionResult> Update(int id, [FromBody] UserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _userService.Update(id, request);

            if (updated is null)
                return NotFound("User not found");

            return Ok(updated);
        }
    }
}
