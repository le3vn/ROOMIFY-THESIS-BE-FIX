using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageBooking;

namespace Roomify.Contracts.RequestModels.ManageBooking;

public class GetBookingToManageRequestModel : IRequest<GetBookingToManageResponseModel>
{
    public int? BuildingId { get; set; }
    public int? RoomId { get; set; }
    public string? Search { get; set; }
}
