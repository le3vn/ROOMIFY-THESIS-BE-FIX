using Roomify.Contracts.ResponseModels.ManageRoom;
using Roomify.Contracts.RequestModels.ManageRoom;
using Roomify.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Roomify.Contracts.RequestModels.ManageUsers;
using Roomify.Contracts.ResponseModels.ManageUsers;
using Roomify.Commons.Services;

namespace Roomify.Commons.RequestHandlers.ManageUsers
{
    public class ListUserRequestHandler : IRequestHandler<ListUserRequest, ListUserResponse>
    {
        private readonly ApplicationDbContext _db;
        private readonly IStorageService _storageService;

        public ListUserRequestHandler(ApplicationDbContext db, IStorageService storageService)
        {
            _db = db;
            _storageService = storageService;
        }

        public async Task<ListUserResponse> Handle(ListUserRequest request, CancellationToken cancellationToken)
        {
            var query = _db.Users.Include(u => u.Blob).AsQueryable();

            if (!string.IsNullOrEmpty(request.GivenName))
            {
                query = query.Where(r => r.GivenName.Contains(request.GivenName));
            }
            if (!string.IsNullOrEmpty(request.FamilyName))
            {
                query = query.Where(r => r.FamilyName.Contains(request.FamilyName));
            }
            if (!string.IsNullOrEmpty(request.Email))
            {
                query = query.Where(r => r.Email.Contains(request.Email));
            }
            
            query = request.SortOrder.ToLower() == "desc" 
                ? query.OrderByDescending(u => u.GivenName) 
                : query.OrderBy(u => u.GivenName);

            var users = await query.ToListAsync(cancellationToken);
            var userModels = new List<UserModel>();

            foreach (var user in users)
            {
                var userModel = new UserModel
                {
                    Id = user.Id,
                    GivenName = user.GivenName,
                    FamilyName = user.FamilyName,
                    Email = user.Email
                };

                if (user.Blob != null && !string.IsNullOrEmpty(user.Blob.FilePath))
                {
                    try
                    {
                        userModel.MinioUrl = await _storageService.GetPresignedUrlReadAsync(user.Blob.FilePath);
                    }
                    catch (Exception)
                    {
                        userModel.MinioUrl = "Error generating URL"; 
                    }
                }
                else
                {
                    userModel.MinioUrl = ""; // Or set a default value
                }

                userModels.Add(userModel);
            }

            return new ListUserResponse
            {
                UserList = userModels,
                TotalData = userModels.Count
            };
        }

    }
}
