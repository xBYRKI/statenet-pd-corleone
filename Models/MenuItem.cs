// Models/MenuItem.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace statenet_lspd.Models
{
    public class MenuItem
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Der Titel ist erforderlich.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Die URL ist erforderlich.")]
        public string Url { get; set; }

        [Required(ErrorMessage = "Das Icon ist erforderlich.")]
        public string Icon { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Die Reihenfolge muss eine positive Zahl sein.")]
        public int Order { get; set; }

        public Guid? ParentId { get; set; }

         [ValidateNever]
        public virtual MenuItem Parent { get; set; }

        // Hier auf string umgestellt:
        [Required(ErrorMessage = "Mindestens eine Rolle ist erforderlich.")]
        public virtual ICollection<MenuItemRole> MenuItemRoles { get; set; } = new List<MenuItemRole>();

        [NotMapped]
        public List<MenuItem> Children { get; set; } = new();

        public bool ShouldValidateParentAndSection() => ParentId.HasValue;
    }

    public class MenuItemRole
    {
        // FK auf MenuItem bleibt Guid
        public Guid MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; }

        // FK auf ApplicationRole muss string sein, weil ApplicationRole.Id string ist
        public string RoleId { get; set; }
        public ApplicationRole Role { get; set; }
    }
}
