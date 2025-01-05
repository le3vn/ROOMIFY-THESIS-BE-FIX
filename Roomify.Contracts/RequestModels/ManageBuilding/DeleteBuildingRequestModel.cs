using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageBuilding;

namespace Roomify.Contracts.RequestModels.ManageBuilding;

public class DeleteBuildingRequestModel : IRequest<DeleteBuildingResponseModel>
{
    public int BuildingId { get; set; }
}
