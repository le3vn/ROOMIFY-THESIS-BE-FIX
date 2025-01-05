using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Roomify.Contracts.RequestModels.ManageBlocker;
using Roomify.Contracts.ResponseModels.ManageBlocker;
using Roomify.Entities;

namespace Roomify.Commons.RequestHandlers.ManageBlocker
{
    public class GetBlockerListRequestHandler : IRequestHandler<GetBlockerListRequestModel, GetBlockerListResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public GetBlockerListRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<GetBlockerListResponseModel> Handle(GetBlockerListRequestModel request, CancellationToken cancellationToken)
        {
            var blockers = await _db.Blockers
                .ToListAsync(cancellationToken);

            var totalData = blockers.Count;

            var response = new GetBlockerListResponseModel
            {
                BlockerLists = blockers.Select(blocker => new BlockerModel
                {
                    BlockerId = blocker.BlockerId,             
                    BlockerName = blocker.Name,               
                    StartDate = blocker.StartDate,             
                    EndDate = blocker.EndDate,
                    IsActive = blocker.IsActive                
                }).ToList(),
                TotalData = totalData
            };

            return response;
        }
    }
}
