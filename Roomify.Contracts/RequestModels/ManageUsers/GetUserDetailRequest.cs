using Roomify.Contracts.ResponseModels.ManageUsers;
using MediatR;

namespace Roomify.Contracts.RequestModels.ManageUsers
{
    public class GetUserDetailRequest : IRequest<GetUserDetailResponse?>
    {
        public string Id { set; get; } = "";
    }
}
