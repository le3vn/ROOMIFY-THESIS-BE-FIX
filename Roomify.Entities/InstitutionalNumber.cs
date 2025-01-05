using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roomify.Entities;

public class InstitutionalNumber
{
    [Key]
    public int InstitutionalId { get; set; }
    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = "";
    public User Users { get; set; } = null!;
    public string? LecturersId {get; set;}
    public string? StaffsId {get; set;}
    public string? StudentsId {get; set;}
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? CreatedBy { get; set; }
}
