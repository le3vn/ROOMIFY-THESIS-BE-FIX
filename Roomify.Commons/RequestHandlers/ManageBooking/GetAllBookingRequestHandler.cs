using System;
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
    public class GetAllBookingRequestHandler : IRequestHandler<GetAllBookingRequestModel, GetAllBookingResponseModel>
    {
        private readonly ApplicationDbContext _db;
        private readonly IStorageService _storageService;

        public GetAllBookingRequestHandler(ApplicationDbContext db, IStorageService storageService)
        {
            _db = db;
            _storageService = storageService;
        }

        public async Task<GetAllBookingResponseModel> Handle(GetAllBookingRequestModel request, CancellationToken cancellationToken)
        {
            // Start building the base query to fetch all bookings
            var query = _db.Bookings.AsQueryable();

            // Apply filters if provided in the request
            if (request.BuildingId.HasValue)
            {
                // Filter by BuildingId (if provided)
                query = query.Where(b => _db.Rooms.Any(r => r.RoomId == b.RoomId && r.BuildingId == request.BuildingId.Value));
            }

            if (request.RoomId.HasValue)
            {
                // Filter by RoomId (if provided)
                query = query.Where(b => b.RoomId == request.RoomId.Value);
            }

            if (!string.IsNullOrEmpty(request.Search))
            {
                // Filter by Search term on Booking Description or User's Name
                query = query.Where(b =>
                    b.BookingDescription.Contains(request.Search) ||
                    _db.Users.Any(u => u.Id == b.UserId && u.GivenName.Contains(request.Search)));
            }

            // Fetch bookings from the database with the applied filters
            var bookings = await query.ToListAsync(cancellationToken);

            if (bookings == null || bookings.Count == 0)
            {
                return new GetAllBookingResponseModel { TotalData = 0, ApproverViewModel = new List<GetAllApproverModel>() };
            }

            // List to store the response model for each booking
            var approverViewModels = new List<GetAllApproverModel>();

            foreach (var booking in bookings)
            {
                // Fetch related data: Room, User (Booker), Sessions, Approver History
                var room = await _db.Rooms.FirstOrDefaultAsync(r => r.RoomId == booking.RoomId, cancellationToken);
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == booking.UserId, cancellationToken);

                var sessionBookings = await _db.SessionBookeds
                    .Where(sb => sb.BookingId == booking.Id)
                    .ToListAsync(cancellationToken);

                var sessionList = sessionBookings.Select(sb => new AllSessionList
                {
                    SessionId = sb.SessionId,
                    SessionName = _db.Sessions.FirstOrDefault(s => s.SessionId == sb.SessionId)?.Name ?? string.Empty,
                    StartTime = _db.Sessions.FirstOrDefault(s => s.SessionId == sb.SessionId).StartTime,
                    EndTime = _db.Sessions.FirstOrDefault(s => s.SessionId == sb.SessionId).EndTime
                }).ToList();

                // Fetch approver history for the booking
                var approverHistory = await _db.ApproverDetails
                    .Where(ad => ad.BookingId == booking.Id)
                    .OrderBy(ad => ad.ApprovalOrder)
                    .ToListAsync(cancellationToken);

                var approverHistoryList = new List<AllApproverList>();
                foreach (var history in approverHistory)
                {
                    var statusId = (history.UpdatedAt == null) ? 1 :
                                    (history.IsApproved) ? 2 : 3; // Example logic for status based on approval

                    var approverUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == history.AppproverUserId, cancellationToken);
                    var approverBlob = await _db.Blobs
                                        .Where(b => b.Id == approverUser.BlobId)
                                        .FirstOrDefaultAsync(cancellationToken);

                    var approverMinioUrl = string.Empty;
                    if (approverBlob != null && !string.IsNullOrEmpty(approverBlob.FilePath))
                    {
                        try
                        {
                            approverMinioUrl = await _storageService.GetPresignedUrlReadAsync(approverBlob.FilePath);
                        }
                        catch (Exception)
                        {
                            approverMinioUrl = "Error generating URL"; 
                        }
                    }

                    approverHistoryList.Add(new AllApproverList
                    {
                        ApproverUserId = history.AppproverUserId,
                        ApproverUserName = approverUser?.GivenName ?? string.Empty,
                        StatusId = statusId,
                        StatusName = _db.Statuses.FirstOrDefault(s => s.StatusId == statusId)?.Name ?? string.Empty,
                        ApproverMinioUrl = approverMinioUrl,
                        ApprovedAt = history.UpdatedAt.HasValue ? history.UpdatedAt.Value.DateTime : DateTime.MinValue
                    });
                }

                // Minio URL for the room (RoomBlob)
                string roomMinioUrl = string.Empty;
                    var roomBlob = await _db.Blobs
                                            .Where(b => b.Id == room.BlobId)
                                            .FirstOrDefaultAsync(cancellationToken);

                    if (roomBlob != null && !string.IsNullOrEmpty(roomBlob.FilePath))
                    {
                        try
                        {
                            roomMinioUrl = await _storageService.GetPresignedUrlReadAsync(roomBlob.FilePath);
                        }
                        catch (Exception)
                        {
                            roomMinioUrl = "Error generating URL"; // Handle error if necessary
                        }
                    }

                // Fetch user (Booker) related data
                string bookerMinioUrl = string.Empty;
                if (user != null && user.BlobId.HasValue)
                {
                    var bookerBlob = await _db.Blobs
                                               .Where(b => b.Id == user.BlobId.Value)
                                               .FirstOrDefaultAsync(cancellationToken);

                    if (bookerBlob != null && !string.IsNullOrEmpty(bookerBlob.FilePath))
                    {
                        try
                        {
                            bookerMinioUrl = await _storageService.GetPresignedUrlReadAsync(bookerBlob.FilePath);
                        }
                        catch (Exception)
                        {
                            bookerMinioUrl = "Error generating URL"; // Handle error if necessary
                        }
                    }
                }

                // Build the response model for this booking
                var bookingViewModel = new GetAllApproverModel
                {
                    BookingId = booking.Id,
                    BookingDescription = booking.BookingDescription,
                    Name = user?.GivenName ?? string.Empty,
                    RoomId = room?.RoomId ?? 0,
                    RoomName = room?.Name ?? string.Empty,
                    RoomMinioUrl = roomMinioUrl,  // Add the generated Room Minio URL
                    SessionList = sessionList,
                    ApproverHistory = approverHistoryList,
                    IsCanceled = booking.IsCanceled,
                    BookingOrganizationName = booking.OrganizationName,
                    BookingInstitutionalId = booking.InstitutionalId,
                    UserRole = _db.Roles.FirstOrDefault(r => r.Id == booking.RoleId)?.Name ?? string.Empty,
                    BookerMinioUrl = bookerMinioUrl  // Add the Booker Minio URL
                };

                // Add the model to the result list
                approverViewModels.Add(bookingViewModel);
            }

            // Prepare the response
            var response = new GetAllBookingResponseModel
            {
                TotalData = approverViewModels.Count,
                ApproverViewModel = approverViewModels
            };

            return response;
        }
    }
}
