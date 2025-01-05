using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Roomify.Contracts.RequestModels.ManageRoom;
using Roomify.Contracts.ResponseModels.ManageRoom;

namespace Roomify.WebApi.Controllers
{
    [Route("api/v1/Room")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IMediator _mediator;
        public RoomController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create-room")]
        public async Task<ActionResult<CreateRoomResponseModel>> Post([FromForm] CreateRoomRequestModel request, [FromServices] IValidator<CreateRoomRequestModel> validator, CancellationToken ct)
        {
            var validationResult = await validator.ValidateAsync(request, ct);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return ValidationProblem(ModelState);
            }
            var response = await _mediator.Send(request, ct);
            return response;
        }
        [HttpGet("get-all-room")]
        public async Task<ActionResult<GetRoomResponseModel>> GetAllRoom([FromQuery] GetRoomRequestModel request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpGet("get-room-group")]
        public async Task<ActionResult<GetRoomGroupsResponseModel>> GetRoomGroup([FromQuery] GetRoomGroupsRequestModel request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpGet("get-user-view")]
        public async Task<ActionResult<GetRoomUserViewResponseModel>> GetRoomUser([FromQuery] GetRoomUserViewRequestModel request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpGet("get-room-detail")]
        public async Task<ActionResult<GetRoomDetailResponseModel>> GetRoom(int id, CancellationToken cancellationToken)
        {
            var request = new GetRoomDetailRequestModel { RoomId = id };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet("get-room-schedule")]
        public async Task<ActionResult<GetRoomScheduleResponseModel>> GetRoomSchedule(int id, CancellationToken cancellationToken)
        {
            var request = new GetRoomScheduleRequestModel { RoomId = id };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet("get-room-available")]
        public async Task<ActionResult<GetRoomAvailableResponseModel>> GetRoomAvailable(Guid id, CancellationToken cancellationToken)
        {
            var request = new GetRoomAvailableRequestModel { BookingId = id };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpPost("{id}")]
        public async Task<ActionResult<UpdateRoomResponseModel>> Put(int id, [FromForm] UpdateRoomModel request, CancellationToken ct)
        {
            var model = new UpdateRoomRequestModel
            {
                RoomId = id,
                Name = request.Name,
                Description = request.Description,
                RoomTypeId = request.RoomTypeId,
                BuildingId = request.BuildingId,
                Capacity = request.Capacity,
                RoomPicture = request.RoomPicture,
                RoomGroupId = request.RoomGroupId
            };

            var response = await _mediator.Send(model, ct);
            return response;
        }
        [HttpDelete("delete-room/{id}")]
        public async Task<ActionResult<DeleteRoomResponseModel>> Delete(int id, 
            [FromServices] IValidator<DeleteRoomRequestModel> validator, CancellationToken ct)
        {
            var request = new DeleteRoomRequestModel
            {
                RoomId = id
            };

            var validationResult = await validator.ValidateAsync(request, ct);

            if(!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                return ValidationProblem(ModelState);
            }

            var response = await _mediator.Send(request, ct);

            return response;
        }

    }
}
