using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Roomify.Contracts.RequestModels.ManageUsers;
using Roomify.Contracts.ResponseModels.ManageUsers;
using Roomify.Entities;

namespace Roomify.Commons.RequestHandlers.ManageUsers
{
    public class DeleteRoleRequestHandler : IRequestHandler<DeleteRoleRequestModel, DeleteRoleResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public DeleteRoleRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<DeleteRoleResponseModel> Handle(DeleteRoleRequestModel request, CancellationToken cancellationToken)
        {
            var response = new DeleteRoleResponseModel();
            
            // Step 1: Retrieve the role information to check if it's a special role
            var role = await _db.Roles
                .FirstOrDefaultAsync(r => r.Id == request.RoleId, cancellationToken);

            if (role == null)
            {
                response.Success = "False";
                response.Message = "Role not found.";
                return response;
            }

            // Step 2: Check if role is Staff or StudentOrganization
            if ((role.Name == "Staff" || role.Name == "StudentOrganization") && string.IsNullOrEmpty(request.OrganizationName))
            {
                // If the role is Staff or StudentOrganization and no OrganizationName is provided, return a response
                response.Success = "False";
                response.Message = "Role cannot be deleted without an organization name for 'Staff' or 'StudentOrganization'.";
                return response;
            }

            // Step 3: Remove the role from ManageRoles table
            var manageRole = await _db.ManageRoles
                .FirstOrDefaultAsync(mr => mr.UserId == request.UserId && mr.RoleId == request.RoleId, cancellationToken);

            if (manageRole != null)
            {
                _db.ManageRoles.Remove(manageRole);
                await _db.SaveChangesAsync(cancellationToken);
            }

            // Step 4: If the role is Staff or StudentOrganization and an OrganizationName is provided,
            // remove the entry from the Organizations table
            if (role.Name == "Staff" || role.Name == "StudentOrganization")
            {
                if (!string.IsNullOrEmpty(request.OrganizationName))
                {
                    var organizationRole = await _db.Organizations
                        .FirstOrDefaultAsync(o => o.UserId == request.UserId &&
                                                   o.RoleId == request.RoleId &&
                                                   o.Name == request.OrganizationName, cancellationToken);

                    if (organizationRole != null)
                    {
                        _db.Organizations.Remove(organizationRole);
                        await _db.SaveChangesAsync(cancellationToken);
                    }
                }
            }

            response.Success = "True";
            response.Message = "Role deleted successfully.";
            return response;
        }
    }
}
