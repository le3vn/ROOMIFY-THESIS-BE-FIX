using MediatR;
using Roomify.Contracts.ResponseModels.ManageGroup;

namespace Roomify.Contracts.RequestModels.ManageGroup
{
    public class GetGroupDetailRequestModel : IRequest<GetGroupDetailResponseModel>
    {
        public int GroupId { get; set; } // GroupId to identify the group whose details are being fetched
    }
}
