using System;

namespace Roomify.Contracts.ResponseModels.ManageBooking;

public class GetBookingDetailResponseModel
{
    public int totalData { get; set;}
    public List<GetBookingDetailModel> BookingDetailModel { get; set; } = new List<GetBookingDetailModel>();
}
public class GetBookingDetailModel
{
    public Guid BookingId { get; set; }
    public string BookingDescription { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public string RoomName { get; set;} = string.Empty;
    public string MinioUrl { get; set; } = "";    
    public bool IsCanceled { get; set; }
    public string RoomMinioUrl { get; set; } ="";
    public string? BookingOrganizationName { get; set; }
    public string BookingInstitutionalId { get; set; } = "";
    public string UserRole { get; set; } = "";
    public string BookerMinioUrl { get; set; } ="";
    public string BookingDate { get; set; } ="";
    public List<SessionBookingList> SessionList { get; set; } = new List<SessionBookingList>();
    public List<ApproverDetaillist> ApproverHistory { get; set; } = new List<ApproverDetaillist>();
    public List<EquipmentBookingList> EquipmentList { get; set; } = new List<EquipmentBookingList>();
}
public class ApproverDetaillist
{
    public string ApproverUserId { get; set; } =string.Empty;
    public string ApproverUserName { get; set; } = string.Empty;
    public string ApproverMinioUrl { get; set; } = string.Empty;
    public int StatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime ApprovedAt { get; set; }
}
public class SessionBookingList
{
    public int SessionId { get; set; }
    public string SessionName { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
public class EquipmentBookingList
{
    public int EquipmentId { get; set; }
    public string EquipmentName { get; set; } = string.Empty;
}