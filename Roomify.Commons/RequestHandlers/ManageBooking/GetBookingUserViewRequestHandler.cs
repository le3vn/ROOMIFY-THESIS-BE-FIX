using System;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Roomify.Commons.Services;
using Roomify.Contracts.RequestModels.ManageBooking;
using Roomify.Contracts.ResponseModels.ManageBooking;
using Roomify.Entities;

namespace Roomify.Commons.RequestHandlers.ManageBooking;

public class GetBookingUserViewRequestHandler : IRequestHandler<GetBookingUserViewRequestModel, GetBookingUserViewResponseModel>
{
    private readonly ApplicationDbContext _db;
    private readonly IStorageService _storageService;
    public GetBookingUserViewRequestHandler(ApplicationDbContext db, IStorageService storageService)
    {
        _db = db;
        _storageService = storageService;
    }

    public async Task<GetBookingUserViewResponseModel> Handle(GetBookingUserViewRequestModel request, CancellationToken cancellationToken)
{
    // Step 1: Get RoleId based on the RoleName
    var role = await _db.Roles
                         .Where(r => r.Name == request.RoleName)
                         .FirstOrDefaultAsync(cancellationToken);

    if (role == null)
    {
        return new GetBookingUserViewResponseModel { TotalData = 0, UserBookings = new List<UserBookingModel>() };
    }

    // Step 2: Get bookings for the user based on userId and roleId, and filter by cancellation status
    IQueryable<Booking> bookingsQuery = _db.Bookings
                                            .Where(b => b.UserId == request.UserId && b.RoleId == role.Id);

    // If the IsCanceled flag is true, filter by statusId = 4 (Canceled)
    if (request.IsCanceled)
    {
        bookingsQuery = bookingsQuery.Where(b => b.StatusId == 4);
    }
    else
    {
        // If IsCanceled is false, filter by all statuses except Canceled (StatusId != 4)
        bookingsQuery = bookingsQuery.Where(b => b.StatusId != 4);
    }

    var bookings = await bookingsQuery.ToListAsync(cancellationToken);

    if (bookings == null || bookings.Count == 0)
    {
        return new GetBookingUserViewResponseModel { TotalData = 0, UserBookings = new List<UserBookingModel>() };
    }

    // Prepare response model
    var response = new GetBookingUserViewResponseModel();
    var userBookings = new List<UserBookingModel>();

    foreach (var booking in bookings)
    {
        // Step 3: Get room details (RoomName and RoomMinioUrl)
        var room = await _db.Rooms
                            .Where(r => r.RoomId == booking.RoomId)
                            .FirstOrDefaultAsync(cancellationToken);
        var qrBlobId = await _db.QRCodes
        .Where(s => s.BookingId == booking.Id)
        .Select(s => s.BlobId)
        .FirstOrDefaultAsync(cancellationToken);

        // Fetch the Blob using the BlobId
        var qrBlob = await _db.Blobs
            .Where(b => b.Id == qrBlobId)
            .FirstOrDefaultAsync(cancellationToken);

        string CheckInPlace = string.Empty;

        if(request.RoleName == "Student"){
            CheckInPlace = "SLC Office";
        }
        else if (request.RoleName == "StudentOrganization"){
            if(room?.RoomType == 1){
                CheckInPlace = "SLC Office";
            }
            else if(room?.RoomType == 2){
                CheckInPlace = "LSC Office";
            }
            else if (room?.RoomType == 3){
                CheckInPlace = "BM Office";
            }
        }
        else if(request.RoleName == "Lecturer"){
            if(room?.RoomType == 1){
                CheckInPlace = "SLC Office";
            }
            else if(room?.RoomType == 2){
                CheckInPlace = "LSC Office";
            }
            else if (room?.RoomType == 3){
                CheckInPlace = "BM Office";
            }
        }
        else if(request.RoleName == "Staff"){
            if(room?.RoomType == 1){
                CheckInPlace = "SLC Office";
            }
            else if(room?.RoomType == 2){
                CheckInPlace = "LSC Office";
            }
            else if (room?.RoomType == 3){
                CheckInPlace = "BM Office";
            }
        }

        string qrMinioUrl = null;

        if (qrBlob != null && !string.IsNullOrEmpty(qrBlob.FilePath))
        {
            try
            {
                // Generate the presigned URL
                qrMinioUrl = await _storageService.GetPresignedUrlReadAsync(qrBlob.FilePath);
            }
            catch (Exception)
            {
                qrMinioUrl = "Error generating URL"; // Handle error if necessary
            }
        }

        var roomModel = new UserBookingModel
        {
            BookingId = booking.Id.ToString(),
            BookingDescription = booking.BookingDescription,
            RoomId = booking.RoomId,
            RoomName = room?.Name ?? "Unknown",
            QrMinioUrl = qrMinioUrl,
            CheckInPlace = CheckInPlace
        };

        // Fetch Minio URL for the room
        var blob = await _db.Blobs.FirstOrDefaultAsync(b => b.Id == room.BlobId, cancellationToken);
        if (blob != null && !string.IsNullOrEmpty(blob.FilePath))
        {
            try
            {
                roomModel.RoomMinioUrl = await _storageService.GetPresignedUrlReadAsync(blob.FilePath);
            }
            catch (Exception)
            {
                roomModel.RoomMinioUrl = "Error generating URL"; // Handle error as needed
            }
        }
        else
        {
            roomModel.RoomMinioUrl = ""; // Or set a default value if no blob
        }

        // Step 4: Get session bookings for this booking
        var sessionBookings = await _db.SessionBookeds
            .Where(sb => sb.BookingId == booking.Id)
            .ToListAsync(cancellationToken);

        var sessionList = new List<SessionBookingListModel>();
        
        foreach (var sessionBooking in sessionBookings)
        {
            var session = await _db.Sessions
                .Where(s => s.SessionId == sessionBooking.SessionId)
                .FirstOrDefaultAsync(cancellationToken);

            if (session != null)
            {
                var sessionModel = new SessionBookingListModel
                {
                    SessionId = session.SessionId,
                    SessionName = session.Name,
                    StartTime = session.StartTime,
                    EndTime = session.EndTime
                };
                sessionList.Add(sessionModel);
            }
        }

        roomModel.SessionBookingList = sessionList; // Add session bookings to room model

        var approvalHistories = new List<ApprovalHistory>();

        // Step 5: Get approval history for each booking
        var approvers = await _db.ApproverDetails
                                 .Where(ad => ad.BookingId == booking.Id)
                                 .OrderBy(ad => ad.ApprovalOrder)  // Order approvers based on ApproverOrder
                                 .ToListAsync(cancellationToken);

        foreach (var approver in approvers)
        {
            // Determine approval status (Pending, Approved, Rejected)
            int statusId;
            string statusName;

            if (!approver.IsApproved && approver.UpdatedAt == null)
            {
                statusId = 1; // Pending
            }
            else if (!approver.IsApproved && approver.UpdatedAt != null && !booking.IsCanceled)
            {
                statusId = 3; // Rejected
            }
            else if (!approver.IsApproved && approver.UpdatedAt != null && booking.IsCanceled)
            {
                statusId = 4; // Rejected (due to cancellation)
            }
            else if (approver.IsApproved && approver.UpdatedAt != null)
            {
                statusId = 2; // Approved
            }
            else
            {
                statusId = 0; // Default fallback, handle if necessary
            }

            // Get approval status name from Statuses table
            var approvalStatus = await _db.Statuses
                                           .Where(s => s.StatusId == statusId)
                                           .FirstOrDefaultAsync(cancellationToken);

            statusName = approvalStatus?.Name ?? "Unknown Status";

            // Get approver details (name and minio URL)
            var approverUser = await _db.Users
                                        .Where(u => u.Id == approver.AppproverUserId)
                                        .FirstOrDefaultAsync(cancellationToken);

            var approverBlob = await _db.Blobs
                                        .Where(b => b.Id == approverUser.BlobId)
                                        .FirstOrDefaultAsync(cancellationToken);

            var approvalHistory = new ApprovalHistory
            {
                ApprovalUserId = approver.AppproverUserId,
                ApprovalName = approverUser?.GivenName,
                ApprovalStatusId = statusId,
                ApprovalStatus = statusName,
                ApprovedAt = approver.UpdatedAt.HasValue ? approver.UpdatedAt.Value.DateTime : DateTime.MinValue
            };

            // Fetch the Minio URL for the approver
            if (approverBlob != null && !string.IsNullOrEmpty(approverBlob.FilePath))
            {
                try
                {
                    approvalHistory.ApprovalMinioUrl = await _storageService.GetPresignedUrlReadAsync(approverBlob.FilePath);
                }
                catch (Exception)
                {
                    approvalHistory.ApprovalMinioUrl = "Error generating URL"; // Handle error as needed
                }
            }
            else
            {
                approvalHistory.ApprovalMinioUrl = ""; // Or set a default value if no blob
            }

            approvalHistories.Add(approvalHistory);
        }

        roomModel.ApprovalHistories = approvalHistories;
        userBookings.Add(roomModel);
    }

    response.TotalData = userBookings.Count;
    response.UserBookings = userBookings;

    return response;
}




}
