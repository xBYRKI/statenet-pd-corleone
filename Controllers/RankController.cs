using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using statenet_lspd.Models;
using statenet_lspd.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;
using statenet_lspd.Data;

namespace statenet_lspd.Controllers
{
    [Authorize(Policy = nameof(Permission.Rang_View))]
    public class RanksController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly AuditService _audit;
        private const int PageSize = 20;

        public RanksController(ApplicationDbContext db, AuditService audit)
        {
            _db = db;
            _audit = audit;
        }

        // GET: /Ranks
        [Authorize(Policy = nameof(Permission.Rang_View))]
        [HttpGet]
        public async Task<IActionResult> Index(string? sortField, string? sortDir, int page = 1)
        {
            IQueryable<Rank> query = _db.Ranks;
            // Sorting
            sortField ??= "SortOrder";
            sortDir   ??= "asc";
            query = (sortField, sortDir.ToLower()) switch
            {
                ("Name", "desc")      => query.OrderByDescending(r => r.Name),
                ("Name", _)             => query.OrderBy(r => r.Name),
                ("SortOrder", "desc") => query.OrderByDescending(r => r.SortOrder),
                _                        => query.OrderBy(r => r.SortOrder),
            };
            // Paging
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
            var entities  = await query.Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();

            // Count users per rank
            var rankIds = entities.Select(r => r.Id).ToList();
            var counts = await _db.Users
                .Where(u => u.RankId != null && rankIds.Contains(u.RankId.Value) && u.Status == true )
                .GroupBy(u => u.RankId.Value)
                .Select(g => new { RankId = g.Key, Count = g.Count() })
                .ToListAsync();
            var countDict = counts.ToDictionary(x => x.RankId, x => x.Count);

            // Map to VM
            var vmList = entities.Select(r => new RankViewModel
            {
                Id              = r.Id,
                Name            = r.Name,
                SortOrder       = r.SortOrder,
                DiscordRoleId   = r.DiscordRoleId,
                ColorHex        = r.ColorHex,
                MinPayGrade     = r.MinPayGrade,
                MaxPayGrade     = r.MaxPayGrade,
                UserCount       = countDict.TryGetValue(r.Id, out var c) ? c : 0
            }).ToList();

            ViewBag.SortField  = sortField;
            ViewBag.SortDir    = sortDir;
            ViewBag.PageNumber = page;
            ViewBag.TotalPages = totalPages;

            return View(vmList);
        }

        // GET: /Ranks/Create
        [HttpGet]
        [Authorize(Policy = nameof(Permission.Rang_Add))]        
        public IActionResult Create()
            => PartialView("_CreateEdit", new RankViewModel());

        // POST: /Ranks/Create
        [Authorize(Policy = nameof(Permission.Rang_Add))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RankViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var entity = new Rank
            {
                Name          = model.Name,
                SortOrder     = model.SortOrder,
                DiscordRoleId = model.DiscordRoleId,
                ColorHex      = model.ColorHex,
                MinPayGrade   = model.MinPayGrade,
                MaxPayGrade   = model.MaxPayGrade
            };
            _db.Ranks.Add(entity);
            await _db.SaveChangesAsync();
            await _audit.LogAsync("Rank.Create", $"Rang '{entity.Name}' (ID {entity.Id}) angelegt. MinPayGrade={entity.MinPayGrade}, MaxPayGrade={entity.MaxPayGrade}");
            return RedirectToAction("Index");
        }

        // GET: /Ranks/Edit/5
        [Authorize(Policy = nameof(Permission.Rang_Edit))]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var r = await _db.Ranks.FindAsync(id);
            if (r == null) return NotFound();
            var vm = new RankViewModel
            {
                Id              = r.Id,
                Name            = r.Name,
                SortOrder       = r.SortOrder,
                DiscordRoleId   = r.DiscordRoleId,
                ColorHex        = r.ColorHex,
                MinPayGrade     = r.MinPayGrade,
                MaxPayGrade     = r.MaxPayGrade
            };
            return PartialView("_CreateEdit", vm);
        }

        // POST: /Ranks/Edit/5
        [Authorize(Policy = nameof(Permission.Rang_Edit))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RankViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var entity = await _db.Ranks.FindAsync(model.Id);
            if (entity == null) return NotFound();
            var changes = new List<string>();
            if (entity.Name != model.Name)
                changes.Add($"Name: '{entity.Name}'→'{model.Name}'");
            if (entity.SortOrder != model.SortOrder)
                changes.Add($"SortOrder: {entity.SortOrder}→{model.SortOrder}");
            if (entity.DiscordRoleId != model.DiscordRoleId)
                changes.Add($"DiscordRoleId: '{entity.DiscordRoleId}'→'{model.DiscordRoleId}'");
            if (entity.ColorHex != model.ColorHex)
                changes.Add($"ColorHex: '{entity.ColorHex}'→'{model.ColorHex}'");
            if (entity.MinPayGrade != model.MinPayGrade)
                changes.Add($"MinPayGrade: {entity.MinPayGrade}→{model.MinPayGrade}");
            if (entity.MaxPayGrade != model.MaxPayGrade)
                changes.Add($"MaxPayGrade: {entity.MaxPayGrade}→{model.MaxPayGrade}");
            entity.Name          = model.Name;
            entity.SortOrder     = model.SortOrder;
            entity.DiscordRoleId = model.DiscordRoleId;
            entity.ColorHex      = model.ColorHex;
            entity.MinPayGrade   = model.MinPayGrade;
            entity.MaxPayGrade   = model.MaxPayGrade;
            _db.Ranks.Update(entity);
            await _db.SaveChangesAsync();
            if (changes.Any())
                await _audit.LogAsync("Rank.Update", $"Rang (ID {entity.Id}) aktualisiert: {string.Join(", ", changes)}");
            return RedirectToAction("Index");
        }

        // GET: /Ranks/Delete/5
        [Authorize(Policy = nameof(Permission.Rang_Delete))]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var r = await _db.Ranks.FindAsync(id);
            if (r == null) return NotFound();
            bool hasUsers = await _db.Users.AnyAsync(u => u.RankId == id);
            ViewBag.HasUsers = hasUsers;
            var vm = new RankViewModel { Id = r.Id, Name = r.Name };
            return PartialView("_Delete", vm);
        }

        // POST: /Ranks/Delete/5
        [Authorize(Policy = nameof(Permission.Rang_Delete))]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            var entity = await _db.Ranks.FindAsync(id);
            if (entity == null) return NotFound();
            if (await _db.Users.AnyAsync(u => u.RankId == id))
                return BadRequest("Dieser Rang wird noch von Benutzern verwendet.");
            _db.Ranks.Remove(entity);
            await _db.SaveChangesAsync();
            await _audit.LogAsync("Rank.Delete", $"Rang '{entity.Name}' (ID {entity.Id}) gelöscht.");
            return RedirectToAction("Index");
        }
    }
}