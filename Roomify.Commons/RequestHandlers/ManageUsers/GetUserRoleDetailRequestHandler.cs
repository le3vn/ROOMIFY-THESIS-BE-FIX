using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Roomify.Contracts.RequestModels.ManageUsers;
using Roomify.Contracts.ResponseModels.ManageUsers;
using Roomify.Entities;

namespace Roomify.Commons.RequestHandlers.ManageUsers
{
    public class GetUserRoleDetailsRequestHandler : IRequestHandler<GetUserRoleDetailsRequestModel, GetUserRoleDetailsResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public GetUserRoleDetailsRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<GetUserRoleDetailsResponseModel> Handle(GetUserRoleDetailsRequestModel request, CancellationToken cancellationToken)
        {
            // Validate the UserId
            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                return new GetUserRoleDetailsResponseModel
                {
                    UserId = "",
                    UserName = "UserId cannot be empty",
                    Roles = new List<GetUserRoleDetailsResponseModel.UserRoleDetail>()
                };
            }

            // 1. Get the user's information from the Users table based on UserId
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user == null)
            {
                return new GetUserRoleDetailsResponseModel
                {
                    UserId = request.UserId,
                    UserName = "User not found",
                    Roles = new List<GetUserRoleDetailsResponseModel.UserRoleDetail>()
                };
            }

            // 2. Get all the roles assigned to the user from ManageRoles table
            var manageRoles = await _db.ManageRoles
                .Where(mr => mr.UserId == request.UserId)
                .ToListAsync(cancellationToken);

            if (manageRoles == null || manageRoles.Count == 0)
            {
                return new GetUserRoleDetailsResponseModel
                {
                    UserId = request.UserId,
                    UserName = user.GivenName, // User's name
                    Roles = new List<GetUserRoleDetailsResponseModel.UserRoleDetail>()
                };
            }

            // 3. Get role details from the Roles table
            var roleIds = manageRoles.Select(mr => mr.RoleId).Distinct().ToList();
            var roles = await _db.Roles
                .Where(r => roleIds.Contains(r.Id))
                .ToListAsync(cancellationToken);

            // Prepare the response
            var response = new GetUserRoleDetailsResponseModel
            {
                UserId = request.UserId,
                UserName = user.GivenName, // User's name
                Roles = new List<GetUserRoleDetailsResponseModel.UserRoleDetail>()
            };

            foreach (var manageRole in manageRoles)
            {
                var role = roles.FirstOrDefault(r => r.Id == manageRole.RoleId);
                if (role != null)
                {
                    // Default DisplayName is set to the RoleName
                    string displayName = role.Name;

                    // Check if the role is "Staff" or "StudentOrganization"
                    if (role.Name == "Staff" || role.Name == "StudentOrganization")
                    {
                        // Fetch the organization name based on UserId and RoleId
                        var organization = await _db.Organizations
                            .FirstOrDefaultAsync(o => o.UserId == request.UserId && o.RoleId == role.Id, cancellationToken);

                        if (organization != null)
                        {
                            displayName = organization.Name; // Set DisplayName to the organization name
                        }
                    }

                    // Add role detail to the response
                    response.Roles.Add(new GetUserRoleDetailsResponseModel.UserRoleDetail
                    {
                        RoleId = role.Id.ToString(),
                        RoleName = role.Name,
                        DisplayName = displayName // Set the DisplayName to either RoleName or Organization Name
                    });
                }
            }

            return response;
        }
    }
}
