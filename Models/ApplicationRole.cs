using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace statenet_lspd.Models
{
    public class ApplicationRole : IdentityRole
    {
        [Display(Name = "Discord-Rollen-ID")]
        public string? DiscordRoleId { get; set; }

        [Display(Name = "Farbe")]
        public string? ColorHex { get; set; } = "gray";

        [NotMapped]
        public int UserCount { get; set; }

        public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; } = new List<IdentityUserRole<string>>();

        // Permissions associated with this role
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

        // MenuItem relation for many-to-many
        public virtual ICollection<MenuItemRole> MenuItemRoles { get; set; } = new List<MenuItemRole>();
    }
}