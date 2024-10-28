using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roomify.Entities;

public class Schedule
{
    [Key]
    public int ScheduleId { get; set; }
    [ForeignKey("RoomId")]
    public int RoomId { get; set; }
    public Room? Room { get; set; }
    public string ScheduleDescription { get; set; } = string.Empty;
   [ForeignKey("SessionId")]
    public int SessionId { get; set; }
    public Session? Sessions { get; set; }
    public DateOnly Date { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? CreatedBy { get; set; }
}
