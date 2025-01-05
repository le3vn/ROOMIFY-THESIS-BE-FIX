using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageBooking;
using Roomify.Contracts.RequestModels.ManageBooking;
using Microsoft.EntityFrameworkCore;
using Roomify.Entities;

namespace Roomify.Application.Handlers.ManageBooking
{
    public class GetBookingToManageRequestHandler : IRequestHandler<GetBookingToManageRequestModel, GetBookingToManageResponseModel>
    {
        private readonly ApplicationDbContext _db;

        // Inject your DbContext through the constructor
        public GetBookingToManageRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<GetBookingToManageResponseModel> Handle(GetBookingToManageRequestModel request, CancellationToken cancellationToken)
        {
            var query = _db.Bookings.AsQueryable();

            // Apply filters based on optional request parameters

            // BuildingId filter: Only include bookings where RoomId belongs to a room with the same BuildingId
            if (request.BuildingId.HasValue)
            {
                query = query.Where(b => _db.Rooms
                                          .Where(r => r.RoomId == b.RoomId && r.BuildingId == request.BuildingId.Value)
                                          .Any());
            }

            if (request.RoomId.HasValue)
            {
                query = query.Where(b => b.RoomId == request.RoomId.Value);
            }

            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(b => b.BookingDescription.Contains(request.Search));
            }

            // Select the required fields from the bookings table
            var bookings = await query
                .Select(b => new
                {
                    b.Id,
                    b.UserId,
                    b.BookingDescription,
                    b.RoomId,
                    b.BookingDate,
                    b.StatusId,
                    b.IsCanceled
                }).Where(b => b.StatusId == 2 && !b.IsCanceled)
                .ToListAsync(cancellationToken);

            var response = new GetBookingToManageResponseModel();

            // Process each booking and populate the response
            foreach (var booking in bookings)
            {
                // Get room details
                var room = await _db.Rooms
                    .Where(r => r.RoomId == booking.RoomId)
                    .FirstOrDefaultAsync(cancellationToken);

                var user = await _db.Users
                    .Where(u => u.Id == booking.UserId)
                    .Select(u => u.GivenName)
                    .FirstOrDefaultAsync(cancellationToken);

                // Get status details
                var status = await _db.Statuses
                    .Where(s => s.StatusId == booking.StatusId)
                    .Select(s => s.Name)
                    .FirstOrDefaultAsync(cancellationToken);

                // Get sessions related to the booking
                var sessionIds = await _db.SessionBookeds
                    .Where(sb => sb.BookingId == booking.Id)
                    .Select(sb => sb.SessionId)
                    .ToListAsync(cancellationToken);

                var sessions = await _db.Sessions
                    .Where(s => sessionIds.Contains(s.SessionId))
                    .Select(s => new ManageSessionModel
                    {
                        SessionId = s.SessionId,
                        SessionName = s.Name
                    })
                    .ToListAsync(cancellationToken);


                // Create a ManageBookingModel object and add it to the response
                var manageBooking = new ManageBookingModel
                {
                    BookingId = booking.Id.ToString(),
                    BookingDescription = booking.BookingDescription,
                    RoomId = booking.RoomId,
                    RoomName = room?.Name ?? "",
                    BookingDate = booking.BookingDate.ToString("yyyy-MM-dd"), // Format as needed
                    StatusId = booking.StatusId,
                    StatusName = status ?? "",
                    manageSessions = sessions,
                    UserName = user ?? "",
                    BuildingId = room?.BuildingId ?? 0
                };

                response.manageBookings.Add(manageBooking);
            }

            return response;
        }
    }
}
