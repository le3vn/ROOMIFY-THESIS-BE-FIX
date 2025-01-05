using System;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Roomify.Commons.Constants;
using Roomify.Commons.Services;
using Roomify.Contracts.RequestModels.ManageBuilding;
using Roomify.Contracts.ResponseModels.ManageBuilding;
using Roomify.Entities;

namespace Roomify.Commons.RequestHandlers.ManageBuilding;

public class CreateBuildingRequestHandler : IRequestHandler<CreateBuildingRequestModel, CreateBuildingResponseModel>
    {
        private readonly ApplicationDbContext _db;
        private readonly IStorageService _storageService;

        public CreateBuildingRequestHandler(ApplicationDbContext db, IStorageService storageService)
        {
            _db = db;
            _storageService = storageService;
        }

        public async Task<CreateBuildingResponseModel> Handle(CreateBuildingRequestModel request, CancellationToken cancellationToken)
        {
            var blob = new Blob
            {
                Id = Guid.NewGuid(),
                FileName = request.BuildingPicture.FileName,
                FilePath = $"{BlobPath.BuildingImage}/{request.BuildingPicture.FileName}",
                ContentType = request.BuildingPicture.ContentType,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Admin"
            };

            using (var stream = request.BuildingPicture.OpenReadStream())
            {
                await _storageService.UploadFileAsync(blob.FilePath, stream);
            }

            _db.Blobs.Add(blob);
            await _db.SaveChangesAsync(cancellationToken);

            var building = new Building
            {
                Name = request.BuildingName,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = "Admin",
                UpdatedAt = DateTimeOffset.UtcNow,
                UpdatedBy = "Admin",
                BlobId = blob.Id
            };

            _db.Buildings.Add(building);
            await _db.SaveChangesAsync(cancellationToken);

            return new CreateBuildingResponseModel
            {
                BuildingId = building.BuildingId, 
                Success = "true",
                Message = "Room created successfully."
            };
        }
    }
