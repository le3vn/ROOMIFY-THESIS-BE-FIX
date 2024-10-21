using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Roomify.Contracts.RequestModels.ManageRoom;
using Roomify.Contracts.ResponseModels.ManageRoom;
using Roomify.Entities;

namespace Roomify.Commons.RequestHandlers.ManageRoom
{
    public class DeleteRoomRequestHandler : IRequestHandler<DeleteRoomRequestModel, DeleteRoomResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public DeleteRoomRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<DeleteRoomResponseModel> Handle(DeleteRoomRequestModel request, CancellationToken cancellationToken)
        {
            var room = await _db.Rooms.FindAsync(request.RoomId);

            if (room == null)
            {
                return new DeleteRoomResponseModel
                {
                    Success = "false",
                    Message = "Room not found."
                };
            }

            _db.Rooms.Remove(room);
            await _db.SaveChangesAsync(cancellationToken);

            return new DeleteRoomResponseModel
            {
                Success = "true",
                Message = "Room deleted successfully."
            };
        }
    }
}
