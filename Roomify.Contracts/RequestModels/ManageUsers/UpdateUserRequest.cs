using MediatR;
using Microsoft.AspNetCore.Http;

namespace Roomify.Contracts.RequestModels.ManageUsers
{
    public class UpdateUserRequest : IRequest
    {
        public string Id { set; get; } = "";

        public string GivenName { set; get; } = "";

        public string FamilyName { set; get; } = "";

        public string Email { set; get; } = "";

        public bool IsEnabled { set; get; }

        public string Password { set; get; } = "";
        public IFormFile ProfilePicture { get; set; } = null!;
    }
}
