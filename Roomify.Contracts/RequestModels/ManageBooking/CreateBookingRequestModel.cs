using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageBooking;

namespace Roomify.Contracts.RequestModels.ManageBooking;

public class CreateBookingRequestModel : IRequest<CreateBookingResponseModel>
{
    public string UserId { get; set; } = "";
    public int RoomId { get; set; }
    public DateTime BookingDate { get; set; }
    public string BookingDescription { get; set; } = "";
    public List<GetSessionBookModel> SessionBookedList { get; set; } = new List<GetSessionBookModel>();
}
public class GetSessionBookModel
{
    public int SessionId { get; set; }
}
