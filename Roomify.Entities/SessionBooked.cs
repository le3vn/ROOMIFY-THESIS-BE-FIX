using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roomify.Entities;

public class SessionBooked
{
   [Key]
    public int SessionBookedId { get; set; }

    [ForeignKey("BookingId")]
    public Guid BookingId { get; set; }
    public Booking? Bookings { get; set; }
    
    [ForeignKey("SessionId")]
    public int SessionId { get; set; }
    public Session? Sessions { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? CreatedBy { get; set; }
}
