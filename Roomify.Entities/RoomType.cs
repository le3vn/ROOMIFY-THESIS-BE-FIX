using System;
using System.ComponentModel.DataAnnotations;

namespace Roomify.Entities;

public class RoomType
{
    [Key]
    public int RoomTypeId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? CreatedBy { get; set; }
}
