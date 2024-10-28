using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageBuilding;

namespace Roomify.Contracts.RequestModels.ManageBuilding;

public class GetBuildingRequestModel : IRequest<GetBuildingResponseModel>
{
    public string? Search { get; set; } = string.Empty;
}
