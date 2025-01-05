using Roomify.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Roomify.Commons.Services;
using Roomify.Contracts.RequestModels.ManageBuilding;
using Roomify.Contracts.ResponseModels.ManageBuilding;

namespace Roomify.Commons.RequestHandlers.ManageBuilding
{
    public class GetBuildingDetailRequestHandler : IRequestHandler<GetBuildingDetailRequestModel, GetBuildingDetailResponseModel>
    {
        private readonly ApplicationDbContext _db;
        private readonly IStorageService _storageService;

        public GetBuildingDetailRequestHandler(ApplicationDbContext db, IStorageService storageService)
        {
            _db = db;
            _storageService = storageService;
        }

        public async Task<GetBuildingDetailResponseModel> Handle(GetBuildingDetailRequestModel request, CancellationToken cancellationToken)
        {
            var building = await _db.Buildings
                .FirstOrDefaultAsync(r => r.BuildingId == request.BuildingId, cancellationToken) ?? throw new KeyNotFoundException("Room not found");

            var BuildingImage = await _db.Buildings
                .Include(u => u.Blob) // Ensure Blob navigation property is included
                .FirstOrDefaultAsync(u => u.BuildingId == building.BuildingId, cancellationToken); // Use user.Id

            string minioUrl = string.Empty;
            if (BuildingImage != null && BuildingImage.Blob != null && !string.IsNullOrEmpty(BuildingImage.Blob.FilePath))
            {
                try
                {
                    minioUrl = await _storageService.GetPresignedUrlReadAsync(BuildingImage.Blob.FilePath);
                }
                catch (Exception)
                {
                    minioUrl = "Error generating URL"; 
                }
            }
            return new GetBuildingDetailResponseModel
            {
                Name = building.Name, 
                MinioUrl = minioUrl,
                CreatedAt = building.CreatedAt,
                CreatedBy = building.CreatedBy,
                UpdatedAt = building.UpdatedAt,
                UpdatedBy = building.UpdatedBy
            };
        }
    }
}
