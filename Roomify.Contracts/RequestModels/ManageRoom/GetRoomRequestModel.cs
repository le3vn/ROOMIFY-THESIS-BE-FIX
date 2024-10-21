using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageRoom;

namespace Roomify.Contracts.RequestModels.ManageRoom
{
    public class GetRoomRequestModel : IRequest<GetRoomResponseModel>
    {
        public string? Search { get; set; } = string.Empty; 
        public int BuildingId { get; set; } 
        public int RoomTypeId { get; set; } 
        public int Capacity { get; set; } 
        public string SortOrder { get; set; } = "asc"; 
    }
}
