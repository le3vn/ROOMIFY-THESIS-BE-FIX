using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageUsers;

namespace Roomify.Contracts.RequestModels.ManageUsers;

public class AddNewRoleRequestModel : IRequest<AddNewRoleResponseModel>
{
    public string UserId { get; set; } ="";
    public string RoleId { get; set; } ="";
    public string? OrganizationName { get; set; } ="";
}
