using System.ComponentModel.DataAnnotations;

namespace statenet_lspd.Models
{
    public class Paygrade
    {
        [Key]
        public int Id { get; set; }  // Primärschlüssel (optional, nur für EF)

        [Display(Name = "Besoldungsstufe")]
        public int Besoldung { get; set; }  // Reiner Besoldungswert

        [Display(Name = "Discord-Rollen-ID")]
        public string? DiscordRoleId { get; set; }
    }
}