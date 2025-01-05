using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageBooking;

namespace Roomify.Contracts.RequestModels.ManageBooking;

public class GetLecturerSubjectRequestModel: IRequest<List<GetLecturerSubjectResponseModel>>
{
    public string LecturerId {get; set; } ="";
}
