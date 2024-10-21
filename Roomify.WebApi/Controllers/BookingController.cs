using Microsoft.AspNetCore.Mvc;
using MediatR;
using Roomify.Contracts.RequestModels.ManageBooking;
using Roomify.Contracts.ResponseModels.ManageBooking;
using System.Threading.Tasks;

namespace Roomify.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BookingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create-booking")]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequestModel request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                return BadRequest("Invalid booking request.");
            }

            // Validate request
            if (string.IsNullOrWhiteSpace(request.UserId) || request.RoomId <= 0 || request.SessionBookedList.Count == 0)
            {
                return BadRequest("Missing required fields.");
            }

            // Send the request with cancellation token
            var response = await _mediator.Send(request, cancellationToken);

            if (response.Success == "true")
            {
                return CreatedAtAction(nameof(CreateBooking), new { id = response.Message }, response);
            }
            else
            {
                return BadRequest(response.Message);
            }
        }
        [HttpPost("approve")]
        public async Task<IActionResult> ApproveBooking([FromBody] ApproveBookingRequestModel request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request data");
            }

            try
            {
                // Send the request to MediatR handler with the cancellation token
                ApproveBookingResponseModel result = await _mediator.Send(request, cancellationToken);

                // Check the response from the handler
                if (result.Success == "true")
                {
                    return Ok(new { Message = result.Message });
                }
                else
                {
                    return BadRequest(new { Message = result.Message });
                }
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation
                return StatusCode(499, new { Message = "Request was cancelled by the client." });
            }
            catch (Exception ex)
            {
                // Handle any unexpected errors
                return StatusCode(500, new { Message = "An error occurred during the booking approval process.", Details = ex.Message });
            }
        }
        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn([FromBody] PostCheckInRequestModel request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                return BadRequest("Request cannot be null.");
            }

            if (request.BookingId == Guid.Empty)
            {
                return BadRequest("Invalid Booking ID.");
            }

            var response = await _mediator.Send(request, cancellationToken);

            if (response.Success == "True")
            {
                return Ok(response);
            }

            return BadRequest(response);
        }
        [HttpGet]
        public async Task<ActionResult<GetApproverViewResponseModel>> Get([FromQuery] GetApproverViewRequestModel request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

    }
}
