using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Roomify.Contracts.RequestModels.ManageNotification;
using Roomify.Contracts.ResponseModels.ManageNotification;

namespace Roomify.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IMediator _mediator;
        public NotificationController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("get-notification")]
        public async Task<ActionResult<GetNotificationResponseModel>> Get([FromQuery] GetNotificationRequestModel request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpGet("get-notification-unread")]
        public async Task<ActionResult<GetNotificationUnreadResponseModel>> Get([FromQuery] GetNotificationUnreadRequestModel request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpPut("read-notification")]
        public async Task<ActionResult<ReadNotificationResponseModel>> Put([FromBody] ReadNotificationRequestModel request, CancellationToken ct)
        {
            var response = await _mediator.Send(request, ct);
            return response;
        }
        [HttpDelete("delete-notification")]
        public async Task<ActionResult<DeleteNotificationResponseModel>> DeleteNotification([FromQuery] DeleteNotificationRequestModel model, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(model, cancellationToken);
            return Ok(response);
        }
        [HttpDelete("delete-all")]
        public async Task<ActionResult<DeleteAllNotificationResponseModel>> DeleteAllNotification([FromBody] DeleteAllNotificationRequestModel model, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(model, cancellationToken);
            return Ok(response);
        }
    }
}
