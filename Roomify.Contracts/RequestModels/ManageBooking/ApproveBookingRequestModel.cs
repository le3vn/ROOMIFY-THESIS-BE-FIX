using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageBooking;

namespace Roomify.Contracts.RequestModels.ManageBooking;

public class ApproveBookingRequestModel : IRequest<ApproveBookingResponseModel>
{
    public string UserId { get; set; } = "";
    public Guid BookingId { get; set; }
    public bool IsApproved { get; set; }
    public string? RejectMessage { get; set; }
}
