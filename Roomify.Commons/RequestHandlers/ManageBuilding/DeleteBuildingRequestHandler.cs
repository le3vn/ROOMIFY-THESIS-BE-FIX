using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Roomify.Contracts.RequestModels.ManageBuilding;
using Roomify.Contracts.RequestModels.ManageRoom;
using Roomify.Contracts.ResponseModels.ManageBuilding;
using Roomify.Contracts.ResponseModels.ManageRoom;
using Roomify.Entities;

namespace Roomify.Commons.RequestHandlers.ManageRoom
{
    public class DeleteBuildingRequestHandler : IRequestHandler<DeleteBuildingRequestModel, DeleteBuildingResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public DeleteBuildingRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<DeleteBuildingResponseModel> Handle(DeleteBuildingRequestModel request, CancellationToken cancellationToken)
        {
            var building = await _db.Buildings.FindAsync(request.BuildingId);

            if (building == null)
            {
                return new DeleteBuildingResponseModel
                {
                    Success = "false",
                    Message = "Room not found."
                };
            }

            _db.Buildings.Remove(building);
            await _db.SaveChangesAsync(cancellationToken);

            return new DeleteBuildingResponseModel
            {
                Success = "true",
                Message = "Room deleted successfully."
            };
        }
    }
}
