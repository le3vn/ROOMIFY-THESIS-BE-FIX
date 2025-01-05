using MediatR;
using Roomify.Contracts.ResponseModels.ManageUsers;

namespace Roomify.Contracts.RequestModels.ManageUsers
{
    public class EditUserRoleRequestModel : IRequest<EditUserRoleResponseModel>
    {
        public string UserId { get; set; } = string.Empty;
        public List<string> RoleIds { get; set; } = new List<string>();  // A list of role IDs
        public string? OrganizationName { get; set; }  // Optional, only for staff or student organization roles
    }
}
