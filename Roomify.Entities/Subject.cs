using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roomify.Entities;

public class Subject
{
    [Key]
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } ="";

    [ForeignKey("LecturerId")]
    public string LecturerId { get; set; } = ""; 

        // Navigation property to User (Lecturer)
    public User Lecturer { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? CreatedBy { get; set; }
}