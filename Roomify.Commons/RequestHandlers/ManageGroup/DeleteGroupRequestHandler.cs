using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Roomify.Contracts.RequestModels.ManageGroup;
using Roomify.Contracts.ResponseModels.ManageGroup;
using Roomify.Entities;

namespace Roomify.Commons.RequestHandlers.ManageGroup
{
    public class DeleteGroupRequestHandler : IRequestHandler<DeleteGroupRequestModel, DeleteGroupResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public DeleteGroupRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<DeleteGroupResponseModel> Handle(DeleteGroupRequestModel request, CancellationToken cancellationToken)
        {
            var group = await _db.RoomGroups.FindAsync(request.GroupId);

            if (group == null)
            {
                return new DeleteGroupResponseModel
                {
                    Success = "false",
                    Message = "Room Group not found."
                };
            }

            _db.RoomGroups.Remove(group);
            await _db.SaveChangesAsync(cancellationToken);

            return new DeleteGroupResponseModel
            {
                Success = "true",
                Message = "Room deleted successfully."
            };
        }
    }
}
