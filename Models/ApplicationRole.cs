using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace statenet_lspd.Models;
public class ApplicationRole : IdentityRole
{
    [Display(Name = "Discord-Rollen-ID")]
    public string? DiscordRoleId { get; set; }

    public string? ColorHex  { get; set; } = "gray"; 

    [NotMapped]
    public int UserCount { get; set; }

    public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; } = new List<IdentityUserRole<string>>();
}
