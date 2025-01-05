using System;

namespace Roomify.Contracts.ResponseModels.ManageUsers;

public class GetUserRoleResponseModel
{
    public List<UserWithRole> userWithRoles { get; set; } = new List<UserWithRole>();
    public int TotalData { get; set; }

}

public class UserWithRole{
    public string UserId { get; set; } ="";
    public string Name { get; set; } ="";
    public List<UserRoles>? userRoles { get; set; } = new List<UserRoles>();
}

public class UserRoles{
   
    public string RoleId { get; set; } = "";
    public string RoleName { get; set; } = "";
    public string DisplayName { get; set; } = "";  // New property for DisplayName
}
