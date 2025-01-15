using Roomify.Contracts.ResponseModels.ManageBooking;
using Roomify.Contracts.RequestModels.ManageBooking;
using Roomify.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using Roomify.Commons.Services;
using Roomify.Commons.Constants;

namespace Roomify.Commons.RequestHandlers.ManageRoom
{
    public class ApproveBookingRequestHandler : IRequestHandler<ApproveBookingRequestModel, ApproveBookingResponseModel>
{
    private readonly ApplicationDbContext _db;
    private readonly IQRCodeGeneratorService _qrCodeGeneratorService;
    public readonly IStorageService _storageService;

    public ApproveBookingRequestHandler(ApplicationDbContext db, IQRCodeGeneratorService qRCodeGeneratorService, IStorageService storageService)
    {
        _db = db;
        _qrCodeGeneratorService = qRCodeGeneratorService;
        _storageService = storageService;
    }

    public async Task<ApproveBookingResponseModel> Handle(ApproveBookingRequestModel request, CancellationToken cancellationToken)
    {
        // Fetch the ApproverDetail for the given BookingId and UserId
        var approverDetail = await _db.ApproverDetails
            .FirstOrDefaultAsync(ad => ad.BookingId == request.BookingId && ad.AppproverUserId == request.UserId, cancellationToken);

        if (approverDetail == null)
        {
            return new ApproveBookingResponseModel
            {
                Success = "false",
                Message = "No approver found for this booking and user."
            };
        }

        // Process the approval/rejection
        if (approverDetail.ApprovalOrder == 1)
        {
            // Case 1: First approver can approve/reject the booking
            await ProcessApprovalAsync(approverDetail, request, cancellationToken);
        }
        else
        {
            // Case 2: Check if previous approval exists
            var previousApproval = await _db.ApproverDetails
                .FirstOrDefaultAsync(ad => ad.BookingId == request.BookingId && ad.ApprovalOrder == approverDetail.ApprovalOrder - 1, cancellationToken);

            if (previousApproval == null || previousApproval.UpdatedAt == null || !previousApproval.IsApproved)
            {
                return new ApproveBookingResponseModel
                {
                    Success = "false",
                    Message = "Previous approval not completed or rejected."
                };
            }

            // Proceed with the current approval
            await ProcessApprovalAsync(approverDetail, request, cancellationToken);
        }

        return new ApproveBookingResponseModel
        {
            Success = "true",
            Message = "Booking approval processed successfully."
        };
    }

    private async Task ProcessApprovalAsync(ApproverDetail approverDetail, ApproveBookingRequestModel request, CancellationToken cancellationToken)
    {
        var bookings = await _db.Bookings
                .Where(r => r.Id == request.BookingId).Select(r => r.RoomId)
                .FirstOrDefaultAsync();

        var room = await _db.Rooms
                .Where(r => r.RoomId == bookings)
                .FirstOrDefaultAsync();
        // Update the current approver's details (whether approving or rejecting)
        approverDetail.IsApproved = request.IsApproved;
        approverDetail.UpdatedAt = DateTimeOffset.UtcNow;
        approverDetail.UpdatedBy = "Admin";  // This should ideally be the user making the request

        // Add a record to the ApprovalHistory table
        _db.ApproverHistories.Add(new ApproverHistory
        {
            BookingId = request.BookingId,
            UserId = request.UserId,
            StatusId = request.IsApproved ? 2 : 3,  // 2 is approved, 3 is rejected
            ApprovalOrder = approverDetail.ApprovalOrder,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = request.UserId  // Replace "Admin" with the actual user if applicable
        });

        if (!request.IsApproved && !string.IsNullOrEmpty(request.RejectMessage))
    {
        var rejectionMessage = new RejectMessage
        {
            BookingId = request.BookingId,
            Message = request.RejectMessage,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = request.UserId  // This can be the actual approver's name
        };

        // Save the rejection message to the database
        _db.RejectMessages.Add(rejectionMessage);
    }

        // If the booking is being rejected, update all subsequent approvers' details
        if (!request.IsApproved)
        {
            // Mark all subsequent approvers as rejected by updating their ApproverDetails
            var subsequentApprovers = await _db.ApproverDetails
                .Where(ad => ad.BookingId == request.BookingId && ad.ApprovalOrder > approverDetail.ApprovalOrder)
                .ToListAsync(cancellationToken);

            foreach (var subsequentApprover in subsequentApprovers)
            {
                subsequentApprover.IsApproved = false;
                subsequentApprover.UpdatedAt = DateTimeOffset.UtcNow;
                subsequentApprover.UpdatedBy = "Admin";  // Again, this can be the actual user
            }

            // Save all changes to the ApproverDetails (current and subsequent approvers)
            await _db.SaveChangesAsync(cancellationToken);

            // Notify the user that the booking was rejected
            var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);
            if (booking != null)
            {
                await AddNotificationToDb(booking.UserId, 
                    "Booking Rejected", 
                    $"Your booking for room {room?.Name} has been rejected. Check your On-going page for details.", 
                    "SYSTEM");
            }

        }
        else
        {
            // If the current approver is approving, just update their own record
            await _db.SaveChangesAsync(cancellationToken);

            // If not the last approver, notify the next approver
            var nextApprover = await _db.ApproverDetails
                .FirstOrDefaultAsync(ad => ad.BookingId == request.BookingId && ad.ApprovalOrder == approverDetail.ApprovalOrder + 1, cancellationToken);

            if (nextApprover != null)
            {
                // Notify the next approver
                await AddNotificationToDb(nextApprover.AppproverUserId, 
                    "Approval Needed", 
                    $"Your approval is needed for for room {room?.Name}. Please review it.", 
                    "SYSTEM");
            }
            else
            {
                // Last approver's decision: approve or reject the booking
                var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);
                if (booking != null)
                {
                    booking.StatusId = request.IsApproved ? 2 : 3;  // Approved or Rejected status
                    

                    await _db.SaveChangesAsync(cancellationToken);

                    byte[] qrCodeBytes = await _qrCodeGeneratorService.GenerateQRCode(request.BookingId.ToString());

                // Create a Blob entity for the QR code PNG
                var blob = new Blob
                {
                    Id = Guid.NewGuid(),
                    FileName = $"{request.BookingId}_QRCode.png",
                    FilePath = $"{BlobPath.QRCode}/{request.BookingId}_QRCode.png",
                    ContentType = "image/png",  // PNG Content-Type
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "Admin"  // This can be the actual user creating the blob
                };

                // Upload the generated QR code PNG to the storage
                using (var stream = new MemoryStream(qrCodeBytes))
                {
                    await _storageService.UploadFileAsync(blob.FilePath, stream);
                }

                // Add the Blob entry to the database
                _db.Blobs.Add(blob);
                await _db.SaveChangesAsync(cancellationToken);

                // Create the QRCodes entry and link it to the booking
                var qrCode = new QRCode
                {
                    BookingId = request.BookingId,
                    BlobId = blob.Id,  // Link to the Blob containing the QR code image
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "Admin"  // This can be the actual user creating the QR code
                };

                // Add the QRCode entry to the QRCodes table
                _db.QRCodes.Add(qrCode);
                await _db.SaveChangesAsync(cancellationToken);


                    

                    // Notify the user that the booking is fully approved or rejected
                    await AddNotificationToDb(booking.UserId, 
                        request.IsApproved ? "Booking Approved" : "Booking Rejected", 
                        $"Your booking for room {room?.Name} has been { (request.IsApproved ? "approved" : "rejected") }. Check your On-going page for details.", 
                        "SYSTEM");
                }
            }
        }
    }

    private async Task AddNotificationToDb(string userId, string subject, string message, string createdBy)
    {
        var notification = new Notification
        {
            UserId = userId,
            Subject = subject,
            Message = message,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = createdBy
        };

        await _db.Notifications.AddAsync(notification);
        await _db.SaveChangesAsync();
    }
}
}