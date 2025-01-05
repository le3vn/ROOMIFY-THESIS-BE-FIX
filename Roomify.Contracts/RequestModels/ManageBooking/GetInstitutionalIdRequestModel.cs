using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageBooking;

namespace Roomify.Commons.RequestModels.ManageBooking;

public class GetInstitutionalIdRequestModel : IRequest<GetInstitutionalIdResponseModel>
{
    public string UserId {get; set;} ="";
}
