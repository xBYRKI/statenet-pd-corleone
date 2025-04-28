using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace statenet_lspd.Models
{
    [Table("AspNetUsers")]
    public class ApplicationUser : IdentityUser
    {
        [Column("discord_id")]
        public string? DiscordId { get; set; }

        [Display(Name = "Vor- und Nachname")]
        [Column("display_name")]
        public string? Displayname { get; set; }

        [Display(Name = "Avatar URL")]
        [Column("avatar")]
        public string? avatar { get; set; }

        [Display(Name = "Prefix")]
        [Column("prefix")]
        public string? Prefix { get; set; }

        [Display(Name = "Dienstnummer")]
        [Column("dienstnummer")]
        public int Dienstnummer { get; set; } // varchar(50)

        [Required]
        [Column("besoldung")]
        public int Besoldung { get; set; }

        [Display(Name = "Erstellt am")]
        [Column("erstellt_am")]
        public DateTime? CreatedAt { get; set; }

        [Column("status")]
        public bool? Status { get; set; }

        [Column("rangs_id")]
        public int? RankId { get; set; }

        [Column("teams_id")]
        public int? TeamId { get; set; }

        [Required]
        [Column("phone")]
        public int Phone { get; set; }

        [Display(Name = "Gekündigt am")]
        [Column("fired_at")]
        public DateTime? FiredAt { get; set; }

        [Column("birthday")]
        public DateTime? Birthday { get; set; }

        [Column("birthday_message_sent")]
        public DateTime? BirthdayMessageSent { get; set; }

        [Display(Name = "Arbeitsstunden")]
        [Column("total_hours")]
        public string? TotalHours { get; set; }

        // Existing
        [Display(Name = "Letzter Login")]
        public DateTime? LastLogin { get; set; }

        public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; } = new List<IdentityUserRole<string>>();
    }
}
