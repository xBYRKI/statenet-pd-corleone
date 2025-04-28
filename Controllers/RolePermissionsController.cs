using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using statenet_lspd.Data;
using statenet_lspd.Models;
using System.Linq;
using System.Threading.Tasks;

namespace statenet_lspd.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class RolePermissionsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RolePermissionsController(ApplicationDbContext db, RoleManager<ApplicationRole> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
        }

        // GET: /RolePermissions
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles
                .Include(r => r.RolePermissions)
                .ToListAsync();
            var allPerms = System.Enum.GetValues(typeof(Permission)).Cast<Permission>();
            ViewBag.AllPermissions = allPerms;
            return View(roles);
        }

        // POST: /RolePermissions/Update
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string roleId, Permission[] selected)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return NotFound();

            // Remove existing permissions
            var existing = _db.RolePermissions.Where(rp => rp.RoleId == roleId);
            _db.RolePermissions.RemoveRange(existing);
            await _db.SaveChangesAsync();

            // Add selected
            foreach (var perm in selected ?? new Permission[0])
            {
                _db.RolePermissions.Add(new RolePermission { RoleId = roleId, Permission = perm });
            }
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}