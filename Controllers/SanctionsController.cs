using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using statenet_lspd.Models;

namespace statenet_lspd.Controllers;

public class SanctionsController : Controller
{
    
    private readonly ApplicationDbContext _db;
    
    public SanctionsController(ApplicationDbContext db)
    {
        _db = db;
    }


    //
    // SANKTIONSKATALOG
    //
    //

    [AllowAnonymous]
    public async Task<IActionResult> Index(string searchTerm = "")
    {
        // Daten abfragen und optional filtern
        var query = _db.Sanktionen.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(s =>
                EF.Functions.Like(s.Vergehen, $"%{searchTerm}%") ||
                EF.Functions.Like(s.Beschreibung, $"%{searchTerm}%") ||
                EF.Functions.Like(s.Kategorie, $"%{searchTerm}%"));
        }

        var sanctions = await query.ToListAsync();
        ViewData["SearchTerm"] = searchTerm;
        return View(sanctions);
    }

}