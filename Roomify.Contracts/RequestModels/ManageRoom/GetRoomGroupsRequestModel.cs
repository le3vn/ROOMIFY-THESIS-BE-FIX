using MediatR;
using Roomify.Contracts.ResponseModels.ManageRoom;

namespace Roomify.Contracts.RequestModels.ManageRoom
{
    public class GetRoomGroupsRequestModel : IRequest<GetRoomGroupsResponseModel>
    {
        // No additional properties for now as we are fetching all room groups.
    }
}
