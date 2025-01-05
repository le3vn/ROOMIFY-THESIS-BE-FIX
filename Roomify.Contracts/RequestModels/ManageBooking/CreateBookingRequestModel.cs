using System;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.AspNetCore.Http;
using Roomify.Contracts.ResponseModels.ManageBooking;

namespace Roomify.Contracts.RequestModels.ManageBooking;

public class CreateBookingRequestModel : IRequest<CreateBookingResponseModel>
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public string UserId { get; set; } = "";
    public int RoomId { get; set; }
    public string FullName { get; set; } ="";
    public string? OrganizationName { get; set; }
    public DateOnly BookingDate { get; set; }
    public string BookingDescription { get; set; } = "";
    public string InstitutionalId { get; set; } ="";
    public IFormFile? Evidence { get; set; } = null!;
    public List<int> SessionBookedList { get; set; } = new List<int>();
    public List<int>? EquipmentBookedList { get; set; } = new List<int>();
    // public List<GetEquipmentBookModel> EquipmentBookedList { get; set; } = new List<GetEquipmentBookModel>();

}
// public class GetEquipmentBookModel
// {
//     public int EquipmentId { get; set; }
// }
