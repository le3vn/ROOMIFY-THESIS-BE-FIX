using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Roomify.Contracts.RequestModels.ManageGroup;
using Roomify.Contracts.ResponseModels.ManageGroup;

namespace Roomify.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IMediator _mediator;
        public GroupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create-group")]
        public async Task<ActionResult<CreateGroupResponseModel>> Post([FromForm] CreateGroupRequestModel request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return response;
        }
        [HttpPost("{id}")]
        public async Task<ActionResult<EditGroupResponseModel>> Put(int id, [FromForm] EditGroupModel request, CancellationToken ct)
        {
            var model = new EditGroupRequestModel
            {
                GroupId = id,
                GroupName = request.GroupName,
                LSCApprover = request.LSCApprover,
                BMApprover = request.BMApprover,
                SLCApprover = request.SLCApprover,
                SSOApprover = request.SSOApprover
            };

            var response = await _mediator.Send(model, ct);
            return response;
        }
        [HttpGet("get-group-detail")]
        public async Task<ActionResult<GetGroupDetailResponseModel>> GetGroup(int id, CancellationToken cancellationToken)
        {
            var request = new GetGroupDetailRequestModel { GroupId = id };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpDelete("delete-group/{id}")]
        public async Task<ActionResult<DeleteGroupResponseModel>> Delete(int id, CancellationToken ct)
        {
            var request = new DeleteGroupRequestModel
            {
                GroupId = id
            };

            var response = await _mediator.Send(request, ct);

            return response;
        }

    }
}
