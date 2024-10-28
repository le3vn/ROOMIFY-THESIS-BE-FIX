using Roomify.Contracts.ResponseModels.ManageBooking;
using Roomify.Contracts.RequestModels.ManageBooking;
using Roomify.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Roomify.Commons.RequestHandlers.ManageRoom
{
    public class ApproveBookingRequestHandler : IRequestHandler<ApproveBookingRequestModel, ApproveBookingResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public ApproveBookingRequestHandler(ApplicationDbContext db)
        {
            _db = db;
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

            // Check the ApprovalOrder
            if (approverDetail.ApprovalOrder == 1)
            {
                // Case 1: First approver can approve/reject the booking
                await ProcessApprovalAsync(approverDetail, request, cancellationToken);
            }
            else
            {
                // Case 2: Check previous approval
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
            // Update ApproverDetails
            approverDetail.IsApproved = request.IsApproved;
            approverDetail.UpdatedAt = DateTimeOffset.UtcNow;
            approverDetail.UpdatedBy = "Admin";

            // Save ApproverDetails changes
            await _db.SaveChangesAsync(cancellationToken);

            // Update Booking status based on approval or rejection
            var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);
            if (booking == null) return;

            booking.ApprovalCount -= 1;

            if (request.IsApproved)
            {
                if(booking.ApprovalCount == 0){
                // Case 1.1: Approved
                     booking.StatusId = 2;

                    // Fetch the sessions booked for this booking
                    var sessionBookeds = await _db.SessionBookeds
                        .Where(sb => sb.BookingId == request.BookingId)
                        .ToListAsync(cancellationToken);

                    // If no sessions are booked, handle appropriately
                    if (sessionBookeds.Count == 0)
                    {
                        throw new Exception("No sessions booked for this booking.");
                    }

                    // Add Schedule entries for each booked session
                    foreach (var sessionBooked in sessionBookeds)
                    {
                        var schedule = new Schedule
                        {
                            RoomId = booking.RoomId,
                            SessionId = sessionBooked.SessionId,  // Use the SessionId from the booked session
                            ScheduleDescription = booking.BookingDescription,
                            CreatedAt = DateTimeOffset.UtcNow,
                            CreatedBy = "Admin",
                            Date = DateOnly.FromDateTime(booking.BookingDate)

                        };

                        _db.Schedules.Add(schedule);
                    }
                }
            }
            else
            {
                // Case 1.2 / 2.1: Rejected
                
                    booking.StatusId = 3; // Booking is rejected
                

                // Add rejection message
                if (!string.IsNullOrWhiteSpace(request.RejectMessage))
                {
                    _db.RejectMessages.Add(new RejectMessage
                    {
                        BookingId = request.BookingId,
                        Message = request.RejectMessage,
                        CreatedAt = DateTimeOffset.UtcNow,
                        CreatedBy = "Admin",
                    });
                }
            }
            booking.UpdatedAt = DateTimeOffset.UtcNow;
            booking.UpdatedBy = "Admin";

            // Save Booking changes
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
