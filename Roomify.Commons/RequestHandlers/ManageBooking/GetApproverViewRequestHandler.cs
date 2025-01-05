using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Roomify.Contracts.RequestModels.ManageBooking;
using Roomify.Contracts.ResponseModels.ManageBooking;
using Roomify.Entities;
using Microsoft.EntityFrameworkCore;
using Roomify.Commons.Services;

namespace Roomify.Commons.RequestHandlers.ManageBooking
{
    public class GetApproverViewRequestHandler : IRequestHandler<GetApproverViewRequestModel, GetApproverViewResponseModel>
    {
        private readonly ApplicationDbContext _db;
        private readonly IStorageService _storageService;

        public GetApproverViewRequestHandler(ApplicationDbContext db, IStorageService storageService)
        {
            _db = db;
            _storageService = storageService;
        }

        public async Task<GetApproverViewResponseModel> Handle(GetApproverViewRequestModel request, CancellationToken cancellationToken)
        {
            // Base query to get approver tasks with no approvals (UpdatedAt is null)
            var query = _db.ApproverDetails
                .Where(a => a.AppproverUserId == request.ApproverId && a.UpdatedAt == null)
                .AsQueryable();

            // Apply search filters if provided
            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(a =>
                    _db.Users.Any(u => u.Id == a.AppproverUserId && u.GivenName.Contains(request.Search)) ||
                    _db.Bookings.Any(b => b.Id == a.BookingId && b.BookingDescription.Contains(request.Search)));
            }

            if (request.RoomId > 0)
            {
                query = query.Where(a => _db.Bookings.Any(b => b.Id == a.BookingId && b.RoomId == request.RoomId));
            }

            if (request.SessionId >  0)
            {
                query = query.Where(a => _db.SessionBookeds.Any(sb => sb.BookingId == a.BookingId && sb.SessionId == request.SessionId));
            }

            // Apply sorting based on the CreatedAt field (asc or desc)
            if (!string.IsNullOrEmpty(request.SortOrder) && request.SortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderByDescending(a => a.CreatedAt);
            }
            else
            {
                query = query.OrderBy(a => a.CreatedAt);
            }

            // Execute the query
            var approverTasks = await query.ToListAsync(cancellationToken);

            // Filter the tasks that should be displayed based on approval order logic
            var approverViewModels = new List<GetApproverViewmodel>();

            foreach (var approverTask in approverTasks)
            {
                // Check if the current approver has the first approval order
                if (approverTask.ApprovalOrder > 1)
                {
                    // Find the previous approval task for the same booking
                    var previousApproval = await _db.ApproverDetails
                        .Where(a => a.BookingId == approverTask.BookingId && a.ApprovalOrder == approverTask.ApprovalOrder - 1)
                        .FirstOrDefaultAsync(cancellationToken);

                    // Skip if the previous approval hasn't been approved yet
                    if (previousApproval == null || previousApproval.IsApproved == false)
                    {
                        continue;
                    }
                }

                // Retrieve booking, room, user, and session details
                var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.Id == approverTask.BookingId, cancellationToken);
                if (booking == null) continue;

                var room = await _db.Rooms.FirstOrDefaultAsync(r => r.RoomId == booking.RoomId, cancellationToken);
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == booking.UserId, cancellationToken);

                var sessionBookings = await _db.SessionBookeds
                    .Where(sb => sb.BookingId == booking.Id)
                    .ToListAsync(cancellationToken);

                var sessionList = sessionBookings.Select(sb => new SessionBookedList
                {
                    SessionId = sb.SessionId,
                    SessionName = _db.Sessions.FirstOrDefault(s => s.SessionId == sb.SessionId)?.Name ?? string.Empty,
                    StartTime = _db.Sessions.FirstOrDefault(s => s.SessionId == sb.SessionId).StartTime,
                    EndTime = _db.Sessions.FirstOrDefault(s => s.SessionId == sb.SessionId).EndTime
                }).ToList();

                // Build approver history for the booking
                var approvalHistory = await _db.ApproverDetails
                    .Where(ad => ad.BookingId == booking.Id)
                    .OrderBy(ad => ad.ApprovalOrder)
                    .ToListAsync(cancellationToken);

                var approverHistoryList = new List<ApproverHistoryList>();
                foreach (var history in approvalHistory)
                {
                    var statusId = (history.ApprovalOrder == approverTask.ApprovalOrder && history.UpdatedAt == null) ? 1 :
                                    (history.ApprovalOrder < approverTask.ApprovalOrder) ? 2 : 1;

                    // Get the approver's user information and BlobId
                    var approverUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == history.AppproverUserId, cancellationToken);
                    string approverMinioUrl = string.Empty;

                    if (approverUser != null && approverUser.BlobId.HasValue) // Ensure BlobId exists for the user
                    {
                        var approverBlob = await _db.Blobs
                            .Where(b => b.Id == approverUser.BlobId.Value) // Use the BlobId from the user record
                            .FirstOrDefaultAsync(cancellationToken);

                        if (approverBlob != null && !string.IsNullOrEmpty(approverBlob.FilePath))
                        {
                            try
                            {
                                // Generate the presigned URL for the approver's Blob
                                approverMinioUrl = await _storageService.GetPresignedUrlReadAsync(approverBlob.FilePath);
                            }
                            catch (Exception)
                            {
                                approverMinioUrl = "Error generating URL"; // Handle error if necessary
                            }
                        }
                    }

                    approverHistoryList.Add(new ApproverHistoryList
                    {
                        ApproverUserId = history.AppproverUserId,
                        ApproverUserName = approverUser?.GivenName ?? string.Empty,
                        StatusId = statusId,
                        StatusName = _db.Statuses.FirstOrDefault(s => s.StatusId == statusId)?.Name ?? string.Empty,
                        ApproverMinioUrl = approverMinioUrl, // Add the generated Minio URL for the approver
                        ApprovedAt = history.UpdatedAt.HasValue ? history.UpdatedAt.Value.DateTime : DateTime.MinValue
                    });
                }


                var Evidence = await _db.Bookings
                .Include(u => u.Blob) // Ensure Blob navigation property is included
                .FirstOrDefaultAsync(u => u.Id == booking.Id, cancellationToken); // Use user.Id

                string minioUrl = string.Empty;
                if (Evidence != null && Evidence.Blob != null && !string.IsNullOrEmpty(Evidence.Blob.FilePath))
                {
                    try
                    {
                        minioUrl = await _storageService.GetPresignedUrlReadAsync(Evidence.Blob.FilePath);
                    }
                    catch (Exception)
                    {
                        minioUrl = "Error generating URL"; 
                    }
                }
                var roomBlob = await _db.Blobs
                    .Where(b => b.Id == room.BlobId)
                    .FirstOrDefaultAsync(cancellationToken);                    
                

                string roomMinioUrl = string.Empty;
                if (roomBlob != null && !string.IsNullOrEmpty(roomBlob.FilePath))
                {
                    try
                    {
                        // Step 3: Generate the presigned URL
                        roomMinioUrl = await _storageService.GetPresignedUrlReadAsync(roomBlob.FilePath);
                    }
                    catch (Exception)
                    {
                        roomMinioUrl = "Error generating URL"; // Handle error if necessary
                    }
                }
                string bookerMinioUrl = string.Empty;
                var bookerBlob = await _db.Blobs
                    .Where(b => b.Id == user.BlobId)
                    .FirstOrDefaultAsync(cancellationToken);                    
                

                if (bookerBlob != null && !string.IsNullOrEmpty(bookerBlob.FilePath))
                {
                    try
                    {
                        // Step 3: Generate the presigned URL
                        bookerMinioUrl = await _storageService.GetPresignedUrlReadAsync(bookerBlob.FilePath);
                    }
                    catch (Exception)
                    {
                        bookerMinioUrl = "Error generating URL"; // Handle error if necessary
                    }
                }

                var userRole = await _db.Roles.Where(s=>s.Id == booking.RoleId).FirstOrDefaultAsync();

                // Build the approver view model for this task
                var approverViewModel = new GetApproverViewmodel
                {
                    BookingId = booking.Id,
                    BookingDescription = booking.BookingDescription,
                    Name = user?.GivenName ?? string.Empty,
                    RoomId = room?.RoomId ?? 0,
                    RoomName = room?.Name ?? string.Empty,
                    SessionList = sessionList,
                    ApproverHistory = approverHistoryList,
                    MinioUrl = minioUrl,
                    IsCanceled = booking.IsCanceled,
                    RoomMinioUrl = roomMinioUrl, 
                    BookingOrganizationName = booking.OrganizationName,
                    BookingInstitutionalId = booking.InstitutionalId,
                    UserRole = userRole.Name,
                    BookerMinioUrl = bookerMinioUrl
                };

                approverViewModels.Add(approverViewModel);
            }

            // Prepare the final response model
            var response = new GetApproverViewResponseModel
            {
                TotalData = approverViewModels.Count,
                ApproverViewModel = approverViewModels
            };

            return response;
        }

    }
}
