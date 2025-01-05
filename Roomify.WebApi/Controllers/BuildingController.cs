using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Roomify.Contracts.RequestModels.ManageBuilding;
using Roomify.Contracts.ResponseModels.ManageBuilding;


namespace Roomify.WebApi.Controllers
{
    [Route("api/v1/Building")]
    [ApiController]
    public class BuildingController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BuildingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create-building")]
        public async Task<ActionResult<CreateBuildingResponseModel>> Post([FromForm] CreateBuildingRequestModel request, CancellationToken ct)
        {
            var response = await _mediator.Send(request, ct);
            return response;
        }
        [HttpPost("edit-building/{id}")]
        public async Task<ActionResult<UpdateBuildingResponseModel>> Put(int id, [FromForm] UpdateBuildingModel request, CancellationToken ct)
        {
            var model = new UpdateBuildingRequestModel
            {
                BuildingId = id,
                Name = request.Name,
                BuildingPicture = request.BuildingPicture
            };

            var response = await _mediator.Send(model, ct);
            return response;
        }
        [HttpGet("get-all-building")]
        public async Task<ActionResult<GetBuildingResponseModel>> GetAllBuilding([FromQuery] GetBuildingRequestModel request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpGet("get-building/{id}")]
        public async Task<ActionResult<GetBuildingDetailResponseModel>> GetBuilding(int id, CancellationToken cancellationToken)
        {
            var request = new GetBuildingDetailRequestModel { BuildingId = id };
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpDelete("delete-building/{id}")]
        public async Task<ActionResult<DeleteBuildingResponseModel>> DeleteBuilding(int id, CancellationToken ct)
        {
            var request = new DeleteBuildingRequestModel
            {
                BuildingId = id
            };

            var response = await _mediator.Send(request, ct);

            return response;
        }
    }
}