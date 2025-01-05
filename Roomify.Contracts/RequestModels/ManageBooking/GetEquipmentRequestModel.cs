using System;
using MediatR;
using Roomify.Contracts.ResponseModels.ManageBooking;

namespace Roomify.Contracts.RequestModels.ManageBooking;

public class GetEquipmentRequestModel : IRequest<List<GetEquipmentResponseModel>>
{
    public int? EquipmentId { get; set; }
}
