using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageBooking;

namespace Roomify.Contracts.RequestModels.ManageBooking;

public class UpdateBookingRoomRequestModel : IRequest<UpdateBookingRoomResponseModel>
{
    public Guid BookingId { get; set; }
    public int RoomId { get; set; }
}
