using System.Collections.Generic;

namespace Roomify.Contracts.ResponseModels.ManageUsers
{
    public class GetUserRoleDetailsResponseModel
    {
        public string UserId { get; set; } = "";
        public string UserName { get; set; } = ""; // User's name

        public List<UserRoleDetail> Roles { get; set; } = new List<UserRoleDetail>(); // List of roles for the user

        public class UserRoleDetail
        {
            public string RoleId { get; set; } = ""; // RoleId
            public string RoleName { get; set; } = ""; // Role Name
            public string DisplayName { get; set; } = ""; // Organization Name (if applicable)
        }
    }
}
