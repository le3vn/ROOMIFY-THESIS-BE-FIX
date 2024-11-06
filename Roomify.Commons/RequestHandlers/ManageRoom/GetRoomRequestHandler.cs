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
    public class GetRoomRequestHandler : IRequestHandler<GetRoomRequestModel, GetRoomResponseModel>
    {
        private readonly ApplicationDbContext _db;
        private readonly IStorageService _storageService;

        public GetRoomRequestHandler(ApplicationDbContext db, IStorageService storageService)
        {
            _db = db;
            _storageService = storageService;
        }

        public async Task<GetRoomResponseModel> Handle(GetRoomRequestModel request, CancellationToken cancellationToken)
        {
            var query = _db.Rooms.AsQueryable();

            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(r => r.Name.Contains(request.Search));
            }

            if (request.BuildingId > 0)
            {
                query = query.Where(r => r.BuildingId == request.BuildingId);
            }

            if (request.RoomTypeId > 0)
            {
                query = query.Where(r => r.RoomType == request.RoomTypeId);
            }

            if (request.Capacity > 0)
            {
                query = query.Where(r => r.Capacity >= request.Capacity);
            }

            query = request.SortOrder.ToLower() == "desc" 
                ? query.OrderByDescending(r => r.Name) 
                : query.OrderBy(r => r.Name);

            var rooms = await query.ToListAsync(cancellationToken);
            var roomModels = new List<GetRoomModel>();

            foreach (var room in rooms)
            {
                var roomModel = new GetRoomModel
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

            return new GetRoomResponseModel
            {
                RoomList = roomModels,
                TotalData = roomModels.Count
            };
        }
    }
}
