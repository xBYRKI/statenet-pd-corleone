// Models/UserInstructionAcceptance.cs
namespace statenet_lspd.Models
{
    public class UserInstructionAcceptance
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ServiceInstructionId { get; set; }
        public DateTime AcceptedAt { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual ServiceInstruction Instruction { get; set; }
    }
}
