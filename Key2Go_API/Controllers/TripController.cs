using Application.Service;
using Contract.Trip.Request;
using Contract.Trip.Response;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        [HttpGet] 
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

        [HttpPost("register-trip")]
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
        public async Task<IActionResult> Update(int id, [FromBody] TripUpdate request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _tripService.Update(id, request);

            if (updated is null)
                return NotFound("Trip not found");

            return Ok(updated);
        }


        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelTrip(int id)
        {
            var updated = await _tripService.CancelTrip(id);

            if (!updated)
                return BadRequest("No se puede cancelar esta reserva");

            return Ok("Reserva cancelada con éxito");
        }

        [HttpPut("{id}/start")]
        public async Task<IActionResult> Start(int id)

        {
            var updated = await _tripService.StartTrip(id);

            if (!updated)
                return BadRequest("No se puede iniciar este viaje");

            return Ok("Viaje iniciado");
        }


        [HttpPut("{id}/finish")]
        public async Task<IActionResult> Finish(int id, [FromBody] int finalKm)
        {
            var updated = await _tripService.FinishTrip(id, finalKm);

            if (!updated)
                return BadRequest("No se puede finalizar el viaje");

            return Ok("Viaje finalizado");
        }

        [HttpGet("AllPendingTrip")]
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
        public async Task<ActionResult<TripResponse>> GetAllFinished()
        {
            var response = await _tripService.GetByStatus(4);

            if (!response.Any())
            {
                return NotFound("No Trips Found");
            }
            return Ok(response);
        }
    }
}
