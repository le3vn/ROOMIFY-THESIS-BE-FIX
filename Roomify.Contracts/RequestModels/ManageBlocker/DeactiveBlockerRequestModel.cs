using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageBlocker;

namespace Roomify.Contracts.RequestModels.ManageBlocker;

public class DeactiveBlockerRequestModel : IRequest<DeactiveBlockerResponseModel>
{
    public int BlockerId { get; set; }
}
