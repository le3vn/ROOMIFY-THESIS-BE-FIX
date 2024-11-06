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

            booking.IsCanceled = true;

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
