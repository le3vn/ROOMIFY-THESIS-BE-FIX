using MediatR;
using Roomify.Contracts.ResponseModels.ManageNotification;

namespace Roomify.Contracts.RequestModels.ManageNotification
{
    public class GetNotificationUnreadRequestModel : IRequest<GetNotificationUnreadResponseModel>
    {
        public string UserId { get; set; } = ""; // UserId as string
    }
}
