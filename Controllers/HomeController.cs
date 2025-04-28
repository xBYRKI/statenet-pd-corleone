using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using statenet_lspd.ViewModels;

namespace statenet_lspd.Controllers
{
    [AllowAnonymous]
    public class HomeController : BaseController
    {
        public HomeController(ApplicationDbContext db)
            : base(db)
        {
        }

        public async Task<IActionResult> Index()
        {
            var model = new DashboardViewModel
            {
                UserCount = await _db.Users.CountAsync(u => u.Status == true),
                RoleCount = await _db.Roles.CountAsync(),
                AuditLogCount = await _db.AuditLogs.CountAsync()
            };

            return View(model);
        }
    }
}