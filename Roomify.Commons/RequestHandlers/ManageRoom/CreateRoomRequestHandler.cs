using Roomify.Contracts.ResponseModels.ManageRoom;
using Roomify.Contracts.RequestModels.ManageRoom;
using Roomify.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using System.Threading.Tasks;
using Roomify.Commons.Constants;
using Roomify.Commons.Services;

namespace Roomify.Commons.RequestHandlers.ManageRoom
{
    public class CreateRoomRequestHandler : IRequestHandler<CreateRoomRequestModel, CreateRoomResponseModel>
    {
        private readonly ApplicationDbContext _db;
        private readonly IValidator<CreateRoomRequestModel> _validator;
        public readonly IStorageService _storageService;

        public CreateRoomRequestHandler(ApplicationDbContext db, IValidator<CreateRoomRequestModel> validator, IStorageService storageService)
        {
            _db = db;
            _validator = validator;
            _storageService = storageService;
        }

        public async Task<CreateRoomResponseModel> Handle(CreateRoomRequestModel request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new CreateRoomResponseModel
                {
                    Success = "false",
                    Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))
                };
            }
                var blob = new Blob
                {
                    Id = Guid.NewGuid(),
                    FileName = request.RoomPicture.FileName,
                    FilePath = $"{BlobPath.RoomsImage}/{request.RoomPicture.FileName}",
                    ContentType = request.RoomPicture.ContentType,
                    CreatedAt = DateTime.UtcNow
                };

                //UploadFileAsync requires Stream parameter
                using (var stream = new MemoryStream())
                {
                    await request.RoomPicture.CopyToAsync(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    await _storageService.UploadFileAsync(blob.FilePath, stream);
                }

                _db.Blobs.Add(blob);
                await _db.SaveChangesAsync(cancellationToken);

            var room = new Room
            {
                Name = request.RoomName,
                RoomType = request.RoomTypeId,
                Description = request.Description,
                BuildingId = request.BuildingId,
                Capacity = request.Capacity,
                BlobId =  blob.Id,
                RoomGroupId = request.RoomGroupId,
                CreatedBy = "Admin",
                UpdatedBy = "Admin",
            };

            _db.Rooms.Add(room);
            await _db.SaveChangesAsync(cancellationToken);

            return new CreateRoomResponseModel
            {
                RoomId = room.RoomId, 
                Success = "true",
                Message = "Room created successfully."
            };
        }
    }
}
