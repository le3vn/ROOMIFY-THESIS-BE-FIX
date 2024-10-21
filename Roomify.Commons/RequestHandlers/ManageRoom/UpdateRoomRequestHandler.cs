using Roomify.Contracts.ResponseModels.ManageRoom;
using Roomify.Contracts.RequestModels.ManageRoom;
using Roomify.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Roomify.Commons.RequestHandlers.ManageRoom
{
    public class UpdateRoomRequestHandler : IRequestHandler<UpdateRoomRequestModel, UpdateRoomResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public UpdateRoomRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<UpdateRoomResponseModel> Handle(UpdateRoomRequestModel request, CancellationToken cancellationToken)
        {
            var room = await _db.Rooms.FindAsync(request.RoomId);

            if (room == null)
            {
                return new UpdateRoomResponseModel
                {
                    Success = "false",
                    Message = "Room not found."
                };
            }

            room.Name = request.Name;
            room.RoomType = request.RoomTypeId;
            room.Description = request.Description;
            room.BuildingId = request.BuildingId;
            room.Capacity = request.Capacity;
            room.UpdatedAt = DateTimeOffset.UtcNow;
            room.UpdatedBy = "Admin";

            _db.Rooms.Update(room);
            await _db.SaveChangesAsync(cancellationToken);

            return new UpdateRoomResponseModel
            {
                Success = "true",
                Message = "Room updated successfully."
            };
        }

    }
}
