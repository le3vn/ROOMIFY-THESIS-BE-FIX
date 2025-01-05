using System;

namespace Roomify.Contracts.ResponseModels.ManageRoom;

public class GetRoomScheduleResponseModel
{
    public List<GetRoomScheduleModel> ScheduleList { get; set; } = new List<GetRoomScheduleModel>();
}

public class GetRoomScheduleModel
{
    public DateOnly BookingDate { get; set; }
    public TimeOnly BookingTimeStart { get; set; }
    public TimeOnly BookingTimeEnd{ get; set; }
    public string EventName { get; set; } = "";
    public string PICName { get; set; } = "";
}
