using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roomify.Entities;

public class Building
{
    [Key]
    public int BuildingId { get; set; }
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;
    [ForeignKey("BlobId")]
    public string BuildingPictureId { get; set; } = string.Empty;
    public Blob? Blob { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? CreatedBy { get; set; }
}
