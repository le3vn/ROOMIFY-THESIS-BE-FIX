using System.Linq;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Roomify.Contracts.RequestModels.ManageRole;
using Roomify.Contracts.ResponseModels.ManageRole;
using Roomify.Entities;

public class GetDisplayNameStudentOrganizationRequestHandler : IRequestHandler<GetDisplayNameStudentOrganizationRequestModel, GetDisplayNameStudentOrganizationResponseModel>
{
    private readonly ApplicationDbContext _db;

    public GetDisplayNameStudentOrganizationRequestHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<GetDisplayNameStudentOrganizationResponseModel> Handle(GetDisplayNameStudentOrganizationRequestModel request, CancellationToken cancellationToken)
    {
        // Get the RoleName and UserId from the request
        var roleName = request.RoleName;
        var userId = request.UserId;

        // 1. Find the RoleId based on the RoleName
        var role = await _db.Roles
            .Where(r => r.Name == roleName)
            .FirstOrDefaultAsync(cancellationToken);

        // If the role is not found, return an empty response
        if (role == null)
        {
            return new GetDisplayNameStudentOrganizationResponseModel
            {
                Name = string.Empty
            };
        }

        // 2. Find the organization based on UserId and RoleId
        var organization = await _db.Organizations
            .Where(org => org.UserId == userId && org.RoleId == role.Id)
            .FirstOrDefaultAsync(cancellationToken);

        // 3. Prepare the response
        var response = new GetDisplayNameStudentOrganizationResponseModel();

        if (organization != null)
        {
            // Set the organization name if found
            response.Name = organization.Name;
        }
        else
        {
            // Return empty if no organization matches
            response.Name = string.Empty;
        }

        return response;
    }
}
