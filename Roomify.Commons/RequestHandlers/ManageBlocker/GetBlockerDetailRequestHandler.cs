using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Roomify.Contracts.RequestModels.ManageBlocker;
using Roomify.Contracts.ResponseModels.ManageBlocker;
using Roomify.Entities;

namespace Roomify.Commons.RequestHandlers.ManageBlocker
{
    public class GetBlockerDetailRequestHandler : IRequestHandler<GetBlockerDetailRequestModel, GetBlockerDetailResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public GetBlockerDetailRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<GetBlockerDetailResponseModel> Handle(GetBlockerDetailRequestModel request, CancellationToken cancellationToken)
        {
            // Fetch the blocker details from the database
            var blocker = await _db.Blockers
                .AsNoTracking() // For read-only operations, this is more efficient
                .FirstOrDefaultAsync(b => b.BlockerId == request.BlockerId, cancellationToken);

            if (blocker == null)
            {
                // Handle case where blocker does not exist, throw exception or return default model
                // For example, you can throw an exception:
                throw new KeyNotFoundException($"Blocker with ID {request.BlockerId} not found.");
                // Or return a default response:
                // return new GetBlockerDetailResponseModel(); // Optional, depending on requirements
            }

            // Map to response model
            return new GetBlockerDetailResponseModel
            {
                BlockerId = blocker.BlockerId,
                BlockerName = blocker.Name,
                StartDate = blocker.StartDate,
                EndDate = blocker.EndDate,
                IsActive = blocker.IsActive
            };
        }
    }
}
