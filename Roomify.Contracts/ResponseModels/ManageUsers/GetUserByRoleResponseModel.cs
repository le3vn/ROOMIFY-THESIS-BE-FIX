using System.Collections.Generic;

namespace Roomify.Contracts.ResponseModels.ManageUsers
{
    public class GetUserByRoleResponseModel
    {
        public List<UserRoleInfo> Users { get; set; } = new List<UserRoleInfo>(); // List of users for the role

        public class UserRoleInfo
        {
            public string UserId { get; set; } = "";   // UserId associated with the Role
            public string RoleId { get; set; } = "";   // RoleId associated with the Role
            public string RoleName { get; set; } = ""; // Role name
            public string UserName { get; set; } = ""; // User name fetched from Users table
        }
    }
}
