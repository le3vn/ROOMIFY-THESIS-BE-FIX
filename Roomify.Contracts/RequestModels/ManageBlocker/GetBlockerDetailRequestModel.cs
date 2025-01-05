using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageBlocker;

namespace Roomify.Contracts.RequestModels.ManageBlocker;

public class GetBlockerDetailRequestModel : IRequest<GetBlockerDetailResponseModel>
{
    public int BlockerId { get; set; }
}
