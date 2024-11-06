using System;
using MediatR;
using Microsoft.AspNetCore.Http;
using Roomify.Contracts.ResponseModels.ManageBuilding;

namespace Roomify.Contracts.RequestModels.ManageBuilding;

public class UpdateBuildingRequestModel : UpdateBuildingModel, IRequest<UpdateBuildingResponseModel>
{
    public int BuildingId { get; set; }
}

public class UpdateBuildingModel
{
    public string Name { get; set; } = "";
    public IFormFile BuildingPicture { get; set; } = null!;
}
