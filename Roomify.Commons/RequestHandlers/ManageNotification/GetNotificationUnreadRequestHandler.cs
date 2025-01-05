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
    public class GetNotificationUnreadRequestHandler : IRequestHandler<GetNotificationUnreadRequestModel, GetNotificationUnreadResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public GetNotificationUnreadRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<GetNotificationUnreadResponseModel> Handle(GetNotificationUnreadRequestModel request, CancellationToken cancellationToken)
        {
            // Validate the userId
            if (string.IsNullOrEmpty(request.UserId))
            {
                return new GetNotificationUnreadResponseModel
                {
                    Total = 0  // If no userId is provided, return 0
                };
            }

            // Fetch the total number of unread notifications for the given userId
            var unreadNotificationsCount = await _db.Notifications
                .Where(n => n.UserId == request.UserId && n.ReadAt == null) // ReadAt == null indicates unread
                .CountAsync(cancellationToken);

            // Return the response with the total count of unread notifications
            return new GetNotificationUnreadResponseModel
            {
                Total = unreadNotificationsCount
            };
        }
    }
}
