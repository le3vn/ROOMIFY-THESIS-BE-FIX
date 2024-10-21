using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roomify.Entities;

public class ApproverDetail
{
    [Key]
    public int ApproverDetailsId { get; set; }
    [ForeignKey("BookingId")]
    public Guid BookingId { get; set; }
    public Booking? Bookings { get; set; }
    [Required]
    public string AppproverUserId { get; set; } = string.Empty;
    public int ApprovalOrder { get; set; }
    public bool IsApproved { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
