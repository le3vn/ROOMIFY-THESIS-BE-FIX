using Roomify.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Roomify.Commons.Services;
using Roomify.Contracts.RequestModels.ManageBuilding;
using Roomify.Contracts.ResponseModels.ManageBuilding;

namespace Roomify.Commons.RequestHandlers.ManageBuilding
{
    public class GetBuildingRequestHandler : IRequestHandler<GetBuildingRequestModel, GetBuildingResponseModel>
    {
        private readonly ApplicationDbContext _db;
        private readonly IStorageService _storageService;

        public GetBuildingRequestHandler(ApplicationDbContext db, IStorageService storageService)
        {
            _db = db;
            _storageService = storageService;
        }

        public async Task<GetBuildingResponseModel> Handle(GetBuildingRequestModel request, CancellationToken cancellationToken)
        {
            var query = _db.Buildings.AsQueryable();

            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(r => r.Name.Contains(request.Search));
            }

            var buildings = await query.ToListAsync(cancellationToken);
            var buildingModels = new List<BuildingModel>();

            foreach (var building in buildings)
            {
                var buildingModel = new BuildingModel
                {
                    BuildingId = building.BuildingId,
                    Name = building.Name,
                    CreatedAt = building.CreatedAt,
                    CreatedBy = building.CreatedBy,
                    UpdatedAt = building.UpdatedAt,
                    UpdatedBy = building.UpdatedBy
                };

                // Assuming there's a Blob associated with each room
                var blob = await _db.Blobs.FirstOrDefaultAsync(b => b.Id == building.BlobId, cancellationToken);
                if (blob != null && !string.IsNullOrEmpty(blob.FilePath))
                {
                    try
                    {
                        buildingModel.MinioUrl = await _storageService.GetPresignedUrlReadAsync(blob.FilePath);
                    }
                    catch (Exception)
                    {
                        buildingModel.MinioUrl = "Error generating URL"; // Handle error as needed
                    }
                }
                else
                {
                    buildingModel.MinioUrl = ""; // Or set a default value if no blob
                }

                buildingModels.Add(buildingModel);
            }

            return new GetBuildingResponseModel
            {
                BuildingList = buildingModels,
                TotalData = buildingModels.Count
            };
        }
    }
}
