using Roomify.Contracts.ResponseModels.ManageRoom;
using Roomify.Contracts.RequestModels.ManageRoom;
using Roomify.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using System.Threading.Tasks;

namespace Roomify.Commons.RequestHandlers.ManageRoom
{
    public class CreateRoomRequestHandler : IRequestHandler<CreateRoomRequestModel, CreateRoomResponseModel>
    {
        private readonly ApplicationDbContext _db;
        private readonly IValidator<CreateRoomRequestModel> _validator;

        public CreateRoomRequestHandler(ApplicationDbContext db, IValidator<CreateRoomRequestModel> validator)
        {
            _db = db;
            _validator = validator;
        }

        public async Task<CreateRoomResponseModel> Handle(CreateRoomRequestModel request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new CreateRoomResponseModel
                {
                    Success = "false",
                    Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))
                };
            }

            var room = new Room
            {
                Name = request.RoomName,
                RoomType = request.RoomTypeId,
                Description = request.Description,
                BuildingId = request.BuildingId,
                Capacity = request.Capacity,
                CreatedBy = "Admin",
                UpdatedBy = "Admin"
            };

            _db.Rooms.Add(room);
            await _db.SaveChangesAsync(cancellationToken);

            return new CreateRoomResponseModel
            {
                RoomId = room.RoomId, 
                Success = "true",
                Message = "Room created successfully."
            };
        }
    }
}
