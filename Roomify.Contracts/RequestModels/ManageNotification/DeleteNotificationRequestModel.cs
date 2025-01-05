using MediatR;
using Roomify.Contracts.ResponseModels.ManageNotification;

namespace Roomify.Contracts.RequestModels.ManageNotification
{
    public class DeleteNotificationRequestModel : IRequest<DeleteNotificationResponseModel>
    {
        public int NotificationId { get; set; }
    }
}
