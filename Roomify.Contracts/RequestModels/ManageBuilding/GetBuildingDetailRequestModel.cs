using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageBuilding;

namespace Roomify.Contracts.RequestModels.ManageBuilding;

public class GetBuildingDetailRequestModel : IRequest<GetBuildingDetailResponseModel>
{
    public int BuildingId { get; set; }
}
