using System;

namespace Roomify.Contracts.ResponseModels.ManageRoom;

public class GetRoomAvailableResponseModel
{
    public List<RoomAvailableModel> RoomAvailables { get; set; } = new List<RoomAvailableModel>();
    public int TotalData { get; set; }
}

public class RoomAvailableModel{
    public int RoomId { get; set; }
    public string RoomName { get; set; } ="";
}
