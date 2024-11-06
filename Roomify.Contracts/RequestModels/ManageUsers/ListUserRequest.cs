using MediatR;
using Roomify.Contracts.ResponseModels.ManageUsers;

namespace Roomify.Contracts.RequestModels.ManageUsers
{
    public class ListUserRequest : IRequest<ListUserResponse>
    {
        public string? GivenName { get; set; }
        public string? FamilyName { get; set; }
        public string? Email { get; set; }
        public int? Page { get; set; } = 1; // Optional for pagination
        public int PageSize { get; set; } = 10; // Default page size
        public string SortOrder { get; set; } = "asc"; // Sort order
    }
}
