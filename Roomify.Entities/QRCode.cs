using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;

namespace Roomify.Entities;

public class QRCode
{
    [Key]
    public int QrCodeId { get; set; }
    [ForeignKey("BookingId")]
    public Guid BookingId { get; set; }
    public Booking? Bookings { get; set; }
    public Guid BlobId { get; set; }
    public Blob Blob { get; set; } = null!;    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? CreatedBy { get; set; }
}
