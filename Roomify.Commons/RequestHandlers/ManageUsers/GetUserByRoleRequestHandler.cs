using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

using Roomify.Entities;
using Microsoft.EntityFrameworkCore;
using Roomify.Contracts.RequestModels.ManageUsers;
using Roomify.Contracts.ResponseModels.ManageUsers;

namespace Roomify.Commons.RequestHandlers.ManageUsers
{
    public class GetUserByRoleRequestHandler : IRequestHandler<GetUserByRoleRequestModel, GetUserByRoleResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public GetUserByRoleRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<GetUserByRoleResponseModel> Handle(GetUserByRoleRequestModel request, CancellationToken cancellationToken)
        {
            // Validate the RoleName
            if (string.IsNullOrWhiteSpace(request.RoleName))
            {
                return new GetUserByRoleResponseModel
                {
                    Users = new List<GetUserByRoleResponseModel.UserRoleInfo>(),
                    // Optionally, you can add error handling to the list if needed
                };
            }

            // 1. Get the RoleId from the Roles table based on the RoleName
            var role = await _db.Roles
                .FirstOrDefaultAsync(r => r.Name == request.RoleName, cancellationToken);

            if (role == null)
            {
                return new GetUserByRoleResponseModel
                {
                    Users = new List<GetUserByRoleResponseModel.UserRoleInfo>(),
                    // You can handle the case where the role is not found if needed
                };
            }

            // 2. Get all ManageRole entries that match the RoleId
            var manageRoles = await _db.ManageRoles
                .Where(mr => mr.RoleId == role.Id)
                .ToListAsync(cancellationToken);

            if (manageRoles == null || manageRoles.Count == 0)
            {
                return new GetUserByRoleResponseModel
                {
                    Users = new List<GetUserByRoleResponseModel.UserRoleInfo>(),
                    // Handle case where no users are found for the role
                };
            }

            // 3. Fetch all the user details based on UserIds from ManageRoles
            var userIds = manageRoles.Select(mr => mr.UserId).Distinct().ToList();
            var users = await _db.Users
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync(cancellationToken);

            // Prepare the response model
            var response = new GetUserByRoleResponseModel
            {
                Users = new List<GetUserByRoleResponseModel.UserRoleInfo>()
            };

            // Add user information to the response
            foreach (var manageRole in manageRoles)
            {
                var user = users.FirstOrDefault(u => u.Id == manageRole.UserId);

                if (user != null)
                {
                    response.Users.Add(new GetUserByRoleResponseModel.UserRoleInfo
                    {
                        UserId = manageRole.UserId,
                        RoleId = role.Id.ToString(),
                        RoleName = request.RoleName,
                        UserName = user.GivenName // Assuming 'Name' is the user's full name field
                    });
                }
            }

            return response;
        }
    }
}
