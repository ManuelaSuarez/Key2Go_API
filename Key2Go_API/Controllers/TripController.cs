using Application.Service;
using Contract.Trip.Request;
using Contract.Trip.Response;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [HttpGet("my")]
        [Authorize(Policy = nameof(RoleType.User))]
        public async Task<ActionResult<List<TripResponse>>> GetMyTrips()
        {
            var currentUserId = GetCurrentUserId();

            var response = await _tripService.GetAllByUserId(currentUserId);

            if (!response.Any())
            {
                return NotFound("No trips found for current user");
            }

            return Ok(response);
        }

        [HttpGet("my/{id}")]
        [Authorize(Policy = nameof(RoleType.User))]
        public async Task<ActionResult<TripResponse>> GetMyTripbyId([FromRoute] int id)
        {
            var currentUserId = GetCurrentUserId();

            var response = await _tripService.GetByUserId(id, currentUserId);

            if (response == null)
            {
                return NotFound("Trip not found for current user");
            }

            return Ok(response);
        }


        [HttpGet]
        [Authorize(Policy = nameof(RoleType.Admin))]
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
        [Authorize(Policy = nameof(RoleType.Admin))]
        public async Task<ActionResult<TripResponse>> GetById([FromRoute] int id)
        {
            var response = await _tripService.GetById(id);

            if (response == null)
            {
                return NotFound("No trips found");
            }
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Policy = nameof(RoleType.Admin))]
        public async Task<IActionResult> AdminCreate([FromBody] AdminTripRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _tripService.AdminCreate(request);

            if (result == null)
                return BadRequest("Could not create trip");

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPost("reserve")]
        [Authorize(Policy = nameof(RoleType.User))]
        public async Task<IActionResult> Create([FromBody] TripRequest request)
        {
            var currentUserId = GetCurrentUserId();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _tripService.Create(currentUserId, request);

            if (result == null)
                return BadRequest("Could not create trip");

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = nameof(RoleType.Admin))]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _tripService.Delete(id);
            if (!result)
                return NotFound("Trip not found");

            return NoContent();
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = nameof(RoleType.Admin))]
        public async Task<IActionResult> AdminUpdate(int id, [FromBody] AdminTripUpdate request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _tripService.AdminUpdate(id, request);

            if (updated is null)
                return NotFound("Trip not found");

            return Ok(updated);
        }

        [HttpPut("modify/{id:int}")]
        [Authorize(Policy = nameof(RoleType.User))]
        public async Task<IActionResult> Update(int id, [FromBody]TripUpdate request)
        {
            var currentUserId = GetCurrentUserId();
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _tripService.Update(id, currentUserId, request);

            if (updated is null)
                return NotFound("Trip not found");

            return Ok(updated);
        }

        [HttpPut("{id}/cancel")]
        [Authorize(Policy = nameof(RoleType.User))]
        public async Task<IActionResult> CancelTrip(int id)
        {
            var updated = await _tripService.CancelTrip(id);

            if (!updated)
                return BadRequest("No se puede cancelar esta reserva");

            return Ok("Reserva cancelada con éxito");
        }

        [HttpPut("{id}/start")]
        [Authorize(Policy = nameof(RoleType.Admin))]
        public async Task<IActionResult> Start(int id)

        {
            var updated = await _tripService.StartTrip(id);

            if (!updated)
                return BadRequest("No se puede iniciar este viaje");

            return Ok("Viaje iniciado");
        }

        [HttpPut("{id}/finish")]
        [Authorize(Policy = nameof(RoleType.Admin))]
        public async Task<IActionResult> Finish(int id, [FromBody] int finalKm)
        {
            var updated = await _tripService.FinishTrip(id, finalKm);

            if (!updated)
                return BadRequest("No se puede finalizar el viaje");

            return Ok("Viaje finalizado");
        }

        [HttpGet("AllPendingTrip")]
        [Authorize(Policy = nameof(RoleType.Admin))]
        public async Task<ActionResult<TripResponse>> GetAllPending()
        {
            var response = await _tripService.GetByStatus(1);

            if (!response.Any())
            {
                return NotFound("No Trips Found");
            }
            return Ok(response);
        }

        [HttpGet("AllActiveTrip")]
        [Authorize(Policy = nameof(RoleType.Admin))]
        public async Task<ActionResult<TripResponse>> GetAllActive()
        {
            var response = await _tripService.GetByStatus(2);

            if (!response.Any())
            {
                return NotFound("No Trips Found");
            }
            return Ok(response);
        }

        [HttpGet("AllCancelledTrip")]
        [Authorize(Policy = nameof(RoleType.Admin))]
        public async Task<ActionResult<TripResponse>> GetAllCancelled()
        {
            var response = await _tripService.GetByStatus(3);

            if (!response.Any())
            {
                return NotFound("No Trips Found");
            }
            return Ok(response);
        }

        [HttpGet("AllFinishedTrip")]
        [Authorize(Policy = nameof(RoleType.Admin))]
        public async Task<ActionResult<TripResponse>> GetAllFinished()
        {
            var response = await _tripService.GetByStatus(4);

            if (!response.Any())
            {
                return NotFound("No Trips Found");
            }
            return Ok(response);
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
