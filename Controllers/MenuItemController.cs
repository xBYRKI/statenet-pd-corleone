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
        // Lade alle Rollen und Parent MenuItems für Dropdowns
        ViewBag.Roles = await _db.Roles.ToListAsync();
        ViewBag.Parents = new SelectList(await _db.MenuItems.Where(x => x.ParentId == null).ToListAsync(), "Id", "Title");

        return PartialView("_CreateMenuItemModal");
    }
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(MenuItem model, List<string> selectedRoleIds)
{
    // Debugging-Ausgabe: Zeige die übergebenen Daten
    Console.WriteLine($"ParentId: {model.ParentId}");
    Console.WriteLine($"Selected Roles: {string.Join(", ", selectedRoleIds)}");

    // Überprüfen, ob ParentId den Wert für "Keine Kategorie" enthält
    if (model.ParentId == Guid.Empty)  
    {
        model.ParentId = null;  // Setze ParentId auf null, wenn "Keine Kategorie" ausgewählt wurde
    }

    // Überprüfen, ob mindestens eine Rolle ausgewählt wurde
    if (selectedRoleIds == null || !selectedRoleIds.Any())
    {
        ModelState.AddModelError("MenuItemRoles", "Mindestens eine Rolle muss ausgewählt werden.");
    }

    // Wenn ModelState ungültig ist, zurück zur View mit den Fehlern und Dropdowns
    if (!ModelState.IsValid)
    {
        Console.WriteLine("ModelState is not valid. Displaying errors:");

        // Zeige alle ModelState-Fehler
        foreach (var key in ModelState.Keys)
        {
            foreach (var error in ModelState[key].Errors)
            {
                Console.WriteLine($"Error in {key}: {error.ErrorMessage}");
            }
        }

        // Fehlerbehandlung und erneutes Laden der Dropdowns
        ViewBag.Roles = await _db.Roles.ToListAsync();
        ViewBag.Parents = new SelectList(await _db.MenuItems.Where(x => x.ParentId == null).ToListAsync(), "Id", "Title");

        return View(model);  // Gibt das Modell und die Fehler zurück
    }

    // Wenn ModelState gültig ist, dann Speichern
    model.Id = Guid.NewGuid();  // Generiere eine neue ID für das MenuItem
    _db.MenuItems.Add(model);
    await _db.SaveChangesAsync();

    // Füge die zugehörigen Rollen hinzu
    foreach (var roleId in selectedRoleIds)
    {
        var menuItemRole = new MenuItemRole
        {
            MenuItemId = model.Id
        };
        _db.MenuItemRoles.Add(menuItemRole);
    }
    await _db.SaveChangesAsync();

    // Erfolgreiches Speichern
    return RedirectToAction(nameof(Index));  // Zurück zur Index-Seite nach erfolgreichem Speichern
}





    // Edit: Zeigt das Formular zur Bearbeitung eines bestehenden MenuItems
    public async Task<IActionResult> Edit(Guid id)
    {
        var menuItem = await _db.MenuItems
            .Include(mi => mi.MenuItemRoles)
            .ThenInclude(mr => mr.Role)
            .FirstOrDefaultAsync(mi => mi.Id == id);

        if (menuItem == null)
        {
            return NotFound();
        }

        // Lade Rollen und Parent MenuItems für Dropdowns
        ViewBag.Roles = await _db.Roles.ToListAsync();
        ViewBag.Parents = new SelectList(await _db.MenuItems.Where(x => x.ParentId == null).ToListAsync(), "Id", "Title");

        return PartialView("_EditMenuItemModal", menuItem);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MenuItem model, List<string> selectedRoleIds)
    {
        // Überprüfen, ob ParentId oder Section benötigt wird
        if (model.ShouldValidateParentAndSection())
        {
            ModelState.AddModelError("Section", "Die Sektion ist erforderlich, wenn das Menüpunkt eine Kategorie hat.");
        }

        if (!ModelState.IsValid)
        {
            // Fehlerbehandlung und Rückgabe der View mit den Fehlern
            ViewBag.Roles = await _db.Roles.ToListAsync();
            ViewBag.Parents = new SelectList(await _db.MenuItems.Where(x => x.ParentId == null).ToListAsync(), "Id", "Title");

            return View(model); // Oder eine PartialView, wenn du Modal verwendest
        }

        var existingMenuItem = await _db.MenuItems
            .Include(mi => mi.MenuItemRoles)
            .FirstOrDefaultAsync(mi => mi.Id == model.Id);

        if (existingMenuItem == null)
        {
            return NotFound();
        }

        // Entferne alle alten Rollen
        _db.MenuItemRoles.RemoveRange(existingMenuItem.MenuItemRoles);

        // Füge die neuen Rollen hinzu
        foreach (var roleId in selectedRoleIds)
        {
            var menuItemRole = new MenuItemRole
            {
                MenuItemId = model.Id
            };
            _db.MenuItemRoles.Add(menuItemRole);
        }

        // Speichere die Änderungen am MenuItem
        existingMenuItem.Title = model.Title;
        existingMenuItem.Url = model.Url;
        existingMenuItem.Icon = model.Icon;
        existingMenuItem.Order = model.Order;
        existingMenuItem.ParentId = model.ParentId;

        _db.MenuItems.Update(existingMenuItem);
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
