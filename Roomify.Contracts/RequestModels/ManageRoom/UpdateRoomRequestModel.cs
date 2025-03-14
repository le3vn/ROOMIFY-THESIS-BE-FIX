using System;
using MediatR;
using Microsoft.AspNetCore.Http;
using Roomify.Contracts.ResponseModels.ManageRoom;

namespace Roomify.Contracts.RequestModels.ManageRoom;

public class UpdateRoomRequestModel : UpdateRoomModel, IRequest<UpdateRoomResponseModel>
{
    public int RoomId { get; set; }
}

public class UpdateRoomModel
{
    public string Name { get; set; } = "";
    public int RoomTypeId { get; set; }
    public string Description { get; set; } = "";
    public int BuildingId { get; set; }
    public int Capacity { get; set; }
    public int RoomGroupId { get; set; }
    public IFormFile? RoomPicture { get; set; } = null!;
}
