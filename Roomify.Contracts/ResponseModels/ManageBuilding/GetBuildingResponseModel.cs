using System;

namespace Roomify.Contracts.ResponseModels.ManageBuilding;

public class GetBuildingResponseModel
{
    public List<BuildingModel> BuildingList { get; set; } = new List<BuildingModel>();
    public int TotalData { get; set; }
}
public class BuildingModel
{
    public int BuildingId { get; set; }
    public string Name { get; set; } = "";
    public string MinioUrl { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; }
    public string CreatedBy { get; set; } = "";
    public DateTimeOffset UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } = "";
}
