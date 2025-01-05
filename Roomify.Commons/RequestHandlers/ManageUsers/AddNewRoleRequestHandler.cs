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
    public class AddNewRoleRequestHandler : IRequestHandler<AddNewRoleRequestModel, AddNewRoleResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public AddNewRoleRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<AddNewRoleResponseModel> Handle(AddNewRoleRequestModel request, CancellationToken cancellationToken)
        {
            var response = new AddNewRoleResponseModel();

            try
            {
                // Check if OrganizationName is null or not
                if (string.IsNullOrEmpty(request.OrganizationName))
                {
                    // If OrganizationName is null, add to ManageRoles
                    var newRole = new ManageRole
                    {
                        UserId = request.UserId,
                        RoleId = request.RoleId,
                        CreatedAt = DateTimeOffset.UtcNow,
                        CreatedBy = "Admin"
                    };

                    _db.ManageRoles.Add(newRole);
                }
                else
                {
                    var newRole = new ManageRole
                    {
                        UserId = request.UserId,
                        RoleId = request.RoleId,
                        CreatedAt = DateTimeOffset.UtcNow,
                        CreatedBy = "Admin"
                    };

                    _db.ManageRoles.Add(newRole);
                    // If OrganizationName is provided, add to Organizations
                    var newOrganization = new Organization
                    {
                        UserId = request.UserId,
                        RoleId = request.RoleId,
                        Name = request.OrganizationName,
                        CreatedAt = DateTimeOffset.UtcNow,
                        CreatedBy = "Admin"
                    };

                    _db.Organizations.Add(newOrganization);
                }

                // Save changes to the database
                await _db.SaveChangesAsync(cancellationToken);

                // Set success response
                response.Success = "True";
                response.Message = "Role added successfully.";
            }
            catch (Exception ex)
            {
                // In case of an error, log and return a failure response
                response.Success = "False";
                response.Message = $"An error occurred: {ex.Message}";
            }

            return response;
        }
    }
}
