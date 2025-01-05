using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Roomify.Contracts.RequestModels.ManageUsers;
using Roomify.Contracts.ResponseModels.ManageUsers;
using Roomify.Entities;
using Microsoft.EntityFrameworkCore;

namespace Roomify.Commons.RequestHandlers.ManageUsers
{
    public class GetRoleAvailableToAddRequestHandler : IRequestHandler<GetRoleAvailableToAddRequestModel, GetRoleAvailableToAddResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public GetRoleAvailableToAddRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<GetRoleAvailableToAddResponseModel> Handle(GetRoleAvailableToAddRequestModel request, CancellationToken cancellationToken)
        {
            // Step 1: Get all roles except 'Administrator'
            var allRoles = await _db.Roles
                .Where(r => r.Name != "Administrator")
                .ToListAsync(cancellationToken);

            // Step 2: Get all roles already assigned to the user
            var assignedRoles = await _db.ManageRoles
                .Where(mr => mr.UserId == request.UserId)
                .Select(mr => mr.RoleId)
                .ToListAsync(cancellationToken);

            // Step 3: Filter out the roles that are already assigned to the user
            var availableRoles = allRoles
                .Where(r => !assignedRoles.Contains(r.Id))
                .Select(r => new RoleModel
                {
                    RoleId = r.Id,
                    RoleName = r.Name
                })
                .ToList();

            // Step 4: Prepare the response
            return new GetRoleAvailableToAddResponseModel
            {
                RoleAvailable = availableRoles,
                TotalData = availableRoles.Count
            };
        }
    }
}
