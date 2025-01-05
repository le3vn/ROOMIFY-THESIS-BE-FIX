using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageRole;

namespace Roomify.Contracts.RequestModels.ManageRole;

public class GetDisplayNameStudentOrganizationRequestModel : IRequest<GetDisplayNameStudentOrganizationResponseModel>
{
    public string UserId { get; set; } = "";
    public string RoleName { get; set; } = "";
}
