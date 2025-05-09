using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using statenet_lspd.Data;
using statenet_lspd.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace statenet_lspd.Controllers
{
    [Authorize(Policy = nameof(Permission.HR_View))]
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
        public async Task<IActionResult> Index(
            bool showTerminated = false,
            string search = "",
            bool onlyDiscord = false,
            int page = 1)
        {
            IQueryable<ApplicationUser> query = _db.Users;

            if (!showTerminated)
                query = query.Where(u => u.Status == true);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(u =>
                    u.Displayname.Contains(search) ||
                    u.UserName.Contains(search));

            if (onlyDiscord)
                query = query.Where(u => !string.IsNullOrEmpty(u.DiscordId));

            var count = await query.CountAsync();
            var list = await query
                .OrderBy(u => u.Dienstnummer)
                .Skip((page - 1) * 20)
                .Take(20)
                .ToListAsync();

            ViewBag.PageNumber     = page;
            ViewBag.TotalPages     = (int)Math.Ceiling(count / 20.0);
            ViewBag.ShowTerminated = showTerminated;
            ViewBag.Search         = search;
            ViewBag.OnlyDiscord    = onlyDiscord;

            return View(list);
        }

        // GET: HR/Hire
        [Authorize(Policy = nameof(Permission.HR_Create))]
        public IActionResult Hire()
        {
            var model = new HRAction
            {
                ActionType    = HRActionType.Hire,
                EffectiveDate = DateTime.Today
            };
            return PartialView("_HireModal", model);
        }

        // POST: HR/Hire
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = nameof(Permission.HR_Create))]
        public async Task<IActionResult> Hire(HRAction model)
        {
            if (!ModelState.IsValid)
                return PartialView("_HireModal", model);

            _db.HRActions.Add(model);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: HR/Terminate
        [Authorize(Policy = nameof(Permission.HR_Delete))]
        public async Task<IActionResult> Terminate(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new HRAction
            {
                ActionType    = HRActionType.Termination,
                EffectiveDate = DateTime.Today,
                UserId        = id
            };
            return PartialView("_TerminateModal", model);
        }

        // POST: HR/Terminate
        [HttpPost, ActionName("Terminate"), ValidateAntiForgeryToken]
        [Authorize(Policy = nameof(Permission.HR_Delete))]
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

        // GET: HR/Sanction
        [Authorize(Policy = nameof(Permission.HR_Sanction))]
        public async Task<IActionResult> Sanction(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new HRAction
            {
                ActionType    = HRActionType.Sanction,
                EffectiveDate = DateTime.Today,
                UserId        = id
            };
            return PartialView("_SanctionModal", model);
        }

        // POST: HR/Sanction
        [HttpPost, ActionName("Sanction"), ValidateAntiForgeryToken]
        [Authorize(Policy = nameof(Permission.HR_Sanction))]
        public async Task<IActionResult> SanctionConfirmed(HRAction model)
        {
            if (!ModelState.IsValid)
                return PartialView("_SanctionModal", model);

            _db.HRActions.Add(model);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: HR/Promote
        [Authorize(Policy = nameof(Permission.HR_Promotion))]
        public async Task<IActionResult> Promote(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new HRAction
            {
                ActionType    = HRActionType.Promotion,
                EffectiveDate = DateTime.Today,
                UserId        = id
            };
            return PartialView("_PromotionModal", model);
        }

        // POST: HR/Promote
        [HttpPost, ActionName("Promote"), ValidateAntiForgeryToken]
        [Authorize(Policy = nameof(Permission.HR_Promotion))]
        public async Task<IActionResult> PromoteConfirmed(HRAction model)
        {
            if (!ModelState.IsValid)
                return PartialView("_PromotionModal", model);

            _db.HRActions.Add(model);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: HR/Demote
        [Authorize(Policy = nameof(Permission.HR_Demotion))]
        public async Task<IActionResult> Demote(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new HRAction
            {
                ActionType    = HRActionType.Demotion,
                EffectiveDate = DateTime.Today,
                UserId        = id
            };
            return PartialView("_DemotionModal", model);
        }

        // POST: HR/Demote
        [HttpPost, ActionName("Demote"), ValidateAntiForgeryToken]
        [Authorize(Policy = nameof(Permission.HR_Demotion))]
        public async Task<IActionResult> DemoteConfirmed(HRAction model)
        {
            if (!ModelState.IsValid)
                return PartialView("_DemotionModal", model);

            _db.HRActions.Add(model);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: HR/Suspend
        [Authorize(Policy = nameof(Permission.HR_Suspension))]
        public async Task<IActionResult> Suspend(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new HRAction
            {
                ActionType    = HRActionType.Suspension,
                EffectiveDate = DateTime.Today,
                UserId        = id
            };
            return PartialView("_SuspensionModal", model);
        }

        // POST: HR/Suspend
        [HttpPost, ActionName("Suspend"), ValidateAntiForgeryToken]
        [Authorize(Policy = nameof(Permission.HR_Suspension))]
        public async Task<IActionResult> SuspendConfirmed(HRAction model)
        {
            if (!ModelState.IsValid)
                return PartialView("_SuspensionModal", model);

            _db.HRActions.Add(model);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

            [Authorize(Policy = nameof(Permission.HR_Create))]
        public async Task<IActionResult> HireWizard()
        {
            var vm = new HireWizardViewModel();
            ViewBag.Ranks = await _db.Ranks.OrderBy(r => r.SortOrder).ToListAsync();
            ViewBag.AllPaygrades = Enumerable.Range(1, 20).ToList();
            return View(vm);
        }

        // AJAX: Generate unique Dienstnummer
        [Authorize(Policy = nameof(Permission.HR_Create))]
        [HttpPost]
        public async Task<IActionResult> GenerateDienstnummer()
        {
            var rand = new Random();
            int num;
            do
            {
                num = rand.Next(1000, 10000);
            } while (await _db.Users.AnyAsync(u => u.Dienstnummer == num));
            return Json(num);
        }

        // POST: HR/HireWizard
        [Authorize(Policy = nameof(Permission.HR_Create))]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> HireWizard(HireWizardViewModel vm)
        {
            ViewBag.Ranks = await _db.Ranks.OrderBy(r => r.SortOrder).ToListAsync();
            ViewBag.AllPaygrades = Enumerable.Range(1, 20).ToList();

            if (!ModelState.IsValid)
                return View(vm);

            var rank = await _db.Ranks.FindAsync(vm.Step2.RankId);
            if (vm.Step2.Paygrade < rank.MinPayGrade || vm.Step2.Paygrade > rank.MaxPayGrade)
            {
                ModelState.AddModelError(nameof(vm.Step2.Paygrade),
                    $"Die Besoldung muss zwischen {rank.MinPayGrade} und {rank.MaxPayGrade} liegen.");
                return View(vm);
            }
            if (vm.Step2.Paygrade > rank.MaxPayGrade && string.IsNullOrWhiteSpace(vm.Step2.PaygradeReason))
            {
                ModelState.AddModelError(nameof(vm.Step2.PaygradeReason),
                    "Bitte gib einen Grund an, wenn die Besoldung über der Maximalbesoldung liegt.");
                return View(vm);
            }

            var user = new ApplicationUser
            {
                Displayname = vm.Step1.FullName,
                Dienstnummer = vm.Step1.Dienstnummer!.Value,
                Birthday = vm.Step1.Birthday,
                Phone = vm.Step1.Phone,
                RankId = vm.Step2.RankId,
                Besoldung = vm.Step2.Paygrade.Value,
                Status = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError("", err.Description);
                return View(vm);
            }

            _db.HRActions.Add(new HRAction
            {
                ActionType = HRActionType.Hire,
                EffectiveDate = DateTime.Today,
                UserId = user.Id
            });
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }

}
