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
    public class DeleteBlockerRequestHandler : IRequestHandler<DeleteBlockerRequestModel, DeleteBlockerResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public DeleteBlockerRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<DeleteBlockerResponseModel> Handle(DeleteBlockerRequestModel request, CancellationToken cancellationToken)
        {
            var blocker = await _db.Blockers
                                    .FirstOrDefaultAsync(b => b.BlockerId == request.BlockerId, cancellationToken);
            
            if (blocker == null)
            {
                return new DeleteBlockerResponseModel
                {
                    Success = "false",
                    Message = $"Blocker with ID {request.BlockerId} not found."
                };
            }

            _db.Blockers.Remove(blocker);

            await _db.SaveChangesAsync(cancellationToken);

            return new DeleteBlockerResponseModel
            {
                Success = "true",
                Message = $"Blocker with ID {request.BlockerId} has been successfully deleted."
            };
        }
    }
}
