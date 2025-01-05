using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Roomify.Entities;

public class Organization
{
    [Key]
    public int OrganizationId { get; set; }
    [Required]
    [StringLength(255)]
    public string Name{ get; set; } = string.Empty;
    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = "";
    public User Users { get; set; } = null!;
     [ForeignKey(nameof(IdentityRole))]
    public string RoleId { get; set; } ="";
    public IdentityRole Role { get; set; } = null!; 
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? CreatedBy { get; set; }
}
