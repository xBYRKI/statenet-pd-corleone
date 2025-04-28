using System;

namespace statenet_lspd.ViewModels
{
    public class DutyViewModel
    {
        public int Id { get; set; }
        public string DiscordId { get; set; }
        public string OfficerDisplayName { get; set; }
        public int? RightAnswer { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}