using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Roomify.Commons.RequestModels.ManageBooking;
using Roomify.Contracts.ResponseModels.ManageBooking;
using Roomify.Entities;

namespace Roomify.Commons.RequestHandlers.ManageBooking
{
    public class GetInstitutionalIdRequestHandler : IRequestHandler<GetInstitutionalIdRequestModel, GetInstitutionalIdResponseModel>
    {
        private readonly ApplicationDbContext _db;

        public GetInstitutionalIdRequestHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<GetInstitutionalIdResponseModel> Handle(GetInstitutionalIdRequestModel request, CancellationToken cancellationToken)
        {
            // Querying the InstitutionalNumbers table to find the record by UserId
            var institutionalNumber = await _db.InstitutionalNumbers
                .Where(x => x.UserId == request.UserId)
                .FirstOrDefaultAsync(cancellationToken);

            // If no record is found, return a response with null values
            if (institutionalNumber == null)
            {
                return new GetInstitutionalIdResponseModel
                {
                    LecturersId = null,
                    StaffsId = null,
                    StudentsId = null
                };
            }

            // Map the retrieved data to the response model
            var response = new GetInstitutionalIdResponseModel
            {
                LecturersId = institutionalNumber.LecturersId,
                StaffsId = institutionalNumber.StaffsId,
                StudentsId = institutionalNumber.StudentsId
            };

            return response;
        }
    }
}
