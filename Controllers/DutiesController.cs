using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using statenet_lspd.Models;
using statenet_lspd.ViewModels;
using statenet_lspd.Data;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace statenet_lspd.Controllers
{
    [Authorize]
    public class DutiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DutiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string periodValue = "", string sortField = "StartTime", string sortDir = "asc")
        {
            var now = DateTime.Now;
            var culture = CultureInfo.CurrentCulture;

            if (string.IsNullOrEmpty(periodValue))
            {
                int currentWeek = culture.Calendar.GetWeekOfYear(now, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
                periodValue = currentWeek.ToString();
            }

            var dutiesQuery = _context.Duties.AsNoTracking();

            if (int.TryParse(periodValue, out var weekNum))
            {
                var firstDayOfYear = new DateTime(now.Year, 1, 1);
                var calendar = culture.Calendar;
                var firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;
                var weekStart = firstDayOfYear.AddDays((weekNum - 1) * 7);
                while (calendar.GetDayOfWeek(weekStart) != firstDayOfWeek)
                    weekStart = weekStart.AddDays(-1);
                var weekEnd = weekStart.AddDays(7);
                dutiesQuery = dutiesQuery.Where(d => d.StartTime.HasValue && d.StartTime.Value >= weekStart && d.StartTime.Value < weekEnd);
            }

            var activeUsers = _context.Users
                .Where(u => u.Status == true)
                .Select(u => new { u.DiscordId, u.Displayname });

            var joined = from d in dutiesQuery
                         join u in activeUsers on d.DiscordId equals u.DiscordId
                         select new DutyViewModel
                         {
                             Id = d.Id,
                             DiscordId = d.DiscordId,
                             OfficerDisplayName = u.Displayname,
                             RightAnswer = d.RightAnswer,
                             StartTime = d.StartTime,
                             EndTime = d.EndTime
                         };

            joined = (sortField, sortDir) switch
            {
                ("DiscordId", "asc")  => joined.OrderBy(x => x.DiscordId),
                ("DiscordId", "desc") => joined.OrderByDescending(x => x.DiscordId),
                ("EndTime", "asc")    => joined.OrderBy(x => x.EndTime),
                ("EndTime", "desc")   => joined.OrderByDescending(x => x.EndTime),
                ("StartTime", "desc") => joined.OrderByDescending(x => x.StartTime),
                _                        => joined.OrderBy(x => x.StartTime),
            };

            var duties = await joined.ToListAsync();

            var totalDuration = duties
                .Where(d => d.StartTime.HasValue && d.EndTime.HasValue)
                .Aggregate(TimeSpan.Zero, (sum, d) => sum + (d.EndTime.Value - d.StartTime.Value));

            ViewBag.PeriodValue = periodValue;
            ViewBag.SortField = sortField;
            ViewBag.SortDir = sortDir;
            ViewBag.TotalDuration = totalDuration;

            return View(duties);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Duty duty)
        {
            if (ModelState.IsValid)
            {
                _context.Add(duty);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(duty);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var duty = await _context.Duties.FindAsync(id);
            if (duty == null) return NotFound();
            return View(duty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Duty duty)
        {
            if (id != duty.Id) return BadRequest();
            if (ModelState.IsValid)
            {
                _context.Update(duty);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(duty);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var duty = await _context.Duties.FindAsync(id);
            if (duty == null) return NotFound();
            return View(duty);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var duty = await _context.Duties.FindAsync(id);
            if (duty != null)
                _context.Duties.Remove(duty);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}