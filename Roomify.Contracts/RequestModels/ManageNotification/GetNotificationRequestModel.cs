using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageNotification;

namespace Roomify.Contracts.RequestModels.ManageNotification;

public class GetNotificationRequestModel : IRequest<GetNotificationResponseModel>
{
        public string UserId { get; set; } = "";
        public int Page { get; set; } = 1;  // Default page
        public int PageSize { get; set; } = 10;  // Default page size
}
