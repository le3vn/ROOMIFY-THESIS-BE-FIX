using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageBooking;

namespace Roomify.Contracts.RequestModels.ManageBooking;

public class GetBookingUserViewRequestModel: IRequest<GetBookingUserViewResponseModel>
{
    public string UserId { get; set; }="";
    public string RoleName { get; set; }="";
    public bool IsCanceled { get; set; }
}
