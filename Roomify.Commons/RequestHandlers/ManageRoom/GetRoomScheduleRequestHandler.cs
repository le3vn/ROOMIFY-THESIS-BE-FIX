using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Roomify.Contracts.RequestModels.ManageRoom;
using Roomify.Contracts.ResponseModels.ManageRoom;
using Roomify.Entities;

namespace Roomify.Commons.RequestHandlers.ManageRoom
{
    public class GetRoomScheduleRequestHandler : IRequestHandler<GetRoomScheduleRequestModel, GetRoomScheduleResponseModel>
    {
    private readonly ApplicationDbContext _db;

    public GetRoomScheduleRequestHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<GetRoomScheduleResponseModel> Handle(GetRoomScheduleRequestModel request, CancellationToken cancellationToken)
    {
        // Fetch the schedules for the provided RoomId and include related data
        var schedules = await _db.Schedules
            .Where(s => s.RoomId == request.RoomId)
            .Include(s => s.Sessions)  // Include related session info for time details
            .Include(s => s.Users)  // Include user info to get the PIC name
            .OrderBy(s => s.Date)  // Order by BookingDate first
            .ThenBy(s => s.Sessions.StartTime)  // Then by StartTime of the session
            .ToListAsync(cancellationToken);

        // Map the schedules to the response model
        var response = new GetRoomScheduleResponseModel
        {
            ScheduleList = schedules.Select(s => new GetRoomScheduleModel
            {
                BookingDate = s.Date,
                BookingTimeStart = s.Sessions.StartTime,  // Assuming Session has StartTime
                BookingTimeEnd = s.Sessions.EndTime,  // Assuming Session has EndTime
                EventName = s.ScheduleDescription,  // Assuming BookingDescription has EventName
                PICName = s.Users.GivenName  // Assuming User has GivenName property
            }).ToList()
        };

        return response;
    }
}
}

