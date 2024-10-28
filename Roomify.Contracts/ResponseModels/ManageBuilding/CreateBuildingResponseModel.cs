using System;

namespace Roomify.Contracts.ResponseModels.ManageBuilding;

public class CreateBuildingResponseModel
{
    public int BuildingId { get; set; }
    public string Success { get; set; } = "";
    public string Message { get; set; } = "";
}
