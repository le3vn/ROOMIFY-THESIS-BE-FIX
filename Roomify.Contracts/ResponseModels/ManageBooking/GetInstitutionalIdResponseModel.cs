using System;

namespace Roomify.Contracts.ResponseModels.ManageBooking;

public class GetInstitutionalIdResponseModel
{
    public string? LecturersId {get; set;}
    public string? StaffsId {get; set;}
    public string? StudentsId {get; set;}

}
