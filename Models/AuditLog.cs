// Models/AuditLog.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace statenet_lspd.Models;

public class AuditLog
{
    public int Id { get; set; }

    [Required]
    public string Action { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public string? UserId { get; set; }

    [ForeignKey("UserId")]
    public ApplicationUser? User { get; set; }
}