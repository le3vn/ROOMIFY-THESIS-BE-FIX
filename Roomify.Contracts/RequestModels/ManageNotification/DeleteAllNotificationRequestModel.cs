using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageNotification;

namespace Roomify.Contracts.RequestModels.ManageNotification;

public class DeleteAllNotificationRequestModel : IRequest<DeleteAllNotificationResponseModel>
{
    public List<int>? Id { get; set; }
}
