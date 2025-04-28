namespace statenet_lspd.ViewModels
{
    public class RankViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int SortOrder { get; set; }
        public string? DiscordRoleId { get; set; }
        public string ColorHex { get; set; } = "#FFFFFF";

        public int MinPayGrade { get; set; }
        public int MaxPayGrade { get; set; }
        public int UserCount { get; set; }
    }
}