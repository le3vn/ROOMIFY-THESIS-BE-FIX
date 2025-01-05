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
    public class GetUserRoleRequestHandler : IRequestHandler<GetUserRoleRequestModel, GetUserRoleResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public GetUserRoleRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<GetUserRoleResponseModel> Handle(GetUserRoleRequestModel request, CancellationToken cancellationToken)
        {
            // 1. Query all users with their givenName
            var usersQuery = _db.Users.AsQueryable();

            // Optionally filter users based on search query
            if (!string.IsNullOrEmpty(request.Search))
            {
                usersQuery = usersQuery.Where(u => u.GivenName.Contains(request.Search));
            }

            // 2. Get the roles associated with each user and their role details
            var usersWithRoles = await usersQuery
                .Select(user => new UserWithRole
                {
                    UserId = user.Id,
                    Name = user.GivenName,
                    userRoles = _db.ManageRoles
                        .Where(mr => mr.UserId == user.Id)
                        .Select(mr => new UserRoles
                        {
                            RoleId = mr.RoleId,
                            RoleName = _db.Roles
                                .Where(r => r.Id == mr.RoleId)
                                .Select(r => r.Name)
                                .FirstOrDefault()
                        }).ToList()
                })
                .ToListAsync(cancellationToken);

            // 3. Update role names and display names for Staff and StudentOrganization based on Organization data
            foreach (var userWithRole in usersWithRoles)
            {
                foreach (var userRole in userWithRole.userRoles)
                {
                    // Check if the role is Staff or StudentOrganization
                    if (userRole.RoleName == "Staff" || userRole.RoleName == "StudentOrganization")
                    {
                        // Fetch the organization name based on RoleId and UserId
                        var organization = await _db.Organizations
                            .Where(org => org.UserId == userWithRole.UserId && org.RoleId == userRole.RoleId)
                            .Select(org => org.Name)
                            .FirstOrDefaultAsync(cancellationToken);

                        // If organization exists, update the RoleName with the organization name and set DisplayName
                        if (organization != null)
                        {
                            userRole.RoleName = userRole.RoleName;  // Update RoleName
                            userRole.DisplayName = organization;  // Set DisplayName to organization name
                        }
                    }
                    else
                    {
                        // For other roles, set DisplayName as the Role Name
                        userRole.DisplayName = userRole.RoleName;
                    }
                }
            }

            // 4. Create response model
            var response = new GetUserRoleResponseModel
            {
                userWithRoles = usersWithRoles,
                TotalData = usersWithRoles.Count
            };

            return response;
        }
    }
}
