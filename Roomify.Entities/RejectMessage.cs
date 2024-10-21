using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roomify.Entities;

public class RejectMessage
{
    [Key]
    public Guid RejectMessageId { get; set; }
    [ForeignKey("Id")]
    public Guid BookingId { get; set; }
    public Booking? Bookings { get; set; }
    [StringLength(255)]
    public string Message { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? CreatedBy { get; set; }
}
