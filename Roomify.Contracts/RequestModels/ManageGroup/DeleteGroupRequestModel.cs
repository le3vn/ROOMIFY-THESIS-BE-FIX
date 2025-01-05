using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageGroup;

namespace Roomify.Contracts.RequestModels.ManageGroup;

public class DeleteGroupRequestModel : IRequest<DeleteGroupResponseModel>
{
    public int GroupId { get; set; }
}
