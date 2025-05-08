using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using statenet_lspd.Data;
using statenet_lspd.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace statenet_lspd.Controllers
{
    [Authorize(Policy = nameof(Permission.RolesPerm_View))]
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
        [Authorize(Policy = nameof(Permission.RolesPerm_View))]
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles
                .Include(r => r.RolePermissions)
                .ToListAsync();
            var allPerms = Enum.GetValues(typeof(Permission)).Cast<Permission>();
            ViewBag.AllPermissions = allPerms;
            return View(roles);
        }

        // POST: /RolePermissions/Update
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = nameof(Permission.RolesPerm_Update))]
        public async Task<IActionResult> Update(string roleId, Permission[] selected, string group)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return NotFound();

            // Ermittle alle Permissions dieser Gruppe anhand des Enum-Namens
            var groupPerms = Enum.GetValues(typeof(Permission))
                .Cast<Permission>()
                .Where(p => p.ToString().StartsWith(group + "_"))
                .ToList();

            // Lösche bestehende Grupp-Permissions in der DB
            var existingGroupPerms = _db.RolePermissions
                .Where(rp => rp.RoleId == roleId && groupPerms.Contains(rp.Permission));
            _db.RolePermissions.RemoveRange(existingGroupPerms);
            await _db.SaveChangesAsync();

            // Füge die ausgewählten Permissions wieder hinzu
            foreach (var perm in selected ?? Enumerable.Empty<Permission>())
            {
                if (groupPerms.Contains(perm))
                {
                    _db.RolePermissions.Add(new RolePermission
                    {
                        RoleId = roleId,
                        Permission = perm
                    });
                }
            }
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}