using MediatR;
using Microsoft.EntityFrameworkCore;
using Roomify.Contracts.RequestModels.ManageNotification;
using Roomify.Contracts.ResponseModels.ManageNotification;
using Roomify.Entities;

namespace Accelist.PDP.Commons.RequestHandlers.Notification
{
	public class DeleteAllNotificationRequestHandler : IRequestHandler<DeleteAllNotificationRequestModel, DeleteAllNotificationResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public DeleteAllNotificationRequestHandler(ApplicationDbContext db)
		{
            _db = db;
		}
        public async Task<DeleteAllNotificationResponseModel> Handle(DeleteAllNotificationRequestModel request, CancellationToken cancellationToken)
        {
            if (request.Id != null)
            {
                foreach (var delete in request.Id)
                {
                    var notifToDelete = await _db.Notifications.Where(n => n.NotificationId == delete).FirstOrDefaultAsync(cancellationToken);
                    _db.Notifications.Remove(notifToDelete);

                }
            }

            await _db.SaveChangesAsync(cancellationToken);

            return new DeleteAllNotificationResponseModel
            {
                Success = "Notifications deleted successfully."
            };

        }
    }
}

