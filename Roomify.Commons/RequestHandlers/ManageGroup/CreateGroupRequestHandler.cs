using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Roomify.Contracts.RequestModels.ManageGroup;
using Roomify.Contracts.ResponseModels.ManageGroup;
using Roomify.Entities;
using Microsoft.EntityFrameworkCore;

namespace Roomify.Commons.RequestHandlers.ManageGroup
{
    public class CreateGroupRequestHandler : IRequestHandler<CreateGroupRequestModel, CreateGroupResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public CreateGroupRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<CreateGroupResponseModel> Handle(CreateGroupRequestModel request, CancellationToken cancellationToken)
        {
            // Validate the request model (you can add more specific validation as required)
            if (string.IsNullOrWhiteSpace(request.GroupName))
            {
                return new CreateGroupResponseModel
                {
                    Success = "false",
                    Message = "Group name cannot be empty."
                };
            }

            // Check if the group already exists (optional, depending on your requirements)
            var existingGroup = await _db.RoomGroups
                .FirstOrDefaultAsync(g => g.Name == request.GroupName, cancellationToken);
            if (existingGroup != null)
            {
                return new CreateGroupResponseModel
                {
                    Success = "false",
                    Message = "A group with the same name already exists."
                };
            }

            // Create a new RoomGroup entity
            var newGroup = new RoomGroup
            {
                Name = request.GroupName,
                ApproverSSOUserId = request.SSOApprover,
                ApproverSLCUserId = request.SLCApprover,
                ApproverLSCUserId = request.LSCApprover,
                ApproverBMUserId = request.BMApprover,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = "Admin"
            };

            // Add the new group to the RoomGroups table
            _db.RoomGroups.Add(newGroup);

            try
            {
                // Save changes to the database
                await _db.SaveChangesAsync(cancellationToken);

                return new CreateGroupResponseModel
                {
                    Success = "true",
                    Message = "Group created successfully."
                };
            }
            catch (Exception ex)
            {
                // Handle any potential errors during database save
                return new CreateGroupResponseModel
                {
                    Success = "false",
                    Message = $"An error occurred while creating the group: {ex.Message}"
                };
            }
        }
    }
}
