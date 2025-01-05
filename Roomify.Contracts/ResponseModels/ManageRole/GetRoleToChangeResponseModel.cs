using System;

namespace Roomify.Contracts.ResponseModels.ManageRole;

public class GetRoleToChangeResponseModel
{
    public int TotalData { get; set; }
    public List<RoleList> UserRoles { get; set; } = new List<RoleList>(); 
}
public class RoleList{
    public string RoleId { get; set; } ="";
    public string RoleName { get; set; } ="";
}
