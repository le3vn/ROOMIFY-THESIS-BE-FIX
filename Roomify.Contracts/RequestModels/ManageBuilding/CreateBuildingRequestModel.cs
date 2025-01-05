using System;
using MediatR;
using Microsoft.AspNetCore.Http;
using Roomify.Contracts.ResponseModels.ManageBuilding;

namespace Roomify.Contracts.RequestModels.ManageBuilding;

public class CreateBuildingRequestModel : IRequest<CreateBuildingResponseModel>
{
    public string BuildingName { get; set; } = "";
    public IFormFile BuildingPicture { get; set; } = null!;
}
