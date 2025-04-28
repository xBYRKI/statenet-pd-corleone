using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using statenet_lspd.Models;
using statenet_lspd.ViewModels;

namespace statenet_lspd.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly AuditService _audit;

    public UsersController(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        RoleManager<ApplicationRole> roleManager,
        AuditService audit)
    {
        _userManager = userManager;
        _context = context;
        _roleManager = roleManager;
        _audit = audit;
    }

    public async Task<IActionResult> Index(string? search, bool onlyDiscord = false, int page = 1)
{
    const int pageSize = 25;
    var query = _userManager.Users.AsQueryable();

    // Alphabetisch sortieren
    query = query.OrderBy(u => u.Displayname);

    // Suchfilter auf UserName, Displayname, DiscordId und Dienstnummer
    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(u =>
            u.UserName.Contains(search!) ||
            (u.Displayname != null && u.Displayname.Contains(search!)) ||
            (u.DiscordId  != null && u.DiscordId.Contains(search!)) ||
            (u.Dienstnummer != null && u.Dienstnummer.ToString().Contains(search!))
        );
    }

    // Nur Nutzer mit Discord‑ID?
    if (onlyDiscord)
    {
        query = query.Where(u => !string.IsNullOrEmpty(u.DiscordId));
    }

    // Paginierung
    var totalCount = await query.CountAsync();
    var users = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var model = new List<UserWithRolesViewModel>(users.Count);
    foreach (var u in users)
    {
        var roles = await _userManager.GetRolesAsync(u);
        model.Add(new UserWithRolesViewModel {
            User  = u,
            Roles = roles
        });
    }

    ViewBag.PageNumber = page;
    ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

    return View(model);
}


    // 🔹 Benutzer erstellen – GET
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var viewModel = new EditUserViewModel
        {
            Dienstnummer = await GenerateUniqueDienstnummerAsync(),
            AllRoles = GetAllRoleSelectList(),
            SelectedRoles = new List<string>()
        };
        return PartialView("_CreateUser", viewModel);
    }

    // 🔹 Benutzer erstellen – POST
    [HttpPost]
    public async Task<IActionResult> Create(EditUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AllRoles = GetAllRoleSelectList();
            return PartialView("_CreateUser", model);
        }

        var user = new ApplicationUser
        {
            UserName = model.UserName,
            Displayname = model.Displayname,
            Dienstnummer = model.Dienstnummer,
            DiscordId = model.DiscordId
        };

        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            AddErrors(result);
            model.AllRoles = GetAllRoleSelectList();
            
            return PartialView("_CreateUser", model);
        }

        if (model.SelectedRoles.Any())
        {
            await _userManager.AddToRolesAsync(user, model.SelectedRoles);
        }
        await _audit.LogAsync("User.Created", $"Benutzer {model.Displayname} wurde angelegt.");
        return RedirectToAction("Index");
    }

    // 🔸 Benutzer bearbeiten – GET
    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var userRoles = await _userManager.GetRolesAsync(user);
        var model = new EditUserViewModel
        {
            Id = user.Id,
            UserName = user.Displayname,
            Displayname = user.Displayname,
            Dienstnummer = user.Dienstnummer,
            DiscordId = user.DiscordId,
            SelectedRoles = userRoles.ToList(),
            AllRoles = GetAllRoleSelectList(userRoles)
        };

        return PartialView("_EditUser", model);
    }

    // 🔸 Benutzer bearbeiten – POST
    [HttpPost]
    public async Task<IActionResult> Edit(EditUserViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null) return NotFound();

        user.Displayname = model.UserName;
        user.Displayname = model.Displayname;
        user.DiscordId = model.DiscordId;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            AddErrors(result);
            model.AllRoles = GetAllRoleSelectList(model.SelectedRoles);
            return PartialView("_EditUser", model);
        }

        // Rollen abgleichen
        var currentRoles = await _userManager.GetRolesAsync(user);
        var rolesToAdd = model.SelectedRoles.Except(currentRoles);
        var rolesToRemove = currentRoles.Except(model.SelectedRoles);

        await _userManager.AddToRolesAsync(user, rolesToAdd);
        await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

        await _audit.LogAsync("User.Edited", $"Benutzer {model.UserName} wurde bearbeitet.");

        return RedirectToAction("Index");
    }

    // 🗑️ Benutzer löschen – GET
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        if (user.Id == _userManager.GetUserId(User))
        {
            TempData["Error"] = "Du kannst dich nicht selbst löschen.";
            return RedirectToAction("Index");
        }

        return PartialView("_DeleteUser", user);
    }

    // 🗑️ Benutzer löschen – POST
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null && user.Id != _userManager.GetUserId(User))
        {
            await _userManager.DeleteAsync(user);
            await _audit.LogAsync("User.Delete", $"Benutzer '{user.Displayname}' wurde gelöscht.");
        }

        return RedirectToAction("Index");
    }

    // 📦 Hilfsfunktionen
    private async Task<int> GenerateUniqueDienstnummerAsync()
    {
        var used = await _context.Users
            .Where(u => u.Dienstnummer != null)
            .Select(u => u.Dienstnummer)
            .ToListAsync();

        var usedSet = new HashSet<int>(used);
        var rand = new Random();
        int nummer;

        do
        {
            nummer = rand.Next(1000, 10000);
        }
        while (usedSet.Contains(nummer));

        return nummer;
    }

    private List<SelectListItem> GetAllRoleSelectList(IEnumerable<string>? selected = null)
    {
        var roles = _roleManager.Roles.ToList(); // Materialize to memory first

        return roles
            .Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name,
                Selected = selected != null && selected.Contains(r.Name)
            })
            .ToList();
    }


    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    private async Task LogAuditAsync(string action, string description)
    {
        var log = new AuditLog
        {
            UserId = User.Identity?.Name ?? "Unbekannt",
            Action = action,
            Description = description,
            Timestamp = DateTime.UtcNow
        };
        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();
    }

}