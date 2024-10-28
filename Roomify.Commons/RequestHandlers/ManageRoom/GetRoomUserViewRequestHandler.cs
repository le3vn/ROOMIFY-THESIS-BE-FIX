using Roomify.Contracts.ResponseModels.ManageRoom;
using Roomify.Contracts.RequestModels.ManageRoom;
using Roomify.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Roomify.Commons.RequestHandlers.ManageRoom
{
    public class GetRoomUserViewRequestHandler : IRequestHandler<GetRoomUserViewRequestModel, GetRoomUserViewResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public GetRoomUserViewRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<GetRoomUserViewResponseModel> Handle(GetRoomUserViewRequestModel request, CancellationToken cancellationToken)
        {
            // Start with all rooms
            var query = _db.Rooms.AsQueryable();

            // Filter by BuildingId
            if (request.BuildingId > 0)
            {
                query = query.Where(r => r.BuildingId == request.BuildingId);
            }

            // Filter by UserRole
            if (request.UserRole == "Student")
            {
                query = query.Where(r => r.RoomType == 1); // Assuming RoomType 1 is for students
            }

            // Determine the date to check availability
            var dateToBook = request.DateToBook;

            // Get all rooms that are booked on the requested date
            var bookedRoomIds = await _db.Schedules
                .Where(s => s.Date == dateToBook)
                .GroupBy(s => s.RoomId)
                .Where(g => g.Count() >= 6) // Rooms fully booked
                .Select(g => g.Key)
                .ToListAsync(cancellationToken);

            // Filter rooms based on availability
            if (request.IsAvailable)
            {
                query = query.Where(r => !bookedRoomIds.Contains(r.RoomId)); // Only available rooms
            }
            else
            {
                query = query.Where(r => bookedRoomIds.Contains(r.RoomId)); // Only booked rooms
            }

            // Select the relevant data
            var rooms = await query.Select(r => new GetRoomModels
            {
                RoomId = r.RoomId,
                Name = r.Name,
                RoomType = _db.RoomTypes.Where(rt => rt.RoomTypeId == r.RoomType).Select(rt => rt.Name).FirstOrDefault() ?? "Unknown",
                Building = _db.Buildings.Where(b => b.BuildingId == r.BuildingId).Select(b => b.Name).FirstOrDefault() ?? "Unknown",
                Description = r.Description,
                Capacity = r.Capacity,
                CreatedAt = r.CreatedAt,
                CreatedBy = r.CreatedBy,
                UpdatedAt = r.UpdatedAt,
                UpdatedBy = r.UpdatedBy
            }).ToListAsync(cancellationToken);

            return new GetRoomUserViewResponseModel
            {
                RoomList = rooms,
                TotalData = rooms.Count
            };
        }
    }
}
