using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roomify.Entities;

public class Room
{
   [Key]
    public int RoomId { get; set; }
    [ForeignKey("RoomGroupId")] // Foreign key to RoomGroup
    public int RoomGroupId { get; set; } // Make it nullable if a room doesn't always belong to a group
    public RoomGroup? RoomGroups { get; set; }
    [ForeignKey("BuildingId")]
    public int BuildingId { get; set; }
    public Building? Buildings { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public int RoomType { get; set; }
    
    public int Capacity { get; set; }
    public Guid BlobId { get; set; }
    public Blob Blob { get; set; } = null!;
    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? UpdatedBy { get; set; }
}
