using System;

namespace Roomify.Contracts.ResponseModels.ManageBooking;

public class GetBookingToManageResponseModel
{
    public List<ManageBookingModel> manageBookings { get; set; } = new List<ManageBookingModel>();
}

public class ManageBookingModel{
    public string BookingId { get; set; } ="";
    public string BookingDescription { get; set; } ="";
    public int RoomId { get; set; }
    public string RoomName { get; set; } ="";
    public string BookingDate { get; set; } ="";
    public string UserName { get; set; } ="";
    public int StatusId { get; set; }
    public string StatusName { get; set; } ="";
    public int BuildingId { get; set; }
    public List<ManageSessionModel> manageSessions { get; set; } = new List<ManageSessionModel>();
}

public class ManageSessionModel{
    public int SessionId { get; set; }
    public string SessionName { get; set; } ="";
}

