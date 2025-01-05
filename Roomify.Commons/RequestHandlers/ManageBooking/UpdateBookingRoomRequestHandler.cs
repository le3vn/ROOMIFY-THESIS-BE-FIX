using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Roomify.Contracts.RequestModels.ManageBooking;
using Roomify.Contracts.ResponseModels.ManageBooking;
using Roomify.Entities;
using Microsoft.EntityFrameworkCore;

namespace Roomify.Commons.RequestHandlers.ManageBooking
{
    public class UpdateBookingRoomRequestHandler : IRequestHandler<UpdateBookingRoomRequestModel, UpdateBookingRoomResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public UpdateBookingRoomRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<UpdateBookingRoomResponseModel> Handle(UpdateBookingRoomRequestModel request, CancellationToken cancellationToken)
        {
            var response = new UpdateBookingRoomResponseModel();

            try
            {
                // Fetch the booking based on BookingId
                var booking = await _db.Bookings
                    .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

                if (booking == null)
                {
                    response.Success = "false";
                    response.Message = "Booking not found.";
                    return response;
                }

                // Update the RoomId for the booking
                booking.RoomId = request.RoomId;

                // Fetch the related schedules based on BookingId
                var schedules = await _db.Schedules
                    .Where(s => s.BookingId == request.BookingId)
                    .ToListAsync(cancellationToken);

                // Update the RoomId for all schedules related to the booking
                foreach (var schedule in schedules)
                {
                    schedule.RoomId = request.RoomId;
                }

                // Save changes to both Bookings and Schedules
                await _db.SaveChangesAsync(cancellationToken);

                var room = await _db.Rooms.Where(r => r.RoomId == booking.RoomId).Select(r => r.Name).FirstOrDefaultAsync();

                // Send a notification to the user that the booking has been moved
                await AddNotificationToDb(booking.UserId, 
                    "Booking Moved", 
                    $"Your booking has been moved to a new room: {room}.", 
                    "SYSTEM");

                // Return success response
                response.Success = "true";
                response.Message = "Room updated successfully for the booking and its schedules.";
            }
            catch (Exception ex)
            {
                // Handle exceptions (log if needed, but for now we return the error message)
                response.Success = "false";
                response.Message = $"An error occurred: {ex.Message}";
            }

            return response;
        }

        // Helper method for adding notifications to the database
        private async Task AddNotificationToDb(string userId, string subject, string message, string createdBy)
        {
            var notification = new Notification
            {
                UserId = userId,
                Subject = subject,
                Message = message,
                ReadAt = DateTimeOffset.MinValue,  // Set this to a default value indicating the notification is unread.
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = createdBy
            };

            await _db.Notifications.AddAsync(notification);
            await _db.SaveChangesAsync();
        }
    }
}
