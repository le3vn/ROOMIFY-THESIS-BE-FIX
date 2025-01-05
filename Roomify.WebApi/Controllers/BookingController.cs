using Microsoft.AspNetCore.Mvc;
using MediatR;
using Roomify.Contracts.RequestModels.ManageBooking;
using Roomify.Contracts.ResponseModels.ManageBooking;
using System.Threading.Tasks;
using Roomify.Commons.RequestModels.ManageBooking;

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
        public async Task<IActionResult> CreateBooking([FromForm] CreateBookingRequestModel request, CancellationToken cancellationToken)
        {

            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                return BadRequest("UserId is required.");
            }

            if (request.RoomId <= 0)
            {
                return BadRequest("RoomId is invalid.");
            }

            if (request.SessionBookedList == null || request.SessionBookedList.Count == 0)
            {
                return BadRequest("SessionBookedList is required.");
            }

            // Log or inspect the list to ensure it's being bound correctly.
            foreach (var sessionId in request.SessionBookedList)
            {
                Console.WriteLine($"SessionId: {sessionId}");
            }

            // Continue with the rest of the logic...
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
        [HttpGet("get-approver-view")]
        public async Task<ActionResult<GetApproverViewResponseModel>> Get([FromQuery] GetApproverViewRequestModel request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpGet("get-user-view")]
        public async Task<ActionResult<GetBookingUserViewResponseModel>> GetUserView([FromQuery] GetBookingUserViewRequestModel request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpGet("get-available-session")]
        public async Task<ActionResult<GetSessionAvailableResponseModel>> Get([FromQuery] GetSessionAvailableRequestModel request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpGet("get-booking-traffic")]
        public async Task<ActionResult<GetAllBookingResponseModel>> Get([FromQuery] GetAllBookingRequestModel request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpPost("cancel-booking")]
        public async Task<ActionResult<CancelBookingResponseModel>> Post([FromBody] CancelBookingRequestModel request, CancellationToken ct)
        {
            var response = await _mediator.Send(request, ct);
            return response;
        }
        [HttpPost("update-booking")]
        public async Task<ActionResult<UpdateBookingRoomResponseModel>> Post([FromBody] UpdateBookingRoomRequestModel request, CancellationToken ct)
        {
            var response = await _mediator.Send(request, ct);
            return response;
        }

        [HttpGet("get-all-equipment")]
        public async Task<ActionResult<GetEquipmentResponseModel>> GetAllEquipment([FromQuery] GetEquipmentRequestModel request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpGet("get-lecturer-subject")]
        public async Task<ActionResult<GetLecturerSubjectResponseModel>> GetLecturerSubject([FromQuery] GetLecturerSubjectRequestModel request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpGet("get-institutional-id")]
        public async Task<ActionResult<GetInstitutionalIdResponseModel>> GetInstitutionalId([FromQuery] GetInstitutionalIdRequestModel request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpGet("get-reject-message")]
        public async Task<ActionResult<GetRejectmessageResponseModel>> GetRejectMessage([FromQuery] GetRejectMessageRequestModel request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpGet("get-booking-detail")]
        public async Task<ActionResult<GetBookingDetailResponseModel>> GetBookingDetail([FromQuery] GetBookingDetailRequestModel request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpGet("get-approver-history")]
        public async Task<ActionResult<GetApproverHistoryResponseModel>> GetApprovalHistory([FromQuery] GetApproverHistoryRequestModel request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpGet("get-all-booking")]
        public async Task<ActionResult<GetBookingToManageResponseModel>> Get([FromQuery] GetBookingToManageRequestModel request, CancellationToken cancellationToken)
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
