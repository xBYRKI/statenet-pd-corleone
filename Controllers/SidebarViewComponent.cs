using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using statenet_lspd.Models;
using System.Linq;
using System.Threading.Tasks;

public class SidebarViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public SidebarViewComponent(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        if (user == null) return View(new List<MenuItem>()); // View zurückgeben für MenuItems

        var roleNames = await _userManager.GetRolesAsync(user); // Rollen des Benutzers abrufen

        // Alle MenuItems mit ihren zugeordneten Rollen (Many-to-Many Beziehung) und nach der Order-Eigenschaft sortieren
        var menuItems = await _db.MenuItems
            .Include(mi => mi.MenuItemRoles)  // Alle Rollen für jedes MenuItem laden
            .ThenInclude(mr => mr.Role)      // Die zugehörigen Rollen laden
            .OrderBy(mi => mi.Order)         // Sortiere nach Order-Eigenschaft
            .ToListAsync();

        // Nur erlaubte MenuItems basierend auf den Rollen des Benutzers
        var accessibleItems = menuItems
            .Where(x => !x.MenuItemRoles.Any() || x.MenuItemRoles.Any(mr => roleNames.Contains(mr.Role.Name)))
            .ToList();

        // Baumstruktur für das Menü aufbauen
        var lookup = accessibleItems.ToLookup(i => i.ParentId);
        foreach (var item in accessibleItems)
        {
            item.Children = lookup[item.Id].ToList();
        }

        var rootItems = lookup[null].ToList(); // MenuItems ohne ParentId (root)

        return View(rootItems); // Rückgabe der Wurzelelemente der Menüstruktur
    }
}
