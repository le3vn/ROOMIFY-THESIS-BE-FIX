using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageRoom;

namespace Roomify.Contracts.RequestModels.ManageRoom;

public class GetRoomUserViewRequestModel : IRequest<GetRoomUserViewResponseModel>
{
    public int BuildingId { get; set; }
    public string UserRole { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public DateOnly DateToBook { get; set; }
}
