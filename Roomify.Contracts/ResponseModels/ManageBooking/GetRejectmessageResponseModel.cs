using System;

namespace Roomify.Contracts.ResponseModels.ManageBooking;

public class GetRejectmessageResponseModel
{
    public string RejectMessage { get; set; } ="";
    public string CreatedBy { get; set; } ="";
    public DateTime CreatedAt { get; set; }
    public string MinioUrl { get; set; } ="";
}
