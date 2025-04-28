using System.ComponentModel.DataAnnotations;

namespace statenet_lspd.Models
{
    public class Rank
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = null!;

        public int SortOrder { get; set; }

        public string? DiscordRoleId { get; set; }

        [RegularExpression("^#(?:[0-9a-fA-F]{3}){1,2}$", ErrorMessage = "Ungültiges HEX-Format")]
        public string ColorHex { get; set; } = "#FFFFFF";

        // Neue Felder für Besoldungsstufen
        [Display(Name = "Mindestbesoldung")]
        public int MinPayGrade { get; set; }

        [Display(Name = "Maximalbesoldung")]
        public int MaxPayGrade { get; set; }
    }
}