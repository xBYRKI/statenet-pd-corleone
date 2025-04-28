using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace statenet_lspd.Models
{
    public class Duty
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(255)]
        public string DiscordId { get; set; } = null!;

        [Required]
        public DutyStatus Status { get; set; } = DutyStatus.PENDING;

        public DateTime? LastCheck { get; set; }

        public int? RightAnswer { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
