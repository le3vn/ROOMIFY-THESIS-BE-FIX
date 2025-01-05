using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roomify.Entities;

public class EquipmentBooked
{
    [Key]
    public int EquipmentBookedId { get; set; }
    [ForeignKey("BookingId")]
    public Guid BookingId { get; set; }
    public Booking? Bookings { get; set; }
    [ForeignKey("EquipmentId")]
    public int EquipmentId { get; set; }
    public Equipment? Equipments { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? CreatedBy { get; set; }
}
