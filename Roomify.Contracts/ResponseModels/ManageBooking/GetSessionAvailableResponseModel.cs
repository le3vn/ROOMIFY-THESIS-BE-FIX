using System;
using MediatR;

namespace Roomify.Contracts.ResponseModels.ManageBooking;

public class GetSessionAvailableResponseModel
{
    public List<AvailableSession> AvailableSessions { get; set; } = new List<AvailableSession>();
}

public class AvailableSession{
    public int SessionId { get; set; }
    public string SessionName { get; set; } ="";
}
