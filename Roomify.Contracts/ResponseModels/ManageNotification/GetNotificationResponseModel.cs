using System;

namespace Roomify.Contracts.ResponseModels.ManageNotification;

public class GetNotificationResponseModel
{
    public List<NotificationModel> notifications { get; set; } = new List<NotificationModel>();
    public int TotalData { get; set; }
}

public class NotificationModel{
    public int NotificationId { get; set; }
    public string Subject { get; set; } ="";
    public string Message { get; set; } ="";
    public string ReadAt { get; set; } ="";

    public string CreatedAt { get; set; } ="";
    public string CreatedBy { get; set; } ="";
}