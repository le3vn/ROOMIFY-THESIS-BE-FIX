using MediatR;
using Roomify.Contracts.ResponseModels.ManageGroup;

namespace Roomify.Contracts.RequestModels.ManageGroup
{
    public class EditGroupRequestModel : EditGroupModel, IRequest<EditGroupResponseModel>
    {
        public int GroupId { get; set; }
    }

public class EditGroupModel
{
    public string GroupName { get; set; } = "";
    public string SSOApprover { get; set; } = "";
    public string SLCApprover { get; set; } = "";
    public string LSCApprover { get; set; } = "";
    public string BMApprover { get; set; } = ""; 
}

}
