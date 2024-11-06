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
    public class GetRoomUserViewRequestHandler : IRequestHandler<GetRoomUserViewRequestModel, GetRoomUserViewResponseModel>
    {
        private readonly ApplicationDbContext _db;
        private readonly IStorageService _storageService;

        public GetRoomUserViewRequestHandler(ApplicationDbContext db, IStorageService storageService)
        {
            _db = db;
            _storageService = storageService;
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
                .Where(g => g.Count() >= 6)
                .Select(g => g.Key)
                .ToListAsync(cancellationToken);

            if (request.IsAvailable)
            {
                query = query.Where(r => !bookedRoomIds.Contains(r.RoomId));
            }
            else
            {
                query = query.Where(r => bookedRoomIds.Contains(r.RoomId)); // Only booked rooms
            }

            // Select the relevant data
            var rooms = await query.ToListAsync(cancellationToken);
            var roomModels = new List<GetRoomModels>();

            foreach (var room in rooms)
            {
                var roomModel = new GetRoomModels
                {
                    RoomId = room.RoomId,
                    Name = room.Name,
                    RoomType = _db.RoomTypes.Where(rt => rt.RoomTypeId == room.RoomType).Select(rt => rt.Name).FirstOrDefault() ?? "Unknown",
                    Building = _db.Buildings.Where(b => b.BuildingId == room.BuildingId).Select(b => b.Name).FirstOrDefault() ?? "Unknown",
                    Description = room.Description,
                    Capacity = room.Capacity,
                    CreatedAt = room.CreatedAt,
                    CreatedBy = room.CreatedBy,
                    UpdatedAt = room.UpdatedAt,
                    UpdatedBy = room.UpdatedBy
                };

                // Assuming there's a Blob associated with each room
                var blob = await _db.Blobs.FirstOrDefaultAsync(b => b.Id == room.BlobId, cancellationToken);
                if (blob != null && !string.IsNullOrEmpty(blob.FilePath))
                {
                    try
                    {
                        roomModel.MinioUrl = await _storageService.GetPresignedUrlReadAsync(blob.FilePath);
                    }
                    catch (Exception)
                    {
                        roomModel.MinioUrl = "Error generating URL"; // Handle error as needed
                    }
                }
                else
                {
                    roomModel.MinioUrl = ""; // Or set a default value if no blob
                }

                roomModels.Add(roomModel);
            }

            return new GetRoomUserViewResponseModel
            {
                RoomList = roomModels,
                TotalData = roomModels.Count
            };
        }
    }
}
