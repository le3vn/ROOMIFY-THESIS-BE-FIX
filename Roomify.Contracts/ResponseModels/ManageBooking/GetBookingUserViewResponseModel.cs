using System;
using Microsoft.AspNetCore.Http;

namespace Roomify.Contracts.ResponseModels.ManageBooking;

public class GetBookingUserViewResponseModel
{
    public int TotalData { get; set; }
    public List<UserBookingModel> UserBookings { get; set; } = new List<UserBookingModel>();
}

public class UserBookingModel
{
    public string BookingId { get; set; }="";
    public string BookingDescription { get; set; }="";
    public int RoomId { get; set; }
    public string RoomName { get; set; } ="";
    public string RoomMinioUrl { get; set; } ="";
    public List<ApprovalHistory> ApprovalHistories { get; set; } = new List<ApprovalHistory>();
    public List<SessionBookingListModel> SessionBookingList { get; set; } = new List<SessionBookingListModel>();

    public string? QrMinioUrl { get; set; }
    public string CheckInPlace { get; set; } = "";
}

public class ApprovalHistory
{
    public string ApprovalUserId { get; set; }="";
    public string ApprovalName { get; set; }="";
    public string ApprovalMinioUrl { get; set; }="";
    public int ApprovalStatusId { get; set; }
    public string ApprovalStatus { get; set;}="";
    public DateTime ApprovedAt { get; set; }
}

public class SessionBookingListModel
{
    public int SessionId { get; set; }
    public string SessionName { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}

