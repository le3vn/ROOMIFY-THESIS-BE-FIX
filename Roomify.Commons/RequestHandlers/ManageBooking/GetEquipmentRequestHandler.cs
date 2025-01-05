using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Roomify.Contracts.RequestModels.ManageBooking;
using Roomify.Contracts.ResponseModels.ManageBooking;
using Roomify.Entities;

namespace Roomify.Commons.RequestHandlers.ManageBooking;

public class GetEquipmentRequestHandler : IRequestHandler<GetEquipmentRequestModel, List<GetEquipmentResponseModel>>
{
    private readonly ApplicationDbContext _db;

    public GetEquipmentRequestHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<GetEquipmentResponseModel>> Handle(GetEquipmentRequestModel request, CancellationToken cancellationToken)
    {
        // If EquipmentId is provided, filter by it. Otherwise, return all equipment.
        IQueryable<Equipment> query = _db.Equipments.AsQueryable();

        if (request.EquipmentId.HasValue)
        {
            query = query.Where(e => e.EquipmentId == request.EquipmentId.Value);
        }

        var equipmentList = await query.ToListAsync(cancellationToken);

        // Map to response model
        var response = equipmentList.Select(e => new GetEquipmentResponseModel
        {
            EquipmentId = e.EquipmentId,
            EquipmentName = e.EquipmentName
        }).ToList();

        return response;
    }
}
