using System;
using System.Text.Json.Serialization;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageBlocker;

namespace Roomify.Contracts.RequestModels.ManageBlocker;

public class PostBlockerRequestModel : IRequest<PostBlockerResponseModel>
{
    public string BlockerName { get; set; } = "";
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}
