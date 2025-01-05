using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageBooking;

namespace Roomify.Contracts.RequestModels.ManageBooking;

public class GetRejectMessageRequestModel : IRequest<GetRejectmessageResponseModel>
{
    public Guid BookingId { get; set; }
}
