using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageRole;

namespace Roomify.Contracts.RequestModels.ManageRole;

public class ChangeRoleRequestModel : IRequest<ChangeRoleResponseModel>
{
    public string UserId { get; set; } ="";
    public string RoleId { get; set; } ="";
}
