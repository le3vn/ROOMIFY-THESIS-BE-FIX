using Roomify.Contracts.ResponseModels.ManageRoom;
using Roomify.Contracts.RequestModels.ManageRoom;
using Roomify.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Roomify.Commons.Constants;
using Roomify.Commons.Services;

namespace Roomify.Commons.RequestHandlers.ManageRoom
{
    public class UpdateRoomRequestHandler : IRequestHandler<UpdateRoomRequestModel, UpdateRoomResponseModel>
    {
        private readonly ApplicationDbContext _db;
        private readonly IStorageService _storageService;

        public UpdateRoomRequestHandler(ApplicationDbContext db, IStorageService storageService)
        {
            _db = db;
            _storageService = storageService;
        }

        public async Task<UpdateRoomResponseModel> Handle(UpdateRoomRequestModel request, CancellationToken cancellationToken)
        {
            // Fetch the room and related RoomGroups
            var room = await _db.Rooms
                .Include(r => r.RoomGroups)
                .Where(Q => Q.RoomId == request.RoomId)
                .FirstOrDefaultAsync(cancellationToken);

            if (room == null)
            {
                return new UpdateRoomResponseModel
                {
                    Success = "false",
                    Message = "Room not found"
                };
            }

            // Check if the RoomGroupId exists
            var roomGroup = await _db.RoomGroups
                .Where(rg => rg.RoomGroupId == request.RoomGroupId)
                .FirstOrDefaultAsync(cancellationToken);

            if (roomGroup == null)
            {
                return new UpdateRoomResponseModel
                {
                    Success = "false",
                    Message = "RoomGroup not found"
                };
            }

            var dateNow = DateTime.UtcNow;

            // Handle RoomPicture update
            if (request.RoomPicture != null)
            {
                // Delete existing blob if it exists
                var deleteBlob = await _db.Blobs.Where(Q => Q.Id == room.BlobId).FirstOrDefaultAsync(cancellationToken);
                if (deleteBlob != null)
                {
                    _db.Blobs.Remove(deleteBlob);
                }

                // Create a new blob for the updated image
                var newBlob = new Blob
                {
                    Id = Guid.NewGuid(),
                    FileName = request.RoomPicture.FileName,
                    FilePath = $"{BlobPath.RoomsImage}/{request.RoomPicture.FileName}",
                    ContentType = request.RoomPicture.ContentType,
                    CreatedAt = dateNow,
                    CreatedBy = "Admin"
                };

                using (var stream = request.RoomPicture.OpenReadStream())
                {
                    await _storageService.UploadFileAsync(newBlob.FilePath, stream);
                }

                _db.Blobs.Add(newBlob);
                room.BlobId = newBlob.Id; // Update room with the new blobId
            }
            // Update Room details
            room.Name = request.Name;
            room.BuildingId = request.BuildingId;
            room.RoomType = request.RoomTypeId;
            room.Description = request.Description;
            room.Capacity = request.Capacity;
            room.UpdatedAt = DateTimeOffset.UtcNow;
            room.UpdatedBy = "Admin";
            room.RoomGroupId = request.RoomGroupId;  // Set the new or existing RoomGroupId

            // Save changes to the database
            await _db.SaveChangesAsync(cancellationToken);

            return new UpdateRoomResponseModel
            {
                Success = "true",
                Message = "Successfully updated room"
            };
        }
    }
}

