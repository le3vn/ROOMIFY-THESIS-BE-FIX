using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageBooking;

namespace Roomify.Contracts.RequestModels.ManageBooking;

public class GetApproverHistoryRequestModel : IRequest<GetApproverHistoryResponseModel>
{
    public string UserId { get; set; } = "";
    public bool IsApproved { get; set; }
}
