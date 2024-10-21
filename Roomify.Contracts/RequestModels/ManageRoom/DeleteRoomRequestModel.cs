using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageRoom;

namespace Roomify.Contracts.RequestModels.ManageRoom;

public class DeleteRoomRequestModel : IRequest<DeleteRoomResponseModel>
{
    public int RoomId { get; set; }
}
