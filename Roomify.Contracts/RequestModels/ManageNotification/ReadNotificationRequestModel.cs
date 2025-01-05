using MediatR;
using Roomify.Contracts.ResponseModels.ManageNotification;

namespace Roomify.Contracts.RequestModels.ManageNotification
{
    public class ReadNotificationRequestModel : IRequest<ReadNotificationResponseModel>
    {
        public int NotificationId { get; set; }
    }
}
