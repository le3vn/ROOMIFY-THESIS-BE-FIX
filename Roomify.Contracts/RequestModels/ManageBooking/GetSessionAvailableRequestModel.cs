using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageBooking;

namespace Roomify.Contracts.RequestModels.ManageBooking;

public class GetSessionAvailableRequestModel : IRequest<GetSessionAvailableResponseModel>
{
    public int RoomId { get; set; }
    public DateOnly Date {get; set; }
}
