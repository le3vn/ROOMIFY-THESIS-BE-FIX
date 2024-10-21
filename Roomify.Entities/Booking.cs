using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roomify.Entities;

public class Booking
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = "";
    public User Users { get; set; } = null!;
        
    [ForeignKey("RoomId")]
    public int RoomId { get; set; }
    public Room? Rooms { get; set; }
    public DateTime BookingDate { get; set; } 
    
    [ForeignKey("StatusId")]
    public int StatusId { get; set; }
    public Status? Statuses { get; set; }
    public int ApprovalCount { get; set; }
    [Required]
    [StringLength(255)]
    public string InstitutionalId { get; set; } = string.Empty;
    [Required]
    [StringLength(255)]
    public string BookingDescription { get; set; } = string.Empty;
    public string? Evidence { get; set; }
    public DateTimeOffset? CheckInTime { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
