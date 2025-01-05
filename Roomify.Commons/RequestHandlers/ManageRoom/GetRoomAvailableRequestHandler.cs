using Roomify.Contracts.ResponseModels.ManageRoom;
using Roomify.Contracts.RequestModels.ManageRoom;
using Roomify.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Roomify.Commons.Services;

namespace Roomify.Commons.RequestHandlers.ManageRoom
{
    public class GetRoomAvailableRequestHandler : IRequestHandler<GetRoomAvailableRequestModel, GetRoomAvailableResponseModel>
    {
        private readonly ApplicationDbContext _db;
        private readonly IStorageService _storageService;

        public GetRoomAvailableRequestHandler(ApplicationDbContext db, IStorageService storageService)
        {
            _db = db;
            _storageService = storageService;
        }

        public async Task<GetRoomAvailableResponseModel> Handle(GetRoomAvailableRequestModel request, CancellationToken cancellationToken)
        {
            // Step 1: Get the BookingId details (RoomId and Date)
            var booking = await _db.Bookings
                .Where(b => b.Id == request.BookingId)
                .Select(b => new { b.RoomId, b.BookingDate, b.UserId })
                .FirstOrDefaultAsync(cancellationToken);

            if (booking == null)
                return new GetRoomAvailableResponseModel
                {
                    RoomAvailables = new List<RoomAvailableModel>(),
                    TotalData = 0
                };

            var roomId = booking.RoomId;
            var bookingDate = booking.BookingDate;

            // Step 2: Get the session IDs associated with this BookingId
            var sessionIds = await _db.SessionBookeds
                .Where(sb => sb.BookingId == request.BookingId)
                .Select(sb => sb.SessionId)
                .ToListAsync(cancellationToken);

            // Step 3: Get rooms from the Schedules table that are booked for the sessionId and roomId
            var bookedRoomSessions = await _db.Schedules
                .Where(s => sessionIds.Contains(s.SessionId) && s.RoomId == roomId && s.Date == bookingDate)
                .ToListAsync(cancellationToken);

            // Step 4: Filter available rooms
            var unavailableRoomIds = bookedRoomSessions.Select(s => s.RoomId).ToList();

            // Step 5: Filter rooms based on availability
            var availableRoomsQuery = _db.Rooms.AsQueryable();

            // If the user is a student, show only rooms with roomType = 1
            var userRole = await _db.Roles
                .Where(u => u.Id == booking.UserId)
                .Select(u => u.Name)
                .FirstOrDefaultAsync(cancellationToken);

            if (userRole == "Student")
            {
                availableRoomsQuery = availableRoomsQuery.Where(r => r.RoomType == 1);
            }

            // Exclude the rooms that are already booked for the requested session and date
            var availableRooms = await availableRoomsQuery
                .Where(r => !unavailableRoomIds.Contains(r.RoomId))
                .Select(r => new RoomAvailableModel
                {
                    RoomId = r.RoomId,
                    RoomName = r.Name
                })
                .ToListAsync(cancellationToken);

            return new GetRoomAvailableResponseModel
            {
                RoomAvailables = availableRooms,
                TotalData = availableRooms.Count
            };
        }
    }
}
