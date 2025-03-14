using System;

namespace Roomify.Contracts.ResponseModels.ManageBuilding;

public class GetBuildingDetailResponseModel
{
    public string Name { get; set; } = "";
    public string MinioUrl { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; }
    public string CreatedBy { get; set; } = "";
    public DateTimeOffset UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } = "";
}
