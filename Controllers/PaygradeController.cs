// PaygradesController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using statenet_lspd.Models;
using statenet_lspd.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;

namespace statenet_lspd.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class PaygradesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly AuditService _audit;

        public PaygradesController(ApplicationDbContext db, AuditService audit)
        {
            _db = db;
            _audit = audit;
        }

        // GET: /Paygrades
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var paygrades = await _db.Paygrades.ToListAsync();
            var ids = paygrades.Select(pg => pg.Id).ToList();

            var counts = await _db.Users
                .Where(u => ids.Contains(u.Besoldung) && u.Status == true)
                .GroupBy(u => u.Besoldung)
                .Select(g => new { PaygradeId = g.Key, Count = g.Count() })
                .ToListAsync();

            var countDict = counts.ToDictionary(x => x.PaygradeId, x => x.Count);

            var list = paygrades.Select(p => new PaygradeViewModel
            {
                Id            = p.Id,
                Besoldung     = p.Besoldung,
                DiscordRoleId = p.DiscordRoleId,
                UserCount     = countDict.TryGetValue(p.Id, out var c) ? c : 0
            }).ToList();

            return View(list);
        }

        // GET: /Paygrades/Create
        [HttpGet]
        public IActionResult Create()
            => PartialView("_CreateEdit", new PaygradeViewModel());

        // POST: /Paygrades/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaygradeViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = new Paygrade
            {
                Besoldung     = model.Besoldung,
                DiscordRoleId = model.DiscordRoleId
            };
            _db.Paygrades.Add(entity);
            await _db.SaveChangesAsync();

            // Detailliertes Logging
            await _audit.LogAsync(
                "Paygrade.Create",
                $"Besoldungsstufe angelegt: ID={entity.Id}, Besoldung={entity.Besoldung}, DiscordRoleId='{entity.DiscordRoleId}'"
            );

            return RedirectToAction("Index");
        }

        // GET: /Paygrades/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var p = await _db.Paygrades.FindAsync(id);
            if (p == null) return NotFound();

            var vm = new PaygradeViewModel
            {
                Id            = p.Id,
                Besoldung     = p.Besoldung,
                DiscordRoleId = p.DiscordRoleId,
                UserCount     = 0
            };
            return PartialView("_CreateEdit", vm);
        }

        // POST: /Paygrades/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PaygradeViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = await _db.Paygrades.FindAsync(model.Id);
            if (entity == null) return NotFound();

            var changes = new List<string>();
            if (entity.Besoldung != model.Besoldung)
                changes.Add($"Besoldung: {entity.Besoldung}→{model.Besoldung}");
            if (entity.DiscordRoleId != model.DiscordRoleId)
                changes.Add($"DiscordRoleId: '{entity.DiscordRoleId}'→'{model.DiscordRoleId}'");

            entity.Besoldung     = model.Besoldung;
            entity.DiscordRoleId = model.DiscordRoleId;
            _db.Paygrades.Update(entity);
            await _db.SaveChangesAsync();

            if (changes.Any())
            {
                await _audit.LogAsync(
                    "Paygrade.Update",
                    $"Besoldungsstufe aktualisiert: ID={entity.Id}, " + string.Join(", ", changes)
                );
            }

            return RedirectToAction("Index");
        }

        // POST: /Paygrades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await _db.Paygrades.FindAsync(id);
            if (entity == null) return NotFound();

            _db.Paygrades.Remove(entity);
            await _db.SaveChangesAsync();

            await _audit.LogAsync(
                "Paygrade.Delete",
                $"Besoldungsstufe gelöscht: ID={entity.Id}, Besoldung={entity.Besoldung}, DiscordRoleId='{entity.DiscordRoleId}'"
            );

            return RedirectToAction("Index");
        }
    }
}