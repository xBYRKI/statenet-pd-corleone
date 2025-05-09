using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using statenet_lspd.Data;
using statenet_lspd.Models;
using statenet_lspd.ViewModels;

namespace statenet_lspd.Controllers
{
    // Nur Benutzer mit Unit_View-Recht dürfen die Übersicht sehen
    [Authorize(Policy = nameof(Permission.Unit_View))]
    public class UnitsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly AuditService _audit;
        private const int PageSize = 10;

        public UnitsController(
            ApplicationDbContext context,
            IAuthorizationService authorizationService,
            AuditService audit)
        {
            _context = context;
            _authorizationService = authorizationService;
            _audit = audit;
        }

        // GET: /Units
        public async Task<IActionResult> Index(string sortField = "Name", string sortDir = "asc", int page = 1)
        {
            var baseQuery = _context.Units
                .Select(u => new UnitViewModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    Description = u.Description,
                    UserCount = u.Users.Count
                });

            // Sorting
            baseQuery = sortField switch
            {
                "Name" => sortDir == "asc"
                    ? baseQuery.OrderBy(u => u.Name)
                    : baseQuery.OrderByDescending(u => u.Name),
                _ => baseQuery.OrderBy(u => u.Name)
            };

            var totalItems = await baseQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            var items = await baseQuery
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.SortField = sortField;
            ViewBag.SortDir = sortDir;
            ViewBag.PageNumber = page;
            ViewBag.TotalPages = totalPages;

            return View(items);
        }

        // GET: /Units/Create
        [Authorize(Policy = nameof(Permission.Unit_Add))]
        public IActionResult Create()
        {
            return PartialView("_CreateUnitModal", new UnitViewModel());
        }

        // POST: /Units/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = nameof(Permission.Unit_Add))]
        public async Task<IActionResult> Create(UnitViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_CreateUnitModal", model);

            var unit = new Unit
            {
                Name = model.Name,
                Description = model.Description
            };
            _context.Units.Add(unit);
            await _context.SaveChangesAsync();

            await _audit.LogAsync("Unit.Created", $"Unit {unit.Name} wurde angelegt.");
            return Json(new { success = true });
        }

        // GET: /Units/Edit/5
        [Authorize(Policy = nameof(Permission.Unit_Edit))]
        public async Task<IActionResult> Edit(int id)
        {
            var unit = await _context.Units.FindAsync(id);
            if (unit == null) return NotFound();

            var model = new UnitViewModel
            {
                Id = unit.Id,
                Name = unit.Name,
                Description = unit.Description,
                UserCount = unit.Users.Count
            };
            return PartialView("_EditUnitModal", model);
        }

        // POST: /Units/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = nameof(Permission.Unit_Edit))]
        public async Task<IActionResult> Edit(int id, UnitViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return PartialView("_EditUnitModal", model);

            var unit = await _context.Units.FindAsync(id);
            if (unit == null) return NotFound();

            unit.Name = model.Name;
            unit.Description = model.Description;

            _context.Units.Update(unit);
            await _context.SaveChangesAsync();

            await _audit.LogAsync("Unit.Edited", $"Unit {unit.Name} wurde bearbeitet.");
            return Json(new { success = true });
        }

        // GET: /Units/Delete/5
        [Authorize(Policy = nameof(Permission.Unit_Delete))]
        public async Task<IActionResult> Delete(int id)
        {
            var unit = await _context.Units
                .Include(u => u.Users)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (unit == null) return NotFound();

            var model = new UnitViewModel
            {
                Id = unit.Id,
                Name = unit.Name,
                Description = unit.Description,
                UserCount = unit.Users.Count
            };
            return PartialView("_DeleteUnitModal", model);
        }

        // POST: /Units/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = nameof(Permission.Unit_Delete))]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var unit = await _context.Units
                .Include(u => u.Users)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (unit == null) return NotFound();

            if (unit.Users.Any())
                return BadRequest("Cannot delete a unit with members.");

            _context.Units.Remove(unit);
            await _context.SaveChangesAsync();

            await _audit.LogAsync("Unit.Deleted", $"Unit {unit.Name} wurde gelöscht.");
            return Json(new { success = true });
        }
    }
}