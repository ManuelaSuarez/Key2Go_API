using Application.Service;
using Contract.Payment.Request;
using Contract.Payment.Response;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<ActionResult<List<PaymentResponse>>> GetAll()
        {
            var response = await _paymentService.GetAll();

            if (!response.Any())
            {
                return NotFound("No payments Found");
            }
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PaymentResponse>> GetById([FromRoute] int id)
        {
            var response = await _paymentService.GetById(id);

            if (response == null)
            {
                return NotFound("No payments found");
            }

            return Ok(response);
        }

        [HttpGet("by-trip/{tripId:int}")]
        public async Task<ActionResult<PaymentResponse>> GetByTripId([FromRoute] int tripId)
        {
            var response = await _paymentService.GetByTripIdAsync(tripId);

            if (response == null)
                return NotFound("No payment associated with this trip");

            return Ok(response);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PaymentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _paymentService.Update(id, request);

            if (updated is null)
            {
                return NotFound("Payment not found");
            }

            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _paymentService.Delete(id);
            if (!result)
                return NotFound("Payment not found");

            return NoContent();
        }
    }
}
