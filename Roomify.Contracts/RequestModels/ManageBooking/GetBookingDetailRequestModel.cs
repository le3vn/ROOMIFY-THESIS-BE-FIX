using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageBooking;

namespace Roomify.Contracts.RequestModels.ManageBooking;

public class GetBookingDetailRequestModel : IRequest<GetBookingDetailResponseModel>
{
   public string BookingId {get; set;} ="";
}
