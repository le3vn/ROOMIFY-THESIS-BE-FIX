using Roomify.Contracts.RequestModels.ManageUsers;
using Roomify.Contracts.ResponseModels.ManageUsers;
using Roomify.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Roomify.RequestHandlers.ManageUsers
{
    public class GetUserDetailRequestHandler : IRequestHandler<GetUserDetailRequest, GetUserDetailResponse?>
    {
        private readonly UserManager<User> _userManager;

        public GetUserDetailRequestHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<GetUserDetailResponse?> Handle(GetUserDetailRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id);
            if (user == null)
            {
                return null;
            }

            return new GetUserDetailResponse
            {
                Id = user.Id,
                Email = user.Email,
                FamilyName = user.FamilyName,
                GivenName = user.GivenName,
                IsEnabled = user.IsEnabled,
            };
        }
    }
}
