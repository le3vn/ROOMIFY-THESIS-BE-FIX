using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageBooking;

namespace Roomify.Contracts.RequestModels.ManageBooking
{
    public class GetApproverViewRequestModel : IRequest<GetApproverViewResponseModel>
    {
        // Required BookingId
        public string ApproverId { get; set; } = string.Empty;
        // Optional filters
        public string? Search { get; set; } = string.Empty;
        public int RoomId { get; set; } // Nullable Guid for optional filtering
        public int SessionId { get; set; } // Nullable DateTime for optional filtering
        public string SortOrder { get; set; } = "asc";
        // Sorting property
        
    }
}
