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
        public string? Avatar { get; set; } // "avatar" umbenannt auf "Avatar" für Konsistenz

        [Display(Name = "Prefix")]
        [Column("prefix")]
        public string? Prefix { get; set; }

        [Display(Name = "Dienstnummer")]
        [Column("dienstnummer")]
        public int Dienstnummer { get; set; } // varchar(50)

        
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

       
        [Column("phone")]
        public string? Phone { get; set; } // Telefonnummer als string für Flexibilität

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

        [Display(Name = "Letzter Login")]
        public DateTime? LastLogin { get; set; }

        public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; } = new List<IdentityUserRole<string>>();

        public virtual ICollection<HRAction> HRActions { get; set; }

         public ICollection<Unit> Units { get; set; } = new List<Unit>();
    }
}
