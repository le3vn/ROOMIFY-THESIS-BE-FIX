using Roomify.Contracts.RequestModels.ManageUsers;
using Roomify.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Roomify.Commons.Constants;
using Roomify.Commons.Services;

namespace Roomify.RequestHandlers.ManageUsers
{
    public class UpdateUserRequestHandler : IRequestHandler<UpdateUserRequest>
    {
        private readonly UserManager<User> _userManager;
        private readonly IStorageService _storageService;
        private readonly ApplicationDbContext _db; 

        public UpdateUserRequestHandler(UserManager<User> userManager, IStorageService storageService, ApplicationDbContext db)
        {
             _userManager = userManager;
            _storageService = storageService;
            _db = db; 
        }

        public async Task Handle(UpdateUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id);

            var blobId = Guid.NewGuid();
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

            // Assume that user won't be null.
            user!.GivenName = request.GivenName;
            user.FamilyName = request.FamilyName;
            user.IsEnabled = request.IsEnabled;
            user.BlobId = blobId;

            await _db.SaveChangesAsync();

            await _userManager.UpdateAsync(user);
            await _userManager.SetUserNameAsync(user, request.Email);
            await _userManager.SetEmailAsync(user, request.Email);

            if (request.Password.HasValue())
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token, request.Password);
            }
        }
    }
}
