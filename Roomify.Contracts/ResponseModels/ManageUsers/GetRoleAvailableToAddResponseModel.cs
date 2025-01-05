using System;

namespace Roomify.Contracts.ResponseModels.ManageUsers;

public class GetRoleAvailableToAddResponseModel
{
    public List<RoleModel> RoleAvailable { get; set; } = new List<RoleModel>();
    public int TotalData { get; set; }
}

public class RoleModel{
    public string RoleId { get; set; } ="";
    public string RoleName { get; set; } ="";
}
