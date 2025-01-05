using System.Linq;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Roomify.Contracts.RequestModels.ManageRole;
using Roomify.Contracts.ResponseModels.ManageRole;
using Roomify.Entities;

public class ChangeRoleRequestHandler : IRequestHandler<ChangeRoleRequestModel, ChangeRoleResponseModel>
{
    private readonly ApplicationDbContext _db;

    public ChangeRoleRequestHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ChangeRoleResponseModel> Handle(ChangeRoleRequestModel request, CancellationToken cancellationToken)
    {
        var response = new ChangeRoleResponseModel();

        try
        {
            // Step 1: Retrieve the current role
            var userRole = await _db.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == request.UserId, cancellationToken);

            // Step 2: Remove the existing role if it exists
            if (userRole != null)
            {
                _db.UserRoles.Remove(userRole);
                await _db.SaveChangesAsync(cancellationToken); // Save changes
            }

            // Step 3: Add the new role
            var newUserRole = new IdentityUserRole<string>
            {
                UserId = request.UserId,
                RoleId = request.RoleId // Set the new role ID
            };

            await _db.UserRoles.AddAsync(newUserRole, cancellationToken);

            // Step 4: Save changes again
            await _db.SaveChangesAsync(cancellationToken);

            // Success response
            response.Success = true;
            response.Message = "Role changed successfully.";
        }
        catch (Exception ex)
        {
            // Handle any errors and set response accordingly
            response.Success = false;
            response.Message = $"An error occurred while changing the role: {ex.Message}";
        }

        return response;
    }
}
