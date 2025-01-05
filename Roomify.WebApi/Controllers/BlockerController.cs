using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Roomify.Contracts.RequestModels.ManageBlocker;
using Roomify.Contracts.ResponseModels.ManageBlocker;

namespace Roomify.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlockerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BlockerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create-blocker")]
        public async Task<ActionResult<PostBlockerResponseModel>> Post([FromForm] PostBlockerRequestModel request, CancellationToken ct)
        {
            var response = await _mediator.Send(request, ct);

            // Check if the operation was not successful (Success is "false")
            if (response != null && response.Success == "false")
            {
                // Return a BadRequest with the failure message
                return BadRequest(new { message = response.Message });
            }

            // If no errors, return the response
            return Ok(response);
        }
        [HttpGet("get-blocker-list")]
        public async Task<ActionResult<GetBlockerListResponseModel>> GetRoomGroup([FromQuery] GetBlockerListRequestModel request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpDelete("delete-blocker")]
        public async Task<ActionResult<DeleteBlockerResponseModel>> DeleteBlocker([FromQuery] DeleteBlockerRequestModel request, CancellationToken ct)
        {
            var response = await _mediator.Send(request, ct);

            // Check if the operation was not successful (Success is "false")
            if (response != null && response.Success == "false")
            {
                // Return a BadRequest with the failure message
                return BadRequest(new { message = response.Message });
            }

            // If no errors, return the response
            return Ok(response);
        }
        [HttpPost("deactive-blocker")]
        public async Task<IActionResult> CDeactive([FromBody] DeactiveBlockerRequestModel request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                return BadRequest("Request cannot be null.");
            }

            if (request.BlockerId == 0)
            {
                return BadRequest("Invalid Blocker ID.");
            }

            var response = await _mediator.Send(request, cancellationToken);

            if (response.Success == "true")
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpGet("get-blocker-detail")]
        public async Task<ActionResult<GetBlockerDetailResponseModel>> GetBlockerDetail([FromQuery] GetBlockerDetailRequestModel request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpPost("update-blocker")]
        public async Task<ActionResult<UpdateBlockerResponseModel>> Post([FromForm] UpdateBlockerRequestModel request, CancellationToken ct)
        {
            var response = await _mediator.Send(request, ct);

            // Check if the operation was not successful (Success is "false")
            if (response != null && response.Success == "false")
            {
                // Return a BadRequest with the failure message
                return BadRequest(new { message = response.Message });
            }

            // If no errors, return the response
            return Ok(response);
        }

    }
}
