using Roomify.Contracts.ResponseModels.ManageBuilding;
using Roomify.Contracts.RequestModels.ManageBuilding;
using Roomify.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Roomify.Commons.Constants;
using Roomify.Commons.Services;
using Polly.Bulkhead;

namespace Roomify.Commons.RequestHandlers.ManageBuilding
{
    public class UpdateBuildingRequestHandler : IRequestHandler<UpdateBuildingRequestModel, UpdateBuildingResponseModel>
    {
        private readonly ApplicationDbContext _db;
        private readonly IStorageService _storageService;

        public UpdateBuildingRequestHandler(ApplicationDbContext db, IStorageService storageService)
        {
            _db = db;
            _storageService = storageService;
        }

        public async Task<UpdateBuildingResponseModel> Handle(UpdateBuildingRequestModel request, CancellationToken cancellationToken)
        {
            var building = await _db.Buildings.Where(Q => Q.BuildingId == request.BuildingId).FirstOrDefaultAsync(cancellationToken);
            if (building == null) return new UpdateBuildingResponseModel();
            var blobId = building.BlobId;
            var dateNow = DateTime.UtcNow;
            if (request.BuildingPicture != null)
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
                    FileName = request.BuildingPicture.FileName,
                    FilePath = $"{BlobPath.BuildingImage}/{request.BuildingPicture.FileName}",
                    ContentType = request.BuildingPicture.ContentType,
                    CreatedAt = dateNow,
                    CreatedBy = "Admin"
                };

                using (var stream = request.BuildingPicture.OpenReadStream())
                {
                    await _storageService.UploadFileAsync(newBlob.FilePath, stream);
                }

                _db.Blobs.Add(newBlob);
            }

            building.Name = request.Name;
            building.BlobId = blobId;
            building.UpdatedAt = DateTimeOffset.UtcNow;
            building.UpdatedBy = "Admin";
            _db.Update(building);

            await _db.SaveChangesAsync(cancellationToken);

            return new UpdateBuildingResponseModel { 
                Success = "Success",
                Message = "Succeed Updating Building"

            };        
        }

    }
}
