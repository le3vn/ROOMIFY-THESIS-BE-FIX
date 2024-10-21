using System;

namespace Roomify.Contracts.ResponseModels.ManageRoom;

public class CreateRoomResponseModel
{
    public int RoomId { get; set; }
    public string Success { get; set; } = "";
    public string Message { get; set; } = "";
}
