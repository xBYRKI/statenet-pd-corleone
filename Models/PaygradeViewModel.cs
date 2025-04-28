using System.ComponentModel.DataAnnotations;

namespace statenet_lspd.ViewModels
{
    public class PaygradeViewModel
    {
        [Display(Name = "Schlüssel‑ID")]
        public int Id { get; set; }

        [Display(Name = "Besoldungsstufe")]
        public int Besoldung { get; set; }

        [Display(Name = "Discord‑Rollen‑ID")]
        public string? DiscordRoleId { get; set; }

        [Display(Name = "Anzahl Nutzer")]
        public int UserCount { get; set; }
    }
}
