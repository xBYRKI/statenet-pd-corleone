using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace statenet_lspd.Models
{
    [Table("hr_actions")]
    public class HRAction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public HRActionType ActionType { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EffectiveDate { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        [Display(Name = "Grund / Bemerkung")]
        public string? Reason { get; set; }

        [Display(Name = "Erstellt am")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}