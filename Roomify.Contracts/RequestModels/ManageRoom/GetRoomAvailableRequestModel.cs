using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageRoom;

namespace Roomify.Contracts.RequestModels.ManageRoom;

public class GetRoomAvailableRequestModel : IRequest<GetRoomAvailableResponseModel>
{
    public Guid BookingId { get; set; }
}
