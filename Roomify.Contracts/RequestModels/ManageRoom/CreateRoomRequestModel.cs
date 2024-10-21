using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageRoom;

namespace Roomify.Contracts.RequestModels.ManageRoom;

public class CreateRoomRequestModel : IRequest<CreateRoomResponseModel>
{
    public string RoomName { get; set; } = "";
    public string Description { get; set; } = "";
    public int RoomTypeId { get; set; }
    public int BuildingId { get; set; }
    public int Capacity { get; set; }
}
