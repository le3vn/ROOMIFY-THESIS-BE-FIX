using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageGroup;

namespace Roomify.Contracts.RequestModels.ManageGroup;

public class CreateGroupRequestModel : IRequest<CreateGroupResponseModel>
{
    public string GroupName { get; set; } = "";
    public string SSOApprover { get; set; } = "";
    public string SLCApprover { get; set; } = "";
    public string LSCApprover { get; set; } = "";
    public string BMApprover { get; set; } = ""; 
}
