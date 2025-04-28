using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using statenet_lspd.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using statenet_lspd.Data;

namespace statenet_lspd.Controllers
{
    [Authorize(Policy = "HR.View")]
    public class HRController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public HRController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // GET: HR Index
        public async Task<IActionResult> Index(bool showTerminated = false, string search = "", bool onlyDiscord = false, int page = 1)
        {
            IQueryable<ApplicationUser> query = _db.Users;
            if (!showTerminated)
                query = query.Where(u => u.Status == true);
            if (!string.IsNullOrEmpty(search))
                query = query.Where(u => u.Displayname.Contains(search) || u.UserName.Contains(search));
            if (onlyDiscord)
                query = query.Where(u => !string.IsNullOrEmpty(u.DiscordId));

            var count = await query.CountAsync();
            var list = await query
                .OrderBy(u => u.Dienstnummer)
                .Skip((page - 1) * 20)
                .Take(20)
                .ToListAsync();

            ViewBag.PageNumber = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)count / 20);
            ViewBag.ShowTerminated = showTerminated;
            ViewBag.Search = search;
            ViewBag.OnlyDiscord = onlyDiscord;

            return View(list);
        }

        // GET: HR/Hire
        [Authorize(Policy = "HR.Create")]
        public IActionResult Hire()
        {
            var model = new HRAction { ActionType = HRActionType.Hire, EffectiveDate = DateTime.Today };
            return PartialView("_HireModal", model);
        }

        // POST: HR/Hire
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "HR.Create")]
        public async Task<IActionResult> Hire(HRAction model)
        {
            if (!ModelState.IsValid)
                return PartialView("_HireModal", model);

            // Persist action
            _db.HRActions.Add(model);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: HR/Terminate
        [Authorize(Policy = "HR.Delete")]
        public async Task<IActionResult> Terminate(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var model = new HRAction { ActionType = HRActionType.Termination, EffectiveDate = DateTime.Today, UserId = id };
            return PartialView("_TerminateModal", model);
        }

        // POST: HR/Terminate
        [HttpPost, ActionName("Terminate"), ValidateAntiForgeryToken]
        [Authorize(Policy = "HR.Delete")]
        public async Task<IActionResult> TerminateConfirmed(HRAction model)
        {
            if (!ModelState.IsValid)
                return PartialView("_TerminateModal", model);

            var user = await _userManager.FindByIdAsync(model.UserId);
            user.Status = false;
            _db.HRActions.Add(model);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // TODO: Implement Sanction, Promotion, Demotion, Suspension flows similarly
    }
}