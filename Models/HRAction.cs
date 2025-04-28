using System;
using System.ComponentModel.DataAnnotations;

namespace statenet_lspd.Models
{
    public enum HRActionType
    {
        Hire,
        Termination,
        Sanction,
        Promotion,
        Demotion,
        Suspension
    }

    public class HRAction
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string UserId { get; set; }
        // Fully qualified to ensure resolution
        public virtual statenet_lspd.Models.ApplicationUser User { get; set; }

        [Required]
        public HRActionType ActionType { get; set; }

        [Required]
        public DateTime EffectiveDate { get; set; }

        // Optional fields for specific actions
        public decimal? Amount { get; set; }
        public string Reason { get; set; }

        public string OldRank { get; set; }
        public string NewRank { get; set; }

        public decimal? OldSalary { get; set; }
        public decimal? NewSalary { get; set; }

        public string Notes { get; set; }
    }
}
