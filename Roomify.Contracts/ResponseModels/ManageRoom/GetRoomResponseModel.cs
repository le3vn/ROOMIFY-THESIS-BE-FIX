using System;

namespace Roomify.Contracts.ResponseModels.ManageRoom;

public class GetRoomResponseModel
{
    public List<GetRoomModel> RoomList { get; set; } = new List<GetRoomModel>();
    public int TotalData { get; set; }
}

public class GetRoomModel
{
    public int RoomId { get; set; }
    public string Name { get; set; } = "";
    public string RoomType { get; set; } = "";
    public string Description { get; set; } = "";
    public string Building { get; set; } = "";
    public int Capacity { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string CreatedBy { get; set; } = "";
    public DateTimeOffset UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } = "";
}
