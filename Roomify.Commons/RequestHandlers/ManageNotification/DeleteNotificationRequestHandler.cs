using MediatR;
using Microsoft.EntityFrameworkCore;
using Roomify.Contracts.RequestModels.ManageNotification;
using Roomify.Contracts.ResponseModels.ManageNotification;
using Roomify.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Roomify.Commons.RequestHandlers.ManageNotification
{
    public class DeleteNotificationRequestHandler : IRequestHandler<DeleteNotificationRequestModel, DeleteNotificationResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public DeleteNotificationRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<DeleteNotificationResponseModel> Handle(DeleteNotificationRequestModel request, CancellationToken cancellationToken)
        {
            var notification = await _db.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == request.NotificationId, cancellationToken);

            if (notification == null)
            {
                return new DeleteNotificationResponseModel
                {
                    Success = false,
                    Message = "Notification not found."
                };
            }

            // Delete the notification
            _db.Notifications.Remove(notification);
            await _db.SaveChangesAsync(cancellationToken);

            return new DeleteNotificationResponseModel
            {
                Success = true,
                Message = "Notification deleted successfully."
            };
        }
    }
}
