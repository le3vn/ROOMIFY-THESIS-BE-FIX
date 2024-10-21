using System;
using System.ComponentModel.DataAnnotations;

namespace Roomify.Entities;

public class Session
{
    [Key]
    public int SessionId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? CreatedBy { get; set; }
}
