using MediatR;
using Roomify.Contracts.ResponseModels.ManageGroup;
using Roomify.Contracts.ResponseModels.ManageUsers;

namespace Roomify.Contracts.RequestModels.ManageUsers
{
    public class GetUserByRoleRequestModel : IRequest<GetUserByRoleResponseModel>
    {
        public string RoleName { get; set; } = ""; // RoleName to search for
    }
}
