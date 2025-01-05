using System;
using System.ComponentModel.DataAnnotations;

namespace Roomify.Entities;

public class Equipment
{
    [Key]
    public int EquipmentId { get; set; }
    public string EquipmentName { get; set; }="";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? CreatedBy { get; set; }
}
