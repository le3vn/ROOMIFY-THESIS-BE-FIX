using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Roomify.Contracts.RequestModels.ManageBuilding;
using Roomify.Contracts.ResponseModels.ManageBuilding;


namespace Roomify.WebApi.Controllers
{
    [Route("api/v1/Room")]
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
        
    }
}