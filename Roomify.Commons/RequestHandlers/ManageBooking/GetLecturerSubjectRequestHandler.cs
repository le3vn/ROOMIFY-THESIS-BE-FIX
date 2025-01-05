using System.Collections.Generic;
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
    public class GetLecturerSubjectRequestHandler : IRequestHandler<GetLecturerSubjectRequestModel, List<GetLecturerSubjectResponseModel>>
    {
        private readonly ApplicationDbContext _db;

        public GetLecturerSubjectRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<GetLecturerSubjectResponseModel>> Handle(GetLecturerSubjectRequestModel request, CancellationToken cancellationToken)
        {
            // Query the Subjects table to find subjects related to the given LecturerId
            var subjects = await _db.Subjects
                .Where(subject => subject.LecturerId == request.LecturerId)
                .Select(subject => new GetLecturerSubjectResponseModel
                {
                    SubjectId = subject.SubjectId,
                    SubjectName = subject.SubjectName
                })
                .ToListAsync(cancellationToken);

            return subjects;
        }
    }
}
