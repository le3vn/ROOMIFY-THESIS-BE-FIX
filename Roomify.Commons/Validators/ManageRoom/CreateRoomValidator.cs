using FluentValidation;
using Roomify.Contracts.RequestModels.ManageRoom;
using Roomify.Entities; // Adjust this based on where your Room entity is located
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

public class CreateRoomValidator : AbstractValidator<CreateRoomRequestModel>
{
    private readonly ApplicationDbContext _db;

    public CreateRoomValidator(ApplicationDbContext db)
    {
        _db = db;

        RuleFor(x => x.RoomName)
            .NotEmpty().WithMessage("Room name is required.")
            .MustAsync(BeUniqueRoomName).WithMessage("A room with the same name already exists.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.RoomTypeId)
            .GreaterThan(0).WithMessage("Room type must be valid.");

        RuleFor(x => x.BuildingId)
            .GreaterThan(0).WithMessage("Building ID must be valid.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than 0.");
    }

    private async Task<bool> BeUniqueRoomName(string roomName, CancellationToken cancellationToken)
    {
        var normalizedRoomName = roomName.Trim().ToLower();
        return !await _db.Rooms
            .AnyAsync(r => r.Name.Trim().ToLower() == normalizedRoomName, cancellationToken);
    }
}
