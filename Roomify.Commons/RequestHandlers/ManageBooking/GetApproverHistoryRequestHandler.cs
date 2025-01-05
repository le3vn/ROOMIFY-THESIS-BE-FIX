using System;
using System.Collections.Generic;
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
    public class GetApproverHistoryRequestHandler : IRequestHandler<GetApproverHistoryRequestModel, GetApproverHistoryResponseModel>
    {
        private readonly ApplicationDbContext _db;
        private readonly IStorageService _storageService;

        public GetApproverHistoryRequestHandler(ApplicationDbContext db, IStorageService storageService)
        {
            _db = db;
            _storageService = storageService;
        }

public async Task<GetApproverHistoryResponseModel> Handle(GetApproverHistoryRequestModel request, CancellationToken cancellationToken)
{
    // Initialize response model
    var response = new GetApproverHistoryResponseModel();

    // 1. Get all approver history entries for the user
   var approverHistories = await _db.ApproverHistories
    .Where(ah => ah.CreatedBy == request.UserId && 
                ah.UserId == ah.CreatedBy && 
                (
                    (request.IsApproved && ah.StatusId == 2) || // If approved, StatusId should be 2
                    (!request.IsApproved && ah.StatusId == 3)  // If rejected, StatusId should be 3
                ))
    .ToListAsync(cancellationToken);

    if (approverHistories == null || !approverHistories.Any())
    {
        return response;
    }

    // 2. Get the unique BookingIds
    var bookingIds = approverHistories.Select(ah => ah.BookingId).Distinct().ToList();

    // 3. Fetch all related bookings from the database
    var bookings = await _db.Bookings
        .Where(b => bookingIds.Contains(b.Id))
        .ToListAsync(cancellationToken);

    // 4. Fetch related rooms for these bookings
    var rooms = await _db.Rooms
        .Where(r => bookings.Select(b => b.RoomId).Contains(r.RoomId))
        .ToListAsync(cancellationToken);

    // 5. Fetch user roles
    var roleIds = bookings.Select(b => b.RoleId).Distinct().ToList();
    var roles = await _db.Roles
        .Where(r => roleIds.Contains(r.Id))
        .ToListAsync(cancellationToken);

    // 6. Fetch sessions for these bookings
    var sessionBookings = await _db.SessionBookeds
        .Where(sb => bookingIds.Contains(sb.BookingId))
        .ToListAsync(cancellationToken);

    var sessionIds = sessionBookings.Select(sb => sb.SessionId).Distinct().ToList();
    var sessions = await _db.Sessions
        .Where(s => sessionIds.Contains(s.SessionId))
        .ToListAsync(cancellationToken);

    // 7. Fetch reject messages
    var rejectMessages = await _db.RejectMessages
        .Where(rm => bookingIds.Contains(rm.BookingId))
        .ToListAsync(cancellationToken);

    // 8. Fetch statuses
    var statusIds = approverHistories.Select(ah => ah.StatusId).Distinct().ToList();
    var statuses = await _db.Statuses
        .Where(s => statusIds.Contains(s.StatusId))
        .ToListAsync(cancellationToken);

    // 9. Fetch blobs for evidence, room, and user
    var blobIds = bookings.Select(b => b.BlobId).Distinct().ToList();
    var blobs = await _db.Blobs
        .Where(b => blobIds.Contains(b.Id))
        .ToListAsync(cancellationToken);

    // Fetch Booker username before processing the response (this needs to be done for each booking)
    var bookerUsers = await _db.Users
        .Where(u => bookings.Select(b => b.UserId).Contains(u.Id))
        .ToListAsync(cancellationToken);

    // 10. Build the response
    foreach (var bookingId in bookingIds)
    {
        var booking = bookings.FirstOrDefault(b => b.Id == bookingId);
        var room = rooms.FirstOrDefault(r => r.RoomId == booking?.RoomId);
        var role = roles.FirstOrDefault(r => r.Id == booking?.RoleId);
        var sessionBooked = sessionBookings.Where(sb => sb.BookingId == bookingId).ToList();
        var approverHistory = approverHistories.Where(ah => ah.BookingId == bookingId).ToList();

        var sessionList = new List<SessionBookedHistoryList>();
        foreach (var sb in sessionBooked)
        {
            var session = sessions.FirstOrDefault(s => s.SessionId == sb.SessionId);
            if (session != null)
            {
                sessionList.Add(new SessionBookedHistoryList
                {
                    SessionId = sb.SessionId,
                    SessionName = session.Name,
                    StartTime = session.StartTime,
                    EndTime = session.EndTime
                });
            }
        }

     var approverHistoryList = approverHistory
    .OrderBy(ah => ah.ApprovalOrder)  // Order by ApprovalOrder in ascending order (use `OrderByDescending` for descending)
    .Select(ah =>
    {
        // Fetch approver username from Users table
        var approverUser = _db.Users
            .FirstOrDefaultAsync(u => u.Id == ah.UserId, cancellationToken).Result; // Use `Result` to synchronously fetch the user
        var approverBlob = approverUser != null
            ? _db.Blobs.FirstOrDefaultAsync(b => b.Id == approverUser.BlobId, cancellationToken).Result // Synchronously fetch the blob
            : null;

        string approverMinioUrl = string.Empty;
        if (approverBlob != null && !string.IsNullOrEmpty(approverBlob.FilePath))
        {
            approverMinioUrl = _storageService.GetPresignedUrlReadAsync(approverBlob.FilePath).Result; // Synchronously generate the URL
        }

        var status = statuses.FirstOrDefault(s => s.StatusId == ah.StatusId);

        return new ApproverListHistory
        {
            ApproverUserId = ah.UserId,
            ApproverUserName = approverUser?.UserName ?? string.Empty, // Using the approver's username
            ApproverMinioUrl = approverMinioUrl,
            StatusId = ah.StatusId,
            StatusName = status?.Name ?? string.Empty,
            ApprovedAt = ah.CreatedAt.DateTime
        };
    })
    .ToList();
        

        // Fetch Booker's username for this booking
        var bookerUser = bookerUsers.FirstOrDefault(u => u.Id == booking?.UserId);
        var bookerUserName = bookerUser?.GivenName ?? string.Empty;

        var bookingResponse = new GetApproverHistoryViewmodel
        {
            BookingId = bookingId,
            BookingDescription = booking?.BookingDescription ?? string.Empty,
            BookerName = bookerUserName, // Set Name as the Booker's username
            RoomId = booking?.RoomId ?? 0,
            RoomName = room?.Name ?? string.Empty,
            EvidenceMinioUrl = await GetBlobUrlAsync(booking?.BlobId),
            IsCanceled = booking?.IsCanceled ?? false,
            RoomMinioUrl = await GetRoomBlobUrlAsync(room?.BlobId),
            BookingOrganizationName = booking?.OrganizationName,
            BookingInstitutionalId = booking?.InstitutionalId ?? string.Empty,
            BookerUserRole = role?.Name ?? string.Empty,
            BookerMinioUrl = await GetUserBlobUrlAsync(booking?.UserId),
            SessionList = sessionList,
            ApproverHistory = approverHistoryList,
        };

        response.ApproverHistoryViewModel.Add(bookingResponse);
    }

    response.TotalData = response.ApproverHistoryViewModel.Count;

    return response;
}


// Helper methods to get the presigned URL for each blob
private async Task<string> GetBlobUrlAsync(Guid? blobId)
{
    if (blobId == null) return string.Empty;

    var blob = await _db.Blobs
        .FirstOrDefaultAsync(b => b.Id == blobId.Value);

    if (blob == null || string.IsNullOrEmpty(blob.FilePath)) return string.Empty;

    try
    {
        return await _storageService.GetPresignedUrlReadAsync(blob.FilePath);
    }
    catch (Exception)
    {
        return "Error generating URL"; // Handle error if necessary
    }
}

private async Task<string> GetRoomBlobUrlAsync(Guid? blobId)
{
    return await GetBlobUrlAsync(blobId);
}

private async Task<string> GetUserBlobUrlAsync(string userId)
{
    if (string.IsNullOrEmpty(userId)) return string.Empty;

    var user = await _db.Users
        .FirstOrDefaultAsync(u => u.Id == userId);

    if (user == null) return string.Empty;

    return await GetBlobUrlAsync(user.BlobId);
}

    }
}
