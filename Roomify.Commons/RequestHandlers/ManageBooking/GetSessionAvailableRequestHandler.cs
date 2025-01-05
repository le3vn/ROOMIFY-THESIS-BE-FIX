using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Roomify.Contracts.RequestModels.ManageBooking;
using Roomify.Contracts.ResponseModels.ManageBooking;
using Roomify.Entities;

namespace Roomify.Commons.RequestHandlers.ManageBooking
{
    public class GetSessionAvailableRequestHandler : IRequestHandler<GetSessionAvailableRequestModel, GetSessionAvailableResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public GetSessionAvailableRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<GetSessionAvailableResponseModel> Handle(GetSessionAvailableRequestModel request, CancellationToken cancellationToken)
        {
            // Fetch schedules for the room on the requested date
            var scheduledSessions = await _db.Schedules
                .Where(s => s.RoomId == request.RoomId && s.Date == request.Date)
                .Select(s => s.SessionId) // Get the session ids that are already booked for that room and date
                .ToListAsync(cancellationToken);

            // Fetch all available sessions
            var allSessions = await _db.Sessions
                .Where(s => !scheduledSessions.Contains(s.SessionId)) // Exclude the ones already scheduled
                .ToListAsync(cancellationToken);

            // Map to the response model
            var response = new GetSessionAvailableResponseModel
            {
                AvailableSessions = allSessions.Select(s => new AvailableSession
                {
                    SessionId = s.SessionId,
                    SessionName = s.Name
                }).OrderBy(s => s.SessionId).ToList()
            };

            return response;
        }
    }
}
