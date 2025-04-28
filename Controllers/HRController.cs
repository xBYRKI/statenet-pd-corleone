using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using statenet_lspd.Models;
using statenet_lspd.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace statenet_lspd.Controllers
{
    [Authorize(Roles = "HR, Mitarbeiter")]
    [Route("HR/[action]")]
    public class HRController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly AuditService _audit;

        public HRController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            AuditService audit)
        {
            _userManager = userManager;
            _context = context;
            _audit = audit;
        }

        // Liste aller Mitarbeiter
        // UsersController.cs → HRController.cs

    // Index bekommt einen Parameter showTerminated (default = false)
        public async Task<IActionResult> Index(bool showTerminated = false)
        {
            // Wir zeigen je nach Parameter nur Status = true oder Status = false
            var usersQuery = _userManager.Users
                .Where(u => (u.Status ?? false) == !showTerminated)
                .OrderBy(u => u.Displayname);

            var users = await usersQuery.ToListAsync();

            ViewBag.ShowTerminated = showTerminated;
            return View(users);
        }


        // GET: HR/Create
        [HttpGet]
        public IActionResult Create()
        {
            var vm = new EditUserViewModel
            {
                Dienstnummer = GenerateUniqueDienstnummer(),
                // weitere Default-Werte
            };
            return View(vm);
        }

        // POST: HR/Create
        [HttpPost]
        public async Task<IActionResult> Create(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Displayname = model.Displayname,
                Dienstnummer = model.Dienstnummer,
                Status = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                foreach(var err in result.Errors)
                    ModelState.AddModelError("", err.Description);
                return View(model);
            }

            await _audit.LogAsync("HR.Create", $"Mitarbeiter {user.Displayname} angelegt.");
            return RedirectToAction(nameof(Index));
        }

        // GET: HR/Edit/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var vm = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName ?? "Unbekannt",
                Displayname = user.Displayname,
                Dienstnummer = user.Dienstnummer,
                DiscordId = user.DiscordId,
                Besoldung = user.Besoldung,
                Status = user.Status ?? false,
                Phone = user.Phone.ToString(),
                FiredAt = user.FiredAt,
                Birthday = user.Birthday,
                TotalHours = user.TotalHours,
                AllRoles = new List<SelectListItem>()
            };
            return View(vm);
        }

        // POST: HR/Edit
        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            user.Displayname = model.Displayname;
            user.DiscordId = model.DiscordId;
            user.Besoldung = model.Besoldung;
            user.Status = model.Status;
            user.Phone = int.Parse(model.Phone ?? "0");
            user.FiredAt = model.FiredAt;
            user.Birthday = model.Birthday;
            user.TotalHours = model.TotalHours;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError("", err.Description);
                return View(model);
            }

            await _audit.LogAsync("HR.Edit", $"Mitarbeiter {user.Displayname} bearbeitet.");
            return RedirectToAction(nameof(Index));
        }

        // POST: HR/Terminate/5
        [HttpPost("Terminate/{id}")]
        public async Task<IActionResult> Terminate(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            user.Status = false;
            await _userManager.UpdateAsync(user);
            // optional: lockout for login
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;
            await _userManager.UpdateAsync(user);
            await _audit.LogAsync("HR.Terminate", $"Mitarbeiter {user.Displayname} gekündigt.");
            return RedirectToAction(nameof(Index));
        }

        // TODO: Sanktionieren, Befördern, Degradieren
        // hier später Actions implementieren

        private int GenerateUniqueDienstnummer()
        {
            var used = _context.Users.Where(u => u.Dienstnummer != 0).Select(u => u.Dienstnummer).ToHashSet();
            var rand = new Random();
            int num;
            do { num = rand.Next(1000, 10000); } while (used.Contains(num));
            return num;
        }
    }
}