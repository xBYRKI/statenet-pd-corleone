// Controllers/SanktionenController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using statenet_lspd.Models;

public class SanktionenController : Controller
{
    private readonly ApplicationDbContext _db;
    public SanktionenController(ApplicationDbContext db) => _db = db;

    // Management-Übersicht
    public async Task<IActionResult> Index()
    {
        var list = await _db.Sanktionen
            .OrderBy(s => s.SortOrder)
            .ToListAsync();
        return View(list);
    }

    // AJAX-Endpunkte für Modals

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Categories = _db.Sanktionen
            .Select(s => s.Kategorie)
            .Distinct()
            .ToList();
        return PartialView("_CreateSanktion", new Sanktion());
    }


    [HttpPost]
    public async Task<IActionResult> Create(Sanktion model)
    {
        if (!ModelState.IsValid) return PartialView("_CreateSanktion", model);
        // Setze SortOrder ans Ende
        var max = await _db.Sanktionen.MaxAsync(s => (int?)s.SortOrder) ?? 0;
        model.SortOrder = max + 1;
        _db.Sanktionen.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction("Index");

    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var s = await _db.Sanktionen.FindAsync(id);
        if (s == null) 
            return NotFound();

        // Bestehende Kategorien für das Datalist
        ViewBag.Categories = await _db.Sanktionen
            .Select(x => x.Kategorie)
            .Distinct()
            .ToListAsync();

        return PartialView("_EditSanktion", s);
    }


    [HttpPost]
    public async Task<IActionResult> Edit(Sanktion model)
    {
        if (!ModelState.IsValid) return PartialView("_EditSanktion", model);
        _db.Sanktionen.Update(model);
        await _db.SaveChangesAsync();
        return RedirectToAction("Index");

    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var s = await _db.Sanktionen.FindAsync(id);
        if (s == null) return NotFound();
        return PartialView("_DeleteSanktion", s);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var s = await _db.Sanktionen.FindAsync(id);
        if (s != null)
        {
            _db.Sanktionen.Remove(s);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction("Index");

    }

    // Sortierung per Drag & Drop abspeichern
    [HttpPost]
    public async Task<IActionResult> UpdateOrder([FromBody] int[] orderedIds)
    {
        for (int i = 0; i < orderedIds.Length; i++)
        {
            var s = await _db.Sanktionen.FindAsync(orderedIds[i]);
            if (s != null) s.SortOrder = i;
        }
        await _db.SaveChangesAsync();
        return Ok();
    }
}
