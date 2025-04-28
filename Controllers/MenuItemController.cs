using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using statenet_lspd.Models;

public class MenuItemController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public MenuItemController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // Index: Zeigt alle MenuItems an, sortiert nach Order
    public async Task<IActionResult> Index()
    {
        var menuItems = await _db.MenuItems
            .OrderBy(mi => mi.Order)  // Sortiere nach Order
            .Include(mi => mi.MenuItemRoles)  // Include Rollen
            .ThenInclude(mr => mr.Role)  // Lade die zugehörigen Rollen
            .ToListAsync();

        // Rollen und Parent MenuItems für Dropdowns im View
        ViewBag.Roles = await _db.Roles.ToListAsync();
        ViewBag.Parents = new SelectList(await _db.MenuItems.Where(x => x.ParentId == null).ToListAsync(), "Id", "Title");

        return View(menuItems);
    }

    // Create: Zeigt das Formular zum Erstellen eines neuen MenuItems
        public async Task<IActionResult> Create()
    {
        // Load roles and parent menu items for dropdowns
        ViewBag.Roles = await _db.Roles.ToListAsync();
        ViewBag.Parents = new SelectList(
            await _db.MenuItems.Where(x => x.ParentId == null).ToListAsync(),
            "Id", "Title");

        // Instantiate model to avoid null Model in view
        var model = new MenuItem
        {
            Order = 0,
            Icon = string.Empty
        };
        return PartialView("_CreateMenuItemModal", model);
    }

    // POST: Create New MenuItem
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MenuItem model, List<string> selectedRoleIds)
    {
        // If "Keine Kategorie" (empty GUID) submitted, treat as null
        if (model.ParentId == Guid.Empty)
        {
            model.ParentId = null;
        }

        // Initialize roles list if null and require at least one role
        selectedRoleIds ??= new List<string>();
        if (!selectedRoleIds.Any())
        {
            ModelState.AddModelError("selectedRoleIds", "Mindestens eine Rolle muss ausgewählt werden.");
        }

        // Validate and return modal partial on error
        if (!ModelState.IsValid)
        {
            ViewBag.Roles = await _db.Roles.ToListAsync();
            ViewBag.Parents = new SelectList(
                await _db.MenuItems.Where(x => x.ParentId == null).ToListAsync(),
                "Id", "Title");
            return PartialView("_CreateMenuItemModal", model);
        }

        // Create new MenuItem
        model.Id = Guid.NewGuid();
        _db.MenuItems.Add(model);
        await _db.SaveChangesAsync();

        // Assign roles
        foreach (var roleId in selectedRoleIds)
        {
            var menuItemRole = new MenuItemRole
            {
                MenuItemId = model.Id,
                RoleId     = roleId
            };
            _db.MenuItemRoles.Add(menuItemRole);
        }
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }



    // GET: Edit MenuItem
    public async Task<IActionResult> Edit(Guid id)
    {
        var model = await _db.MenuItems
            .Include(mi => mi.MenuItemRoles)
            .FirstOrDefaultAsync(mi => mi.Id == id);
        if (model == null) return NotFound();

        ViewBag.Roles = await _db.Roles.ToListAsync();
        ViewBag.Parents = new SelectList(
            await _db.MenuItems.Where(x => x.ParentId == null).ToListAsync(),
            "Id", "Title", model.ParentId);

        return PartialView("_EditMenuItemModal", model);
    }

    // POST: Edit MenuItem
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MenuItem model, List<string> selectedRoleIds)
    {
        // Treat empty GUID as no parent
        if (model.ParentId == Guid.Empty) model.ParentId = null;

        // Remove existing role mappings
        var existing = await _db.MenuItemRoles
            .Where(mr => mr.MenuItemId == model.Id).ToListAsync();
        _db.MenuItemRoles.RemoveRange(existing);

        // Add updated roles
        selectedRoleIds ??= new List<string>();
        foreach (var roleId in selectedRoleIds)
        {
            _db.MenuItemRoles.Add(new MenuItemRole
            {
                MenuItemId = model.Id,
                RoleId     = roleId
            });
        }

        // Return modal on validation errors
        if (!ModelState.IsValid)
        {
            ViewBag.Roles = await _db.Roles.ToListAsync();
            ViewBag.Parents = new SelectList(
                await _db.MenuItems.Where(x => x.ParentId == null).ToListAsync(),
                "Id", "Title", model.ParentId);
            return PartialView("_EditMenuItemModal", model);
        }

        // Persist changes
        _db.MenuItems.Update(model);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }



    // Delete: Zeigt das Formular zur Löschung eines MenuItems
    public async Task<IActionResult> Delete(Guid id)
    {
        var menuItem = await _db.MenuItems.FindAsync(id);

        if (menuItem == null)
        {
            return NotFound();
        }

        return View(menuItem);
    }

    // DeleteConfirmed (POST): Löscht das MenuItem
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var menuItem = await _db.MenuItems
            .Include(mi => mi.MenuItemRoles)
            .FirstOrDefaultAsync(mi => mi.Id == id);

        if (menuItem == null)
        {
            return NotFound();
        }

        // Entferne alle zugehörigen Rollen
        _db.MenuItemRoles.RemoveRange(menuItem.MenuItemRoles);

        // Entferne das MenuItem selbst
        _db.MenuItems.Remove(menuItem);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // MoveUp: Verschiebt das MenuItem nach oben (ändert den Order)
    public async Task<IActionResult> MoveUp(Guid id)
    {
        var item = await _db.MenuItems.FindAsync(id);
        if (item == null) return NotFound();

        var previousItem = await _db.MenuItems
            .Where(x => x.Order < item.Order)
            .OrderByDescending(x => x.Order)
            .FirstOrDefaultAsync();

        if (previousItem != null)
        {
            var tempOrder = item.Order;
            item.Order = previousItem.Order;
            previousItem.Order = tempOrder;

            _db.MenuItems.Update(item);
            _db.MenuItems.Update(previousItem);
            await _db.SaveChangesAsync();
        }

        return Json(new { success = true });
    }

    // MoveDown: Verschiebt das MenuItem nach unten (ändert den Order)
    public async Task<IActionResult> MoveDown(Guid id)
    {
        var item = await _db.MenuItems.FindAsync(id);
        if (item == null) return NotFound();

        var nextItem = await _db.MenuItems
            .Where(x => x.Order > item.Order)
            .OrderBy(x => x.Order)
            .FirstOrDefaultAsync();

        if (nextItem != null)
        {
            var tempOrder = item.Order;
            item.Order = nextItem.Order;
            nextItem.Order = tempOrder;

            _db.MenuItems.Update(item);
            _db.MenuItems.Update(nextItem);
            await _db.SaveChangesAsync();
        }

        return Json(new { success = true });
    }
}
