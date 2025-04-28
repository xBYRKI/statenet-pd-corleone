// Models/Sanktion.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace statenet_lspd.Models
{
    [Table("Sanktionen")]
    public class Sanktion
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Kategorie { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Vergehen { get; set; } = string.Empty;

        [Required]
        public string Beschreibung { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Geldstrafe { get; set; }

        public bool Verwarnung { get; set; }

        // geändert von string? auf bool
        public bool Degradierung { get; set; }

        // geändert von string? auf bool
        public bool Suspendierung { get; set; }

        public bool Kuendigung { get; set; }

        public int SortOrder { get; set; }
    }
}