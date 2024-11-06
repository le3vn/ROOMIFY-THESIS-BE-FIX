using Roomify.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Roomify.Contracts.RequestModels.ManageRole;
using Roomify.Contracts.ResponseModels.ManageRole;

namespace Roomify.Commons.RequestHandlers.ManageRoles
{
    public class GetRoleToChangeRequestHandler : IRequestHandler<GetRoleToChangeRequestModel, GetRoleToChangeResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public GetRoleToChangeRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<GetRoleToChangeResponseModel> Handle(GetRoleToChangeRequestModel request, CancellationToken cancellationToken)
        {
            var userRoles = await _db.ManageRoles
                .Where(mr => mr.UserId == request.UserId)
                .Select(mr => mr.RoleId)
                .ToListAsync(cancellationToken);

            if (!userRoles.Any())
            {
                return new GetRoleToChangeResponseModel
                {
                    TotalData = 0,
                    UserRoles = new List<RoleList>()
                };
            }

            var roles = await _db.Roles
                .Where(r => userRoles.Contains(r.Id))
                .Select(r => new RoleList
                {
                    RoleId = r.Id,
                    RoleName = r.Name
                })
                .ToListAsync(cancellationToken);

            return new GetRoleToChangeResponseModel
            {
                TotalData = roles.Count,
                UserRoles = roles
            };
        }
    }
}
