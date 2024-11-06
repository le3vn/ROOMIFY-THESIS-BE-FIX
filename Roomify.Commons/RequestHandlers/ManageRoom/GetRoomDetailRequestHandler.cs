using Roomify.Contracts.ResponseModels.ManageRoom;
using Roomify.Contracts.RequestModels.ManageRoom;
using Roomify.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Roomify.Commons.Services;

namespace Roomify.Commons.RequestHandlers.ManageRoom
{
    public class GetRoomDetailRequestHandler : IRequestHandler<GetRoomDetailRequestModel, GetRoomDetailResponseModel>
    {
        private readonly ApplicationDbContext _db;
        private readonly IStorageService _storageService;

        public GetRoomDetailRequestHandler(ApplicationDbContext db, IStorageService storageService)
        {
            _db = db;
            _storageService = storageService;
        }

        public async Task<GetRoomDetailResponseModel> Handle(GetRoomDetailRequestModel request, CancellationToken cancellationToken)
        {
            var room = await _db.Rooms
                .FirstOrDefaultAsync(r => r.RoomId == request.RoomId, cancellationToken) ?? throw new KeyNotFoundException("Room not found");

            var RoomImage = await _db.Rooms
                .Include(u => u.Blob) // Ensure Blob navigation property is included
                .FirstOrDefaultAsync(u => u.RoomId == room.RoomId, cancellationToken); // Use user.Id

            string minioUrl = string.Empty;
            if (RoomImage != null && RoomImage.Blob != null && !string.IsNullOrEmpty(RoomImage.Blob.FilePath))
            {
                try
                {
                    minioUrl = await _storageService.GetPresignedUrlReadAsync(RoomImage.Blob.FilePath);
                }
                catch (Exception)
                {
                    minioUrl = "Error generating URL"; 
                }
            }
            return new GetRoomDetailResponseModel
            {
                Name = room.Name, 
                RoomType = await _db.RoomTypes
                    .Where(rt => rt.RoomTypeId == room.RoomType)
                    .Select(rt => rt.Name)
                    .FirstOrDefaultAsync(cancellationToken) ?? "Unknown",
                Building = await _db.Buildings
                    .Where(b => b.BuildingId == room.BuildingId)
                    .Select(b => b.Name)
                    .FirstOrDefaultAsync(cancellationToken) ?? "Unknown",
                
                Capacity = room.Capacity,
                MinioUrl = minioUrl,
                Description = room.Description, 
                CreatedAt = room.CreatedAt,
                CreatedBy = room.CreatedBy,
                UpdatedAt = room.UpdatedAt,
                UpdatedBy = room.UpdatedBy
            };
        }
    }
}
