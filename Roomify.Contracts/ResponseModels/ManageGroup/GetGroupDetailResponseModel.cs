namespace Roomify.Contracts.ResponseModels.ManageGroup
{
    public class GetGroupDetailResponseModel
    {
        public string GroupName { get; set; } = "";
        public string SSOApprover { get; set; } = "";
        public string SLCApprover { get; set; } = "";
        public string LSCApprover { get; set; } = "";
        public string BMApprover { get; set; } = ""; 
    }
}
