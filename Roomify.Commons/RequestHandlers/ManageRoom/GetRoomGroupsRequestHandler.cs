using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Roomify.Contracts.RequestModels.ManageRoom;
using Roomify.Contracts.ResponseModels.ManageRoom;
using Roomify.Entities;

namespace Roomify.Commons.RequestHandlers.ManageRooms
{
    public class GetRoomGroupsRequestHandler : IRequestHandler<GetRoomGroupsRequestModel, GetRoomGroupsResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public GetRoomGroupsRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<GetRoomGroupsResponseModel> Handle(GetRoomGroupsRequestModel request, CancellationToken cancellationToken)
        {
            // 1. Query all room groups
            var roomGroupsQuery = _db.RoomGroups.AsQueryable();

            // 2. Fetch the room groups from the database
            var roomGroups = await roomGroupsQuery
                .Select(rg => new RoomGroupModel
                {
                    Id = rg.RoomGroupId,
                    Name = rg.Name.ToString()
                    // You can add other properties of RoomGroup here if necessary.
                })
                .ToListAsync(cancellationToken);

            // 3. Create and return the response model
            var response = new GetRoomGroupsResponseModel
            {
                RoomGroups = roomGroups,
                TotalData = roomGroups.Count
            };

            return response;
        }
    }
}
