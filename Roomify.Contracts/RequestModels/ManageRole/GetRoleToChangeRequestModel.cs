using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageRole;

namespace Roomify.Contracts.RequestModels.ManageRole;

public class GetRoleToChangeRequestModel : IRequest<GetRoleToChangeResponseModel>
{
    public string UserId { get; set; } = "";
}
