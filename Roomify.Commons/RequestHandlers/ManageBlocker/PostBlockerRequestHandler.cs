using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Roomify.Contracts.RequestModels.ManageBlocker;
using Roomify.Contracts.ResponseModels.ManageBlocker;
using Roomify.Entities;

namespace Roomify.Commons.RequestHandlers.ManageBlocker
{
    public class PostBlockerRequestHandler : IRequestHandler<PostBlockerRequestModel, PostBlockerResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public PostBlockerRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PostBlockerResponseModel> Handle(PostBlockerRequestModel request, CancellationToken cancellationToken)
        {
            // Validate if the new blocker overlaps with any existing blocker
            var overlappingBlocker = await _db.Blockers
                .Where(b => b.IsActive)  // Only consider active blockers
                .AnyAsync(b =>
                    (request.StartDate < b.EndDate && request.EndDate > b.StartDate), // Check for overlap
                    cancellationToken);

            if (overlappingBlocker)
            {
                // If there is an overlap, return an error message
                return new PostBlockerResponseModel
                {
                    Success = "false",
                    Message = "The date range overlaps with an existing blocker."
                };
            }

            // Create a new Blocker entity based on the request
            var blocker = new Blocker
            {
                Name = request.BlockerName,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsActive = true
            };

            // Add the new blocker to the database
            _db.Blockers.Add(blocker);

            try
            {
                // Save changes asynchronously
                await _db.SaveChangesAsync(cancellationToken);
                
                // After successfully adding the blocker, send notifications to all users
                await SendNotificationToAllUsers(blocker, cancellationToken, request);

                // Return a success response
                return new PostBlockerResponseModel
                {
                    Success = "true",
                    Message = "Blocker successfully added."
                };
            }
            catch (Exception ex)
            {
                // Handle errors and return failure message
                return new PostBlockerResponseModel
                {
                    Success = "false",
                    Message = $"Error occurred while adding blocker: {ex.Message}"
                };
            }
        }

        // Method to send notifications to all users about the new blocker
        private async Task SendNotificationToAllUsers(Blocker blocker, CancellationToken cancellationToken, PostBlockerRequestModel request)
        {
            // Fetch all users
            var users = await _db.Users.ToListAsync(cancellationToken);

            // Create notification for all users
            foreach (var user in users)
            {
                var notification = new Notification
                {
                    UserId = user.Id,
                    Subject = $"{request.BlockerName}",
                    Message = $"Booking has been blocked from {blocker.StartDate.ToShortDateString()} to {blocker.EndDate.ToShortDateString()}.",
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = "SYSTEM"  // This can be any string to indicate who created the notification
                };

                // Add notification to the database
                await _db.Notifications.AddAsync(notification, cancellationToken);
            }

            // Save all notifications
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
