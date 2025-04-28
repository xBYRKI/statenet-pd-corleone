using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace statenet_lspd.Models
{
    public class EditUserViewModel
    {
        public string? Id { get; set; }

        [Required]
        [Display(Name = "Benutzername")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Vor- und Nachname")]
        public string? Displayname { get; set; }

        [Display(Name = "Dienstnummer")]
        public int Dienstnummer { get; set; }

        [Display(Name = "Discord-ID")]
        public string? DiscordId { get; set; }

        [Display(Name = "Besoldung")]
        public int Besoldung { get; set; }

        [Display(Name = "Status")]
        public bool Status { get; set; }

        [Display(Name = "Telefon")]
        public string? Phone { get; set; }

        [Display(Name = "Gekündigt am")]
        [DataType(DataType.Date)]
        public DateTime? FiredAt { get; set; }

        [Display(Name = "Geburtstag")]
        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; }

        [Display(Name = "Geburtstagsnachricht gesendet")]
        [DataType(DataType.Date)]
        public DateTime? BirthdayMessageSent { get; set; }

        [Display(Name = "Arbeitsstunden")]
        public string? TotalHours { get; set; }

        [Display(Name = "Rollen")]
        public List<string> SelectedRoles { get; set; } = new();

        // Wird im Controller für die Rollenauswahl befüllt
        public List<SelectListItem> AllRoles { get; set; } = new();
    }
}
