using Roomify.Contracts.ResponseModels.ManageRoom;
using Roomify.Contracts.RequestModels.ManageRoom;
using Roomify.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Roomify.Commons.RequestHandlers.ManageRoom
{
    public class GetRoomRequestHandler : IRequestHandler<GetRoomRequestModel, GetRoomResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public GetRoomRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<GetRoomResponseModel> Handle(GetRoomRequestModel request, CancellationToken cancellationToken)
        {
            var query = _db.Rooms.AsQueryable();

            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(r => r.Name.Contains(request.Search));
            }

            if (request.BuildingId > 0)
            {
                query = query.Where(r => r.BuildingId == request.BuildingId);
            }

            if (request.RoomTypeId > 0)
            {
                query = query.Where(r => r.RoomType == request.RoomTypeId);
            }

            if (request.Capacity > 0)
            {
                query = query.Where(r => r.Capacity >= request.Capacity);
            }

            query = request.SortOrder.ToLower() == "desc" 
                ? query.OrderByDescending(r => r.Name) 
                : query.OrderBy(r => r.Name);

            var rooms = await query.Select(r => new GetRoomModel
            {
                RoomId = r.RoomId,
                Name = r.Name,
                RoomType = _db.RoomTypes.Where(rt => rt.RoomTypeId == r.RoomType).Select(rt => rt.Name).FirstOrDefault() ?? "Unknown",
                Building = _db.Buildings.Where(b => b.BuildingId == r.BuildingId).Select(b => b.Name).FirstOrDefault() ?? "Unknown",
                Description = r.Description,
                Capacity = r.Capacity,
                CreatedAt = r.CreatedAt,
                CreatedBy = r.CreatedBy,
                UpdatedAt = r.UpdatedAt,
                UpdatedBy = r.UpdatedBy
            }).ToListAsync(cancellationToken);

            return new GetRoomResponseModel
            {
                RoomList = rooms,
                TotalData = rooms.Count
            };
        }
    }
}
