using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Roomify.Contracts.RequestModels.ManageRole;
using Roomify.Contracts.ResponseModels.ManageRole;

namespace Roomify.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RoleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("get-user-role")]
        public async Task<ActionResult<GetRoleToChangeResponseModel>> GetUserRole([FromQuery] GetRoleToChangeRequestModel request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpPost("change-role")]
        public async Task<IActionResult> ChangeRole([FromBody] ChangeRoleRequestModel request, CancellationToken cancellationToken)
        {
            if (request == null || string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.RoleId))
            {
                return BadRequest("Invalid request.");
            }

            ChangeRoleResponseModel response = await _mediator.Send(request, cancellationToken);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response.Message);
        }
        [HttpGet("get-display-name")]
        public async Task<ActionResult<GetDisplayNameStudentOrganizationResponseModel>> GetRoomUser([FromQuery] GetDisplayNameStudentOrganizationRequestModel request, CancellationToken cancellationToken)
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
