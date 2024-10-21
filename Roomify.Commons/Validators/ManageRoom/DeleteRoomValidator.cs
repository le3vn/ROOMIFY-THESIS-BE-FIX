using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Roomify.Contracts.RequestModels.ManageRoom;
using Roomify.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Roomify.Validators
{
    public class DeleteRoomValidator : AbstractValidator<DeleteRoomRequestModel>
    {
        private readonly ApplicationDbContext _db;

        public DeleteRoomValidator(ApplicationDbContext db)
        {
            _db = db;

            RuleFor(x => x.RoomId)
                .NotEmpty().WithMessage("Id is required.")
                .MustAsync(ExistInDatabase).WithMessage("Room with this Id does not exist.");
        }

        private async Task<bool> ExistInDatabase(int id, CancellationToken cancellationToken)
        {
            return await _db.Rooms.AnyAsync(r => r.RoomId == id, cancellationToken);
        }
    }
}
