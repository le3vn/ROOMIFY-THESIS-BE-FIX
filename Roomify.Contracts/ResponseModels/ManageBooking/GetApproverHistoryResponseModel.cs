using System;

namespace Roomify.Contracts.ResponseModels.ManageBooking;

public class GetApproverHistoryResponseModel
{
    public int TotalData { get; set; }
    public List<GetApproverHistoryViewmodel> ApproverHistoryViewModel { get; set; } = new List<GetApproverHistoryViewmodel>();

}

public class GetApproverHistoryViewmodel
{
    public Guid BookingId { get; set; }
    public string BookingDescription { get; set; } = string.Empty;
    public string BookerName { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public string RoomName { get; set;} = string.Empty;
    public string EvidenceMinioUrl { get; set; } = "";
    public bool IsCanceled { get; set; }
    public string RoomMinioUrl { get; set; } ="";
    public string? BookingOrganizationName { get; set; }
    public string BookingInstitutionalId { get; set; } = "";
    public string BookerUserRole { get; set; } = "";
    public string BookerMinioUrl { get; set; } ="";
    public List<SessionBookedHistoryList> SessionList { get; set; } = new List<SessionBookedHistoryList>();
    public List<ApproverListHistory> ApproverHistory { get; set; } = new List<ApproverListHistory>();
}
public class SessionBookedHistoryList
{
    public int SessionId { get; set; }
    public string SessionName { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
public class ApproverListHistory
{
    public string ApproverUserId { get; set; } =string.Empty;
    public string ApproverUserName { get; set; } = string.Empty;
    public string ApproverMinioUrl { get; set; } = string.Empty;
    public int StatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime ApprovedAt { get; set; }
}
