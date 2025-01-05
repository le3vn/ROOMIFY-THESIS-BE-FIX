using System;

namespace Roomify.Contracts.ResponseModels.ManageBooking;

public class GetEquipmentResponseModel
{
    public int EquipmentId { get; set; }
    public string EquipmentName { get; set; } = "";
}
