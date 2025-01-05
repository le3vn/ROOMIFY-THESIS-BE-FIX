using System;

namespace Roomify.Contracts.ResponseModels.ManageRole;

public class ChangeRoleResponseModel
{
    public bool Success { get; set; }
    public string Message { get; set; }="";
}
