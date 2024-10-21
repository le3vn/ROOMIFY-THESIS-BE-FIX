using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Roomify.Contracts.RequestModels.ManageBooking;
using Roomify.Contracts.ResponseModels.ManageBooking;
using Roomify.Entities;

namespace Roomify.Commons.RequestHandlers.ManageBooking;

public class PostCheckInRequestHandler : IRequestHandler<PostCheckInRequestModel, PostCheckInResponseModel>
{
    private readonly ApplicationDbContext _db;

    public PostCheckInRequestHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PostCheckInResponseModel> Handle(PostCheckInRequestModel request, CancellationToken cancellationToken)
    {
        // Find the booking with the given BookingId
        var booking = await _db.Bookings.FindAsync(request.BookingId);
        
        // Check if booking exists
        if (booking == null)
        {
            return new PostCheckInResponseModel
            {
                Success = "False",
                Message = "Booking not found."
            };
        }

        // Check if the booking status is 3
        if (booking.StatusId != 2)
        {
            return new PostCheckInResponseModel
            {
                Success = "False",
                Message = "Booking is not eligible for check-in."
            };
        }

        // Set the check-in time to the current UTC time
        booking.CheckInTime = DateTimeOffset.UtcNow;

        // Update the booking in the database
        _db.Bookings.Update(booking);
        await _db.SaveChangesAsync(cancellationToken);

        return new PostCheckInResponseModel
        {
            Success = "True",
            Message = "Check-in successful."
        };
    }
}
