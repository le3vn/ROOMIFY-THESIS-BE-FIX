using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageBooking;

namespace Roomify.Contracts.RequestModels.ManageBooking;

public class PostCheckInRequestModel : IRequest<PostCheckInResponseModel>
{
    public Guid BookingId { get; set; }
}
