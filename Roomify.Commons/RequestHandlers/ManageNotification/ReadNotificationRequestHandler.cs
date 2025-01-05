using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Roomify.Contracts.RequestModels.ManageNotification;
using Roomify.Contracts.ResponseModels.ManageNotification;
using Roomify.Entities;

namespace Roomify.Commons.RequestHandlers.ManageNotification
{
    public class ReadNotificationRequestHandler : IRequestHandler<ReadNotificationRequestModel, ReadNotificationResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public ReadNotificationRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ReadNotificationResponseModel> Handle(ReadNotificationRequestModel request, CancellationToken cancellationToken)
        {
            var response = new ReadNotificationResponseModel();

            // Find the notification by its ID
            var notification = await _db.Notifications
                                        .FirstOrDefaultAsync(n => n.NotificationId == request.NotificationId, cancellationToken);

            // If the notification doesn't exist, return a failure message
            if (notification == null)
            {
                response.Success = "false";
                response.Message = $"Notification with ID {request.NotificationId} not found.";
                return response;
            }

            // Mark the notification as read by setting the ReadAt field to the current UTC time
            if (notification.ReadAt == null)
            {
                notification.ReadAt = DateTimeOffset.UtcNow;
                _db.Notifications.Update(notification);
                await _db.SaveChangesAsync(cancellationToken);

                response.Success = "true";
                response.Message = $"Notification {request.NotificationId} marked as read successfully.";
            }
            else
            {
                response.Success = "false";
                response.Message = "Notification is already marked as read.";
            }

            return response;
        }
    }
}
