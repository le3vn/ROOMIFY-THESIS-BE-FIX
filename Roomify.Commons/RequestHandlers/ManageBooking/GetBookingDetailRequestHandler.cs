using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Roomify.Commons.Services;
using Roomify.Contracts.RequestModels.ManageBooking;
using Roomify.Contracts.ResponseModels.ManageBooking;
using Roomify.Entities;
using MediatR;

namespace Roomify.Commons.RequestHandlers.ManageBooking
{
    public class GetBookingDetailRequestHandler : IRequestHandler<GetBookingDetailRequestModel, GetBookingDetailResponseModel>
    {
        private readonly ApplicationDbContext _db;
        private readonly IStorageService _storageService;

        public GetBookingDetailRequestHandler(ApplicationDbContext db, IStorageService storageService)
        {
            _db = db;
            _storageService = storageService;
        }

        public async Task<GetBookingDetailResponseModel> Handle(GetBookingDetailRequestModel request, CancellationToken cancellationToken)
{
    var booking = await _db.Bookings
        .Where(b => b.Id.ToString() == request.BookingId)
        .Select(b => new
        {
            b.Id,
            b.BookingDescription,
            b.UserId,
            b.RoleId,
            b.BlobId,
            b.OrganizationName,
            b.InstitutionalId,
            b.IsCanceled,
            b.RoomId,
            b.BookingDate
        })
        .FirstOrDefaultAsync(cancellationToken);

    if (booking == null)
        throw new Exception("Booking not found");

    var user = await _db.Users.FindAsync(booking.UserId);
    var room = await _db.Rooms.FindAsync(booking.RoomId);
    var role = await _db.Roles.FindAsync(booking.RoleId);
    string minioUrl = string.Empty;
    if (booking.BlobId.HasValue)
    {
        var blob = await _db.Blobs.FindAsync(booking.BlobId.Value);
        if (blob != null && !string.IsNullOrEmpty(blob.FilePath))
        {
            try
            {
                minioUrl = await _storageService.GetPresignedUrlReadAsync(blob.FilePath);
            }
            catch (Exception)
            {
                minioUrl = "Error generating URL"; // Handle error if necessary
            }
        }
    }

    string userBlobMinioUrl = string.Empty;
    if (user != null && user.BlobId.HasValue)
    {
        var userBlob = await _db.Blobs.FindAsync(user.BlobId.Value);
        if (userBlob != null && !string.IsNullOrEmpty(userBlob.FilePath))
        {
            try
            {
                userBlobMinioUrl = await _storageService.GetPresignedUrlReadAsync(userBlob.FilePath);
            }
            catch (Exception)
            {
                userBlobMinioUrl = "Error generating URL"; // Handle error if necessary
            }
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

    // Fetch approver details and order them by the approval timestamp
    var approverDetails = await _db.ApproverDetails
        .Where(a => a.BookingId == booking.Id)
        .OrderBy(a => a.UpdatedAt) // Assuming UpdatedAt determines the approval order
        .ToListAsync(cancellationToken);

    var approverHistory = new List<ApproverDetaillist>();

    foreach (var approver in approverDetails)
    {
        var approverUser = await _db.Users.FindAsync(approver.AppproverUserId);
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

        int statusId;
        if (!approver.IsApproved && approver.UpdatedAt == null)
        {
            statusId = 1;
        }
        else if (approver.IsApproved)
        {
            statusId = 2;
        }
        else
        {
            statusId = 3;
        }

        var statusName = await _db.Statuses
            .Where(s => s.StatusId == statusId)
            .Select(s => s.Name)
            .FirstOrDefaultAsync(cancellationToken);

        approverHistory.Add(new ApproverDetaillist
        {
            ApproverUserId = approver.AppproverUserId,
            ApproverUserName = approverUser?.GivenName ?? string.Empty,
            ApproverMinioUrl = approverMinioUrl,
            StatusId = statusId,
            StatusName = statusName ?? string.Empty,
            ApprovedAt = approver.UpdatedAt.HasValue ? approver.UpdatedAt.Value.DateTime : DateTime.MinValue
        });
    }

    // Fetch session and equipment bookings as previously done
    var sessionBookings = await _db.SessionBookeds
        .Where(sb => sb.BookingId == booking.Id)
        .ToListAsync(cancellationToken);

    var sessionList = new List<SessionBookingList>();

    foreach (var sessionBooking in sessionBookings)
    {
        var session = await _db.Sessions.FindAsync(sessionBooking.SessionId);

        if (session != null)
        {
            sessionList.Add(new SessionBookingList
            {
                SessionId = session.SessionId,
                SessionName = session.Name,
                StartTime = session.StartTime,
                EndTime = session.EndTime
            });
        }
    }

    var equipmentBookings = await _db.EquipmentBookeds
        .Where(sb => sb.BookingId == booking.Id)
        .ToListAsync(cancellationToken);

    var equipmentList = new List<EquipmentBookingList>();
    foreach (var equipments in equipmentBookings)
    {
        var equipment = await _db.Equipments.FindAsync(equipments.EquipmentId);

        if (equipment != null)
        {
            equipmentList.Add(new EquipmentBookingList
            {
                EquipmentId = equipment.EquipmentId,
                EquipmentName = equipment.EquipmentName
            });
        }
    }

    return new GetBookingDetailResponseModel
    {
        totalData = 1,
        BookingDetailModel = new List<GetBookingDetailModel>
        {
            new GetBookingDetailModel
            {
                BookingId = booking.Id,
                BookingDescription = booking.BookingDescription,
                Name = user?.GivenName ?? string.Empty,
                RoomId = booking.RoomId,
                RoomName = room?.Name ?? string.Empty,
                MinioUrl = minioUrl,
                IsCanceled = booking.IsCanceled,
                RoomMinioUrl = roomMinioUrl, // Placeholder if required
                BookingOrganizationName = booking.OrganizationName,
                BookingInstitutionalId = booking.InstitutionalId,
                UserRole = role?.Name ?? string.Empty,
                BookerMinioUrl = userBlobMinioUrl,
                SessionList = sessionList,
                ApproverHistory = approverHistory,
                BookingDate = booking.BookingDate.ToString(),
                EquipmentList = equipmentList
            }
        }
    };
}

    }
}
