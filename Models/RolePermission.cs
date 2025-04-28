using System.ComponentModel.DataAnnotations;

namespace statenet_lspd.Models
{
    public class RolePermission
    {
        [Required]
        public string RoleId { get; set; }
        public virtual ApplicationRole Role { get; set; }

        [Required]
        public Permission Permission { get; set; }
    }
}