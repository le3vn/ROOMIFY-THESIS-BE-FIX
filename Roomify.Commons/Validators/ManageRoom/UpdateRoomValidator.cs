using System;
using FluentValidation;
using Roomify.Contracts.RequestModels.ManageRoom;

namespace Roomify.Commons.Validators.ManageRoom;

public class UpdateRoomValidator : AbstractValidator<UpdateRoomRequestModel>
{
    public UpdateRoomValidator()
    {
        RuleFor(x => x.RoomId)
            .GreaterThan(0).WithMessage("RoomId must be greater than 0.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.RoomTypeId)
            .GreaterThan(0).WithMessage("RoomTypeId must be greater than 0.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.BuildingId)
            .GreaterThan(0).WithMessage("BuildingId must be greater than 0.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than 0.");

    }
}
