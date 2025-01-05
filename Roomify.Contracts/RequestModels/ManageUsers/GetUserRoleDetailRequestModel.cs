using MediatR;
using Roomify.Contracts.ResponseModels.ManageUsers;

namespace Roomify.Contracts.RequestModels.ManageUsers
{
    public class GetUserRoleDetailsRequestModel : IRequest<GetUserRoleDetailsResponseModel>
    {
        public string UserId { get; set; } = "";
    }
}
