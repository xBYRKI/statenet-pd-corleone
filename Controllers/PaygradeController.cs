using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using statenet_lspd.Data;
using statenet_lspd.Models;
using statenet_lspd.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace statenet_lspd.Controllers
{
    [Authorize(Policy = nameof(Permission.Paygrade_View))]
    public class PaygradesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly AuditService _audit;
        private readonly IAuthorizationService _auth;

        public PaygradesController(
            ApplicationDbContext db,
            AuditService audit,
            IAuthorizationService auth)
        {
            _db    = db;
            _audit = audit;
            _auth  = auth;
        }

        // LIST
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // 1. Daten laden
            var paygrades = await _db.Paygrades.ToListAsync();
            var ids       = paygrades.Select(pg => pg.Id).ToList();

            var counts = await _db.Users
                .Where(u => ids.Contains(u.Besoldung) && u.Status == true)
                .GroupBy(u => u.Besoldung)
                .Select(g => new { PaygradeId = g.Key, Count = g.Count() })
                .ToListAsync();

            var countDict = counts.ToDictionary(x => x.PaygradeId, x => x.Count);

            var vmList = paygrades.Select(p => new PaygradeViewModel
            {
                Id            = p.Id,
                Besoldung     = p.Besoldung,
                DiscordRoleId = p.DiscordRoleId,
                UserCount     = countDict.GetValueOrDefault(p.Id, 0)
            }).ToList();

            // 2. Rechte abfragen
            ViewData["CanCreate"] = (await _auth.AuthorizeAsync(User, nameof(Permission.Paygrade_Create))).Succeeded;
            ViewData["CanEdit"]   = (await _auth.AuthorizeAsync(User, nameof(Permission.Paygrade_Edit))).Succeeded;
            ViewData["CanDelete"] = (await _auth.AuthorizeAsync(User, nameof(Permission.Paygrade_Delete))).Succeeded;

            return View(vmList);
        }

        // CREATE – GET
        [Authorize(Policy = nameof(Permission.Paygrade_Create))]
        [HttpGet]
        public IActionResult Create()
            => PartialView("_CreateEdit", new PaygradeViewModel());

        // CREATE – POST
        [Authorize(Policy = nameof(Permission.Paygrade_Create))]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaygradeViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = new Paygrade
            {
                Besoldung     = model.Besoldung,
                DiscordRoleId = model.DiscordRoleId
            };

            _db.Paygrades.Add(entity);
            await _db.SaveChangesAsync();

            await _audit.LogAsync(
                "Paygrade.Created",
                $"Paygrade angelegt: ID={entity.Id}, Besoldung={entity.Besoldung}, DiscordRoleId='{entity.DiscordRoleId}'"
            );

            return RedirectToAction(nameof(Index));
        }

        // EDIT – GET
        [Authorize(Policy = nameof(Permission.Paygrade_Edit))]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var p = await _db.Paygrades.FindAsync(id);
            if (p == null) 
                return NotFound();

            var vm = new PaygradeViewModel
            {
                Id            = p.Id,
                Besoldung     = p.Besoldung,
                DiscordRoleId = p.DiscordRoleId,
                UserCount     = 0
            };
            return PartialView("_CreateEdit", vm);
        }

        // EDIT – POST
        [Authorize(Policy = nameof(Permission.Paygrade_Edit))]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PaygradeViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = await _db.Paygrades.FindAsync(model.Id);
            if (entity == null) 
                return NotFound();

            var changes = new List<string>();
            if (entity.Besoldung     != model.Besoldung)
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
                    "Paygrade.Updated",
                    $"Paygrade aktualisiert: ID={entity.Id}, {string.Join(", ", changes)}"
                );
            }

            return RedirectToAction(nameof(Index));
        }

        // DELETE – POST
        [Authorize(Policy = nameof(Permission.Paygrade_Delete))]
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await _db.Paygrades.FindAsync(id);
            if (entity == null) 
                return NotFound();

            _db.Paygrades.Remove(entity);
            await _db.SaveChangesAsync();

            await _audit.LogAsync(
                "Paygrade.Deleted",
                $"Paygrade gelöscht: ID={entity.Id}, Besoldung={entity.Besoldung}, DiscordRoleId='{entity.DiscordRoleId}'"
            );

            return RedirectToAction(nameof(Index));
        }
    }
}
