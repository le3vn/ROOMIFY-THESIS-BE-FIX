using System;

namespace Roomify.Contracts.ResponseModels.ManageBooking;

public class CancelBookingResponseModel
{
    public Guid BookingId { get; set; }
    public string Success { get; set; } = "";
    public string Message { get; set; } = "";
}
