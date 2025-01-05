using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Roomify.Contracts.RequestModels.ManageUsers;
using Roomify.Contracts.ResponseModels.ManageUsers;
using Roomify.Entities;
using Microsoft.EntityFrameworkCore;

namespace Roomify.Commons.RequestHandlers.ManageUsers
{
    public class EditUserRoleRequestHandler : IRequestHandler<EditUserRoleRequestModel, EditUserRoleResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public EditUserRoleRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<EditUserRoleResponseModel> Handle(EditUserRoleRequestModel request, CancellationToken cancellationToken)
        {
            // 1. Check if the user exists
            var user = await _db.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return new EditUserRoleResponseModel
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            // 2. Check if roles exist and handle multiple roles
            foreach (var roleId in request.RoleIds)
            {
                var role = await _db.Roles.FindAsync(roleId);
                if (role == null)
                {
                    return new EditUserRoleResponseModel
                    {
                        Success = false,
                        Message = $"Role with ID {roleId} not found."
                    };
                }

                // 3. Check if the user already has this role
                var existingRole = await _db.ManageRoles
                    .FirstOrDefaultAsync(mr => mr.UserId == request.UserId && mr.RoleId == roleId, cancellationToken);

                if (existingRole != null)
                {
                    // If role exists, just update the CreatedAt field
                    existingRole.CreatedAt = DateTimeOffset.UtcNow; // Update creation time
                    existingRole.CreatedBy = "Admin"; // Optionally update the creator if necessary

                    // If it's a staff or student organization, ensure organization handling
                    if (role.Name == "Staff" || role.Name == "StudentOrganization")
                    {
                        if (string.IsNullOrEmpty(request.OrganizationName))
                        {
                            return new EditUserRoleResponseModel
                            {
                                Success = false,
                                Message = "OrganizationName is required for staff or student organization roles."
                            };
                        }

                        // Check if the organization already exists for this user and role
                        var existingOrganization = await _db.Organizations
                            .FirstOrDefaultAsync(o => o.UserId == request.UserId && o.RoleId == roleId, cancellationToken);

                        if (existingOrganization == null)
                        {
                            // Create the organization entry if not exists
                            var organization = new Organization
                            {
                                Name = request.OrganizationName,
                                RoleId = roleId,
                                UserId = request.UserId,
                                CreatedAt = DateTimeOffset.UtcNow,
                                CreatedBy = "Admin"
                            };

                            await _db.Organizations.AddAsync(organization, cancellationToken);
                        }
                        else
                        {
                            // Optionally update the organization if needed
                            existingOrganization.Name = request.OrganizationName;
                            existingOrganization.CreatedAt = DateTimeOffset.UtcNow;
                            existingOrganization.CreatedBy = "Admin";
                        }
                    }
                    continue; // Move to the next role
                }

                // 4. Add entry to ManageRoles if it doesn't already exist
                var manageRole = new ManageRole
                {
                    UserId = request.UserId,
                    RoleId = roleId,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = "Admin"
                };

                await _db.ManageRoles.AddAsync(manageRole, cancellationToken);

                // 5. Handle organization logic for Staff/StudentOrganization roles
                if (role.Name == "Staff" || role.Name == "StudentOrganization")
                {
                    if (string.IsNullOrEmpty(request.OrganizationName))
                    {
                        return new EditUserRoleResponseModel
                        {
                            Success = false,
                            Message = "OrganizationName is required for staff or student organization roles."
                        };
                    }

                    // Check if the organization already exists for this user and role
                    var existingOrganization = await _db.Organizations
                        .FirstOrDefaultAsync(o => o.UserId == request.UserId && o.RoleId == roleId, cancellationToken);

                    if (existingOrganization == null)
                    {
                        // Create the organization entry if not exists
                        var organization = new Organization
                        {
                            Name = request.OrganizationName,
                            RoleId = roleId,
                            UserId = request.UserId,
                            CreatedAt = DateTimeOffset.UtcNow,
                            CreatedBy = "Admin"
                        };

                        await _db.Organizations.AddAsync(organization, cancellationToken);
                    }
                    else
                    {
                        // Optionally update the organization if needed
                        existingOrganization.Name = request.OrganizationName;
                        existingOrganization.CreatedAt = DateTimeOffset.UtcNow;
                        existingOrganization.CreatedBy = "Admin";
                    }
                }
            }

            // 6. Commit transaction
            await _db.SaveChangesAsync(cancellationToken);

            return new EditUserRoleResponseModel
            {
                Success = true,
                Message = "Roles successfully added/updated for the user."
            };
        }
    }
}
