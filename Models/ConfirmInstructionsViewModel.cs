using System.Collections.Generic;
using statenet_lspd.Models;

namespace statenet_lspd.ViewModels
{
    public class ConfirmInstructionsViewModel
    {
        public List<ServiceInstruction> PendingInstructions { get; set; }
    }
}