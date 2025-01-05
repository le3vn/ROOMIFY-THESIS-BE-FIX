using Roomify.Contracts.RequestModels.ManageUsers;
using Roomify.Contracts.ResponseModels.ManageUsers;
using Roomify.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Roomify.Commons.Services;
using Microsoft.EntityFrameworkCore;

namespace Roomify.RequestHandlers.ManageUsers
{
    public class GetUserDetailRequestHandler : IRequestHandler<GetUserDetailRequest, GetUserDetailResponse?>
    {
        private readonly UserManager<User> _userManager;
        private readonly IStorageService _storageService;
        private readonly ApplicationDbContext _db;

        public GetUserDetailRequestHandler(UserManager<User> userManager, IStorageService storageService, ApplicationDbContext db)
        {
            _userManager = userManager;
            _storageService = storageService;
            _db = db;
        }

        public async Task<GetUserDetailResponse?> Handle(GetUserDetailRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id);
            if (user == null)
            {
                return null;
            }

            // Fetch the user image associated with the user
            var userImage = await _db.Users
                .Include(u => u.Blob) // Ensure Blob navigation property is included
                .FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken); // Use user.Id

            string minioUrl = string.Empty;
            if (userImage != null && userImage.Blob != null && !string.IsNullOrEmpty(userImage.Blob.FilePath))
            {
                try
                {
                    minioUrl = await _storageService.GetPresignedUrlReadAsync(userImage.Blob.FilePath);
                }
                catch (Exception)
                {
                    minioUrl = "Error generating URL"; 
                }
            }

            return new GetUserDetailResponse
            {
                Id = user.Id,
                Email = user.Email,
                FamilyName = user.FamilyName,
                GivenName = user.GivenName,
                IsEnabled = user.IsEnabled,
                MinioUrl = minioUrl // Set the MiniUrl in the response
            };
        }
    }
}
