using System.Collections.Generic;
using statenet_lspd.Models;

namespace statenet_lspd.ViewModels
{
    /// <summary>
    /// DTO for listing users along with their roles.
    /// </summary>
    public class UserWithRolesViewModel
    {
        public ApplicationUser User { get; set; } = null!;
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
