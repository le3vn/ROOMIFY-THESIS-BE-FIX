using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Roomify.Commons.Services;
using Roomify.Contracts.RequestModels.ManageBooking;
using Roomify.Contracts.ResponseModels.ManageBooking;
using Roomify.Entities;

namespace Roomify.Commons.RequestHandlers.ManageBooking
{
    public class GetRejectMessageRequestHandler : IRequestHandler<GetRejectMessageRequestModel, GetRejectmessageResponseModel>
    {
        private readonly ApplicationDbContext _db;
        private readonly IStorageService _storageService;

        public GetRejectMessageRequestHandler(ApplicationDbContext db, IStorageService storageService)
        {
            _db = db;
            _storageService = storageService;
        }

        public async Task<GetRejectmessageResponseModel> Handle(GetRejectMessageRequestModel request, CancellationToken cancellationToken)
        {
            // 1. Query RejectMessages table by BookingId
            var rejectMessageEntity = await _db.RejectMessages
                .Where(rm => rm.BookingId == request.BookingId)
                .FirstOrDefaultAsync(cancellationToken);

            if (rejectMessageEntity == null)
            {
                // If no reject message found, return null or handle as needed
                return null;
            }

            // 2. Get the user (creator) information from the Users table using CreatedBy ID
            var creator = await _db.Users
                .Where(u => u.Id == rejectMessageEntity.CreatedBy)
                .FirstOrDefaultAsync(cancellationToken);

            if (creator == null)
            {
                // If the creator is not found, handle accordingly (e.g., return null or throw exception)
                return null;
            }

            // 3. Get the creator's blob information
            string creatorBlobUrl = string.Empty; // Default if no blob is found
            var creatorBlob = await _db.Blobs
                .Where(b => b.Id == creator.BlobId)
                .FirstOrDefaultAsync(cancellationToken);

            if (creatorBlob != null && !string.IsNullOrEmpty(creatorBlob.FilePath))
            {
                try
                {
                    creatorBlobUrl = await _storageService.GetPresignedUrlReadAsync(creatorBlob.FilePath);
                }
                catch
                {
                    creatorBlobUrl = "Error generating URL"; // Handle error as needed
                }
            }

            // 4. Prepare the response
            var response = new GetRejectmessageResponseModel
            {
                RejectMessage = rejectMessageEntity.Message, // Assuming the message field in RejectMessages table is "Message"
                CreatedBy = creator.GivenName, // Assuming the User entity has a "GivenName" property
                CreatedAt = rejectMessageEntity.CreatedAt.DateTime,
                MinioUrl = creatorBlobUrl // Include the URL in the response
            };

            return response;
        }
    }
}
