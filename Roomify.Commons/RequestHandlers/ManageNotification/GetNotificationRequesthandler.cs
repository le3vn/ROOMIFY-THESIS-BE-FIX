using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Roomify.Contracts.RequestModels.ManageNotification;
using Roomify.Contracts.ResponseModels.ManageNotification;
using Roomify.Entities;

namespace Roomify.Commons.RequestHandlers.ManageNotification
{
    public class GetNotificationRequestHandler : IRequestHandler<GetNotificationRequestModel, GetNotificationResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public GetNotificationRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<GetNotificationResponseModel> Handle(GetNotificationRequestModel request, CancellationToken cancellationToken)
        {
            // Validate that the UserId is valid
            if (request.UserId == null)
            {
                return new GetNotificationResponseModel
                {
                    TotalData = 0,
                    notifications = new List<NotificationModel>()
                };
            }

            // Fetch notifications for the given UserId
            var notificationsQuery = _db.Notifications
                                        .Where(n => n.UserId == request.UserId) // Filter by UserId
                                        .OrderByDescending(n => n.CreatedAt); // Order by the creation date (newest first)

            // Pagination logic (if required)
            var totalNotifications = await notificationsQuery.CountAsync(cancellationToken);

            var notifications = await notificationsQuery
                .Skip((request.Page - 1) * request.PageSize) // Apply pagination
                .Take(request.PageSize)  // Take the requested number of notifications
                .Select(n => new NotificationModel
                {
                    NotificationId = n.NotificationId,
                    Subject = n.Subject,
                    Message = n.Message,
                    ReadAt = n.ReadAt.HasValue ? n.ReadAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                    CreatedAt = n.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"), // Format the date as string
                    CreatedBy = n.CreatedBy
                })
                .ToListAsync(cancellationToken);

            // Return the response model with the notifications and total count
            return new GetNotificationResponseModel
            {
                TotalData = totalNotifications,
                notifications = notifications
            };
        }
    }
}
