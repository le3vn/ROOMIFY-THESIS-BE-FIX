using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Roomify.Contracts.RequestModels.ManageGroup;
using Roomify.Contracts.ResponseModels.ManageGroup;
using Roomify.Entities;

namespace Roomify.Commons.RequestHandlers.ManageGroup
{
    public class EditGroupRequestHandler : IRequestHandler<EditGroupRequestModel, EditGroupResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public EditGroupRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<EditGroupResponseModel> Handle(EditGroupRequestModel request, CancellationToken cancellationToken)
        {
            // Validate the request (ensure a valid GroupId is provided)
            if (request.GroupId <= 0)
            {
                return new EditGroupResponseModel
                {
                    Success = "false",
                    Message = "Invalid Group ID."
                };
            }

            // Retrieve the group by GroupId
            var group = await _db.RoomGroups
                .FirstOrDefaultAsync(g => g.RoomGroupId == request.GroupId, cancellationToken);

            // If the group does not exist
            if (group == null)
            {
                return new EditGroupResponseModel
                {
                    Success = "false",
                    Message = "Group not found."
                };
            }

            // Update approver information
            group.ApproverSSOUserId = request.SSOApprover ?? group.ApproverSSOUserId;
            group.ApproverSLCUserId = request.SLCApprover ?? group.ApproverSLCUserId;
            group.ApproverLSCUserId = request.LSCApprover ?? group.ApproverLSCUserId;
            group.ApproverBMUserId = request.BMApprover ?? group.ApproverBMUserId;
            group.Name = request.GroupName ?? group.Name;

            // Save the changes to the database
            try
            {
                await _db.SaveChangesAsync(cancellationToken);
                return new EditGroupResponseModel
                {
                    Success = "true",
                    Message = "Group approvers updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new EditGroupResponseModel
                {
                    Success = "false",
                    Message = $"An error occurred while updating the group: {ex.Message}"
                };
            }
        }
    }
}
