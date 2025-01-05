using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageBlocker;

namespace Roomify.Contracts.RequestModels.ManageBlocker;

public class UpdateBlockerRequestModel : IRequest<UpdateBlockerResponseModel>
{ 
    public int BlockerId { get; set; }
    public string BlockerName { get; set; } = "";
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}
