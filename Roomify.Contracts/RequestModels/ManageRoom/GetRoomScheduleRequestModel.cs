using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageRoom;

namespace Roomify.Contracts.RequestModels.ManageRoom;

public class GetRoomScheduleRequestModel : IRequest<GetRoomScheduleResponseModel>
{
    public int RoomId { get; set; }
}
