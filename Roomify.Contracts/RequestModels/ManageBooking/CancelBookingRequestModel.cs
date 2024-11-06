using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageBooking;

namespace Roomify.Contracts.RequestModels.ManageBooking;

public class CancelBookingRequestModel: IRequest<CancelBookingResponseModel>
{
    public Guid BookingId { get; set; }
}
