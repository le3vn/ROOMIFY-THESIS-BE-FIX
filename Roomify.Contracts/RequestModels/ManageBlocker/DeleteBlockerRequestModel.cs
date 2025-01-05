using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageBlocker;

namespace Roomify.Contracts.RequestModels.ManageBlocker;

public class DeleteBlockerRequestModel : IRequest<DeleteBlockerResponseModel>
{
    public int BlockerId { get; set; }
}
