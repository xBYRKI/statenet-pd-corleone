using System.ComponentModel.DataAnnotations;

namespace statenet_lspd.ViewModels
{
    public class UnitViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Der Name darf maximal 100 Zeichen lang sein.")]
        [Display(Name = "Name der Unit")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Die Beschreibung darf maximal 500 Zeichen lang sein.")]
        [Display(Name = "Beschreibung")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Anzahl Mitglieder")]
        public int UserCount { get; set; }
    }
}