using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageUsers;

namespace Roomify.Contracts.RequestModels.ManageUsers;

public class GetUserRoleRequestModel : IRequest<GetUserRoleResponseModel>
{
    public string? Search { get; set; }
}
