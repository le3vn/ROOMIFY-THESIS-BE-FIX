using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Roomify.Entities;

public class ManageRole
{
    [Key]
    public int ManageRolesId { get; set; }

    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = "";
    public User Users { get; set; } = null!;
    
    [ForeignKey(nameof(IdentityRole))]
    public string RoleId { get; set; } ="";
    public IdentityRole Role { get; set; } = null!; 
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string? CreatedBy { get; set; }
}
