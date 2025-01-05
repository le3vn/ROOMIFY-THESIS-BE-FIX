using System;

namespace Roomify.Contracts.ResponseModels.ManageRoom;

public class GetRoomDetailResponseModel
{
    public string Name { get; set; } = "";
    public string RoomType { get; set; } = "";
    public int RoomTypeId { get; set; }
    public string Description { get; set; } = "";
    public string Building { get; set; } = "";
    public int BuildingId { get; set; }
    public string GroupName { get; set; } = "";
    public int RoomGroupId { get; set; }
    public int Capacity { get; set; }
    public string SSOApprover { get; set; } = "";
    public string SLCApprover { get; set; } = "";
    public string LSCApprover { get; set; } = "";
    public string BMApprover { get; set; } = "";   
    public string MinioUrl { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; }
    public string CreatedBy { get; set; } = "";
    public DateTimeOffset UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } = "";
}
