using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Roomify.Contracts.RequestModels.ManageBlocker;
using Roomify.Contracts.ResponseModels.ManageBlocker;
using Roomify.Entities;

namespace Roomify.Commons.RequestHandlers.ManageBlocker
{
    public class UpdateBlockerRequestHandler : IRequestHandler<UpdateBlockerRequestModel, UpdateBlockerResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public UpdateBlockerRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<UpdateBlockerResponseModel> Handle(UpdateBlockerRequestModel request, CancellationToken cancellationToken)
        {
            // Validate if the new blocker overlaps with any existing blocker (active blockers)
            var overlappingBlocker = await _db.Blockers
                .Where(b => b.IsActive && b.BlockerId != request.BlockerId) // Exclude the current blocker from the check
                .AnyAsync(b =>
                    (request.StartDate < b.EndDate && request.EndDate > b.StartDate), // Check for overlap
                    cancellationToken);

            if (overlappingBlocker)
            {
                // If there is an overlap, return an error message
                return new UpdateBlockerResponseModel
                {
                    Success = "false",
                    Message = "The date range overlaps with an existing blocker."
                };
            }

            // Find the blocker to update
            var blocker = await _db.Blockers
                .FirstOrDefaultAsync(b => b.BlockerId == request.BlockerId, cancellationToken);

            if (blocker == null)
            {
                // If blocker not found, return an error
                return new UpdateBlockerResponseModel
                {
                    Success = "false",
                    Message = "Blocker not found."
                };
            }

            // Update the blocker fields
            blocker.Name = request.BlockerName;
            blocker.StartDate = request.StartDate;
            blocker.EndDate = request.EndDate;

            // Save changes to the database
            try
            {
                await _db.SaveChangesAsync(cancellationToken);

                // Return success response
                return new UpdateBlockerResponseModel
                {
                    Success = "true",
                    Message = "Blocker successfully updated."
                };
            }
            catch (Exception ex)
            {
                // Handle errors and return failure message
                return new UpdateBlockerResponseModel
                {
                    Success = "false",
                    Message = $"Error occurred while updating blocker: {ex.Message}"
                };
            }
        }
    }
}
