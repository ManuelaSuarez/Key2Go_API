

using Application.Service;
using Contract.Car.Request;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    
    public class CarController : ControllerBase
    {
        private readonly ICarService _carService;

        public CarController(ICarService carService)
        {
            _carService = carService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetAll()
        {
            var response = await _carService.GetAll();
            if (!response.Any())
            {
                return NotFound("No hay autos disponibles");
            }
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetById([FromRoute] int id)
        {
            var response = await _carService.GetById(id);
            if (response == null)
            {
                return NotFound("Car not found");
            }
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Policy = nameof(RoleType.Admin))]
        public async Task<IActionResult> Create([FromBody] CarRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _carService.Create(request);

            if (result == null)
                return BadRequest("Could not create car");

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = nameof(RoleType.Admin))]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _carService.Delete(id);
            if (!result)
            {
                return NotFound("Car not found");
            }
            return NoContent();
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = nameof(RoleType.Admin))]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CarRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _carService.Update(id, request);

            if (result == null)
                return NotFound("Car not found");

            return Ok(result);
        }
    }
}
