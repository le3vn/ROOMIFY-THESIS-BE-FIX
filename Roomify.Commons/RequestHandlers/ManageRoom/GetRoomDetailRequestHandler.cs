using Roomify.Contracts.ResponseModels.ManageRoom;
using Roomify.Contracts.RequestModels.ManageRoom;
using Roomify.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Roomify.Commons.RequestHandlers.ManageRoom
{
    public class GetRoomDetailRequestHandler : IRequestHandler<GetRoomDetailRequestModel, GetRoomDetailResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public GetRoomDetailRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<GetRoomDetailResponseModel> Handle(GetRoomDetailRequestModel request, CancellationToken cancellationToken)
        {
            var room = await _db.Rooms
                .FirstOrDefaultAsync(r => r.RoomId == request.RoomId, cancellationToken) ?? throw new KeyNotFoundException("Room not found");
            return new GetRoomDetailResponseModel
            {
                Name = room.Name, 
                RoomType = await _db.RoomTypes
                    .Where(rt => rt.RoomTypeId == room.RoomType)
                    .Select(rt => rt.Name)
                    .FirstOrDefaultAsync(cancellationToken) ?? "Unknown",
                Building = await _db.Buildings
                    .Where(b => b.BuildingId == room.BuildingId)
                    .Select(b => b.Name)
                    .FirstOrDefaultAsync(cancellationToken) ?? "Unknown",
                Capacity = room.Capacity,
                Description = room.Description, 
                CreatedAt = room.CreatedAt,
                CreatedBy = room.CreatedBy,
                UpdatedAt = room.UpdatedAt,
                UpdatedBy = room.UpdatedBy
            };
        }
    }
}
