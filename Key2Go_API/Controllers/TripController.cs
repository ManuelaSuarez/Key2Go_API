using Application.Service;
using Contract.Trip.Request;
using Contract.Trip.Response;
using Contract.User.Request;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class TripController : ControllerBase
    {
        private readonly ITripService _tripService;

        public TripController(ITripService tripService)
        {
            _tripService = tripService;
        }

        [HttpGet] // por qué a veces hacemos action result y a veces iaction result?
        public async Task<ActionResult<List<TripResponse>>> GetAll()
        {
            var response = await _tripService.GetAll();

            if (!response.Any())
            {
                return NotFound("No Trips Found");
            }
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TripResponse>> GetById([FromRoute] int id)
        {
            var response = await _tripService.GetById(id);

            if (response == null)
            {
                return NotFound("No trips found");
            }
            return Ok(response);
        }

        [HttpPost("register-car")]
        public async Task<IActionResult> Create([FromBody] TripRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _tripService.Create(request);

            if (result == null)
                return BadRequest("Could not create trip");

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _tripService.Delete(id);
            if (!result)
                return NotFound("Trip not found");

            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] TripRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _tripService.Update(id, request);

            if (updated is null)
                return NotFound("Trip not found");

            return Ok(updated);
        }
    }
}
