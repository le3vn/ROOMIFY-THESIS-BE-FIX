using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roomify.Entities;

public class RoomGroup
{
    [Key]
    public int RoomGroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ApproverLSCUserId { get; set; }
    public string? ApproverSSOUserId { get; set; }
    public string? ApproverSLCUserId { get; set; }
    public string? ApproverBMUserId { get; set; }
     public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? CreatedBy { get; set; }
}
