using Roomify.Contracts.RequestModels.ManageUsers;
using Roomify.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Roomify.Commons.Services;
using System.IO;
using Roomify.Commons.Constants;

namespace Roomify.RequestHandlers.ManageUsers
{
    public class CreateUserRequestHandler : IRequestHandler<CreateUserRequest, string>
    {
        private readonly UserManager<User> _userManager;
        private readonly IStorageService _storageService;
        private readonly ApplicationDbContext _db; 

        public CreateUserRequestHandler(UserManager<User> userManager, IStorageService storageService, ApplicationDbContext db)
        {
            _userManager = userManager;
            _storageService = storageService;
            _db = db; 
        }

        public async Task<string> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            var blobId = Guid.NewGuid();
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                GivenName = request.GivenName,
                FamilyName = request.FamilyName,
                IsEnabled = true,
                BlobId = request.ProfilePicture != null ? blobId : (Guid?)null,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
            };

            if( request.ProfilePicture != null )
            {
                var userImage = new Blob
                {
                    Id = blobId,
                    FileName = request.ProfilePicture.FileName,
                    FilePath = $"{BlobPath.UserImage}/{request.ProfilePicture.FileName}",
                    ContentType = request.ProfilePicture.ContentType,
                    CreatedAt = DateTime.UtcNow
                };

                //UploadFileAsync requires Stream parameter
                using (var stream = new MemoryStream())
                {
                    await request.ProfilePicture.CopyToAsync(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    await _storageService.UploadFileAsync(userImage.FilePath, stream);
                }

                _db.Blobs.Add(userImage);

            }

            await _db.SaveChangesAsync();
            await _userManager.SetEmailAsync(user, request.Email);
            await _userManager.SetUserNameAsync(user, request.Email);
            await _userManager.CreateAsync(user, request.Password);

            return user.Id;
        }
    }
}
