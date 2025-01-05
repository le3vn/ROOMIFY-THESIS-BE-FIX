using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Roomify.Entities;

public class Booking
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = "";
    public User Users { get; set; } = null!;

    [ForeignKey(nameof(IdentityRole))]
    public string RoleId { get; set; } ="";
    public IdentityRole Role { get; set; } = null!; 

    public string? FullName { get; set; }
    public string? OrganizationName { get; set; }
        
    [ForeignKey("RoomId")]
    public int RoomId { get; set; }
    public Room? Rooms { get; set; }
    public DateOnly BookingDate { get; set; } 
    
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
    public Guid? BlobId { get; set; }
    public Blob Blob { get; set; } = null!;
    public bool IsCanceled { get; set; }   
    public DateTimeOffset? CheckInTime { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
