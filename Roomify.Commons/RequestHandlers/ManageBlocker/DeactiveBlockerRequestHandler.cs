using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Roomify.Contracts.RequestModels.ManageBlocker;
using Roomify.Contracts.ResponseModels.ManageBlocker;
using Roomify.Entities;

namespace Roomify.Commons.RequestHandlers.ManageBlocker
{
    public class DeactiveBlockerRequestHandler : IRequestHandler<DeactiveBlockerRequestModel, DeactiveBlockerResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public DeactiveBlockerRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<DeactiveBlockerResponseModel> Handle(DeactiveBlockerRequestModel request, CancellationToken cancellationToken)
        {
            var blocker = await _db.Blockers
                                    .FirstOrDefaultAsync(b => b.BlockerId == request.BlockerId, cancellationToken);
            
            if (blocker == null)
            {
                return new DeactiveBlockerResponseModel
                {
                    Success = "false",
                    Message = $"Blocker with ID {request.BlockerId} not found."
                };
            }

            string actionMessage;
            string notificationMessage;

            if(blocker.IsActive == true)
            {
                // Deactivate the blocker
                blocker.IsActive = false;
                actionMessage = "deactivated";
                notificationMessage = $"{blocker.Name} blocker has been lifted and is no longer active from {blocker.StartDate.ToShortDateString()} to {blocker.EndDate.ToShortDateString()}.";   
            }
            else
            {
                // Activate the blocker
                blocker.IsActive = true;
                actionMessage = "activated";
                notificationMessage = $"{blocker.Name} blocker has been reactivated from {blocker.StartDate.ToShortDateString()} to {blocker.EndDate.ToShortDateString()}.";
            }

            // Update blocker status
            _db.Update(blocker);
            await _db.SaveChangesAsync(cancellationToken);

            // Send notifications about the blocker status change
            await SendNotificationToAllUsers(blocker, notificationMessage, cancellationToken);

            return new DeactiveBlockerResponseModel
            {
                Success = "true",
                Message = $"Blocker with ID {request.BlockerId} has been successfully {actionMessage}."
            };
        }

        private async Task SendNotificationToAllUsers(Blocker blocker, string notificationMessage, CancellationToken cancellationToken)
        {
            // Fetch all users
            var users = await _db.Users.ToListAsync(cancellationToken);

            // Create notification for all users
            foreach (var user in users)
            {
                var notification = new Notification
                {
                    UserId = user.Id,
                    Subject = $"{blocker.Name}",
                    Message = notificationMessage,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = "SYSTEM"  // This can be any string to indicate who created the notification
                };

                // Add notification to the database
                await _db.Notifications.AddAsync(notification, cancellationToken);
            }

            // Save all notifications to the database
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
