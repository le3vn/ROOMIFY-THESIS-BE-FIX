using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageUsers;

namespace Roomify.Contracts.RequestModels.ManageUsers;

public class GetRoleAvailableToAddRequestModel : IRequest<GetRoleAvailableToAddResponseModel>
{
    public string UserId { get; set; } ="";
}
