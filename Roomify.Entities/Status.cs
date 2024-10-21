using System;
using System.ComponentModel.DataAnnotations;

namespace Roomify.Entities;

public class Status
{
    [Key]
    public int StatusId { get; set; }
    [Required]
    [StringLength(255)]
    public string Name{ get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? CreatedBy { get; set; }
}
