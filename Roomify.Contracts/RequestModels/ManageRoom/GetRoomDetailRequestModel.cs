using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageRoom;

namespace Roomify.Contracts.RequestModels.ManageRoom;

public class GetRoomDetailRequestModel : IRequest<GetRoomDetailResponseModel>
{
    public int RoomId { get; set; }
}
