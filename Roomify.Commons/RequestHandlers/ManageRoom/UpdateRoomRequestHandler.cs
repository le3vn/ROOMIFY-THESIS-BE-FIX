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
            var room = await _db.Rooms.Where(Q => Q.RoomId == request.RoomId).FirstOrDefaultAsync(cancellationToken);
            if (room == null) return new UpdateRoomResponseModel();
            var blobId = room.BlobId;
            var dateNow = DateTime.UtcNow;
            if (request.RoomPicture != null)
            {
                var deleteBlob = await _db.Blobs.Where(Q => Q.Id == blobId).FirstOrDefaultAsync(cancellationToken);
                if (deleteBlob != null)
                {
                    _db.Blobs.Remove(deleteBlob);
                }
                blobId = Guid.NewGuid();
                var newBlob = new Blob
                {
                    Id = blobId,
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
            }

            room.Name = request.Name;
            room.BuildingId = request.BuildingId;
            room.RoomType = request.RoomTypeId;
            room.Description = request.Description;
            room.Capacity = request.Capacity;
            room.BlobId = blobId;
            room.UpdatedAt = DateTimeOffset.UtcNow;
            room.UpdatedBy = "Admin";
            _db.Update(room);

            await _db.SaveChangesAsync(cancellationToken);

            return new UpdateRoomResponseModel { 
                Success = "Success",
                Message = "Succeed Updating Room"

            };        
        }

    }
}
