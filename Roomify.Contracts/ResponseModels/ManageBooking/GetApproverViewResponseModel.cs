using System;

namespace Roomify.Contracts.ResponseModels.ManageBooking;

public class GetApproverViewResponseModel
{
    public int TotalData { get; set; }
    public List<GetApproverViewmodel> ApproverViewModel { get; set; } = new List<GetApproverViewmodel>();
}

public class GetApproverViewmodel
{
    public Guid BookingId { get; set; }
    public string BookingDescription { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public string RoomName { get; set;} = string.Empty;
    public List<SessionBookedList> SessionList { get; set; } = new List<SessionBookedList>();
    public List<ApproverHistoryList> ApproverHistory { get; set; } = new List<ApproverHistoryList>();
}
public class SessionBookedList
{
    public int SessionId { get; set; }
    public string SessionName { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
public class ApproverHistoryList
{
    public string ApproverUserId { get; set; } =string.Empty;
    public string ApproverUserName { get; set; } = string.Empty;
    public int StatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;
}