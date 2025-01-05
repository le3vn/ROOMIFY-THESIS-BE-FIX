using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Roomify.Contracts.RequestModels.ManageBooking;
using Roomify.Contracts.ResponseModels.ManageBooking;
using Roomify.Entities;

namespace Roomify.Commons.RequestHandlers.ManageBooking
{
    public class CancelBookingRequestHandler : IRequestHandler<CancelBookingRequestModel, CancelBookingResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public CancelBookingRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<CancelBookingResponseModel> Handle(CancelBookingRequestModel request, CancellationToken cancellationToken)
        {
            // Fetch the booking by ID
            var booking = await _db.Bookings
                .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

            if (booking == null)
            {
                return new CancelBookingResponseModel
                {
                    BookingId = request.BookingId,
                    Success = "false",
                    Message = "Booking not found."
                };
            }

            if (booking.IsCanceled)
            {
                return new CancelBookingResponseModel
                {
                    BookingId = request.BookingId,
                    Success = "false",
                    Message = "Booking is already canceled."
                };
            }

            // Set IsCanceled to true
            booking.IsCanceled = true;

            // Get the approver details for the booking
            var approverDetails = await _db.ApproverDetails
                .Where(ad => ad.BookingId == request.BookingId)
                .ToListAsync(cancellationToken);

            if (approverDetails.Any())
            {
                // Check if any approval has been made
                var unapprovedEntries = approverDetails.Where(ad => ad.UpdatedAt == null).ToList();

                if (unapprovedEntries.Any())
                {
                    // Case 1: No approval has been made (before any approval)
                    foreach (var entry in unapprovedEntries)
                    {
                        // Set IsApproved to false and set UpdatedAt to current time
                        entry.IsApproved = false;
                        entry.UpdatedAt = DateTime.UtcNow;
                        entry.UpdatedBy = "Admin";
                    }
                }
                else
                {
                    // Case 2: One or more approvals are already made
                    foreach (var entry in approverDetails)
                    {
                        // If there's an unapproved entry, mark it as false and set UpdatedAt
                        if (entry.UpdatedAt == null)
                        {
                            entry.IsApproved = false;
                            entry.UpdatedAt = DateTime.UtcNow;
                            entry.UpdatedBy = "Admin";
                        }
                    }
                }

                // Save changes to ApproverDetails
                await _db.SaveChangesAsync(cancellationToken);
            }

            // Now, change the StatusId of the booking to 4 (canceled)
            booking.StatusId = 4;
            
            // Save changes to Booking
            await _db.SaveChangesAsync(cancellationToken);

            return new CancelBookingResponseModel
            {
                BookingId = request.BookingId,
                Success = "true",
                Message = "Booking canceled successfully."
            };
        }
    }

}
