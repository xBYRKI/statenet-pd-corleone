// Models/Unit.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace statenet_lspd.Models
{
    [Table("units")]
    public class Unit
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Einheitsname")]
        public string Name { get; set; }

        [Display(Name = "Beschreibung")]
        public string Description { get; set; }

         public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
