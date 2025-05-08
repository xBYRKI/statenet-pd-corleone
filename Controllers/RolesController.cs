using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using statenet_lspd.Models;
using statenet_lspd.Data;

namespace statenet_lspd.Controllers
{
    [Authorize(Policy = nameof(Permission.ROLE_View))]
    public class RolesController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly AuditService _audit;

        public RolesController(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            AuditService audit)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            _audit = audit;
        }

        // List roles
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var roleUserCounts = await _context.UserRoles
                .GroupBy(ur => ur.RoleId)
                .ToDictionaryAsync(g => g.Key, g => g.Count());

            foreach (var role in roles)
                role.UserCount = roleUserCounts.GetValueOrDefault(role.Id);

            return View(roles);
        }

        // GET: Create Role Modal
        [Authorize(Policy = nameof(Permission.ROLE_Add))]
        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_CreateRole", new ApplicationRole());
        }

        // POST: Create Role
        [Authorize(Policy = nameof(Permission.ROLE_Add))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationRole model)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreateRole", model);

            var result = await _roleManager.CreateAsync(model);
            if (result.Succeeded)
            {
                await _audit.LogAsync("Role.Created", $"Rolle '{model.Name}' wurde erstellt.");
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return PartialView("_CreateRole", model);
        }

        // GET: Edit Role Modal
        [Authorize(Policy = nameof(Permission.ROLE_Edit))]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();
            return PartialView("_EditRole", role);
        }

        // POST: Edit Role
        [Authorize(Policy = nameof(Permission.ROLE_Edit))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationRole model)
        {
            if (!ModelState.IsValid)
                return PartialView("_EditRole", model);

            var role = await _roleManager.FindByIdAsync(model.Id);
            if (role == null) return NotFound();

            role.Name = model.Name;
            role.DiscordRoleId = model.DiscordRoleId;
            role.ColorHex = model.ColorHex;

            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                await _audit.LogAsync("Role.Updated", $"Rolle '{model.Name}' wurde bearbeitet.");
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return PartialView("_EditRole", model);
        }

        // GET: Delete Role Modal
        [Authorize(Policy = nameof(Permission.ROLE_Delete))]
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();

            var userCount = await _context.UserRoles.CountAsync(ur => ur.RoleId == id);
            ViewBag.UserCount = userCount;
            return PartialView("_DeleteRole", role);
        }

        // POST: Delete Role
        [Authorize(Policy = nameof(Permission.ROLE_Delete))]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();

            var userCount = await _context.UserRoles.CountAsync(ur => ur.RoleId == id);
            if (userCount > 0)
            {
                TempData["Error"] = "Rolle kann nicht gelöscht werden, da noch Benutzer zugewiesen sind.";
                return RedirectToAction(nameof(Index));
            }

            await _roleManager.DeleteAsync(role);
            await _audit.LogAsync("Role.Deleted", $"Rolle '{role.Name}' wurde gelöscht.");
            return RedirectToAction(nameof(Index));
        }
    }
}
