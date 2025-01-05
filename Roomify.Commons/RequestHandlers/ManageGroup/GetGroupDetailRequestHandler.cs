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
    public class GetGroupDetailRequestHandler : IRequestHandler<GetGroupDetailRequestModel, GetGroupDetailResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public GetGroupDetailRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<GetGroupDetailResponseModel> Handle(GetGroupDetailRequestModel request, CancellationToken cancellationToken)
        {
            // Validate the GroupId (ensure it's a valid ID)
            if (request.GroupId <= 0)
            {
                return new GetGroupDetailResponseModel
                {
                    GroupName = "",
                    SSOApprover = "",
                    SLCApprover = "",
                    LSCApprover = "",
                    BMApprover = ""
                };
            }

            // Retrieve the group by GroupId from the database
            var group = await _db.RoomGroups
                .FirstOrDefaultAsync(g => g.RoomGroupId == request.GroupId, cancellationToken);

            // If the group does not exist, return empty details
            if (group == null)
            {
                return new GetGroupDetailResponseModel
                {
                    GroupName = "",
                    SSOApprover = "",
                    SLCApprover = "",
                    LSCApprover = "",
                    BMApprover = ""
                };
            }

            // Map the group entity to the response model
            var response = new GetGroupDetailResponseModel
            {
                GroupName = group.Name,
                SSOApprover = group.ApproverSSOUserId ?? "",
                SLCApprover = group.ApproverSLCUserId ?? "",
                LSCApprover = group.ApproverLSCUserId ?? "",
                BMApprover = group.ApproverBMUserId ?? ""
            };

            return response;
        }
    }
}
