using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using statenet_lspd.Data;
using statenet_lspd.Models;
using statenet_lspd.ViewModels;

namespace statenet_lspd.Controllers
{
    public class AccountController : BaseController
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuditService _audit;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AuditService audit,
            ILogger<AccountController> logger
        ) : base(db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _audit = audit;
            _logger = logger;
        }

        // Discord Login
        [HttpGet, AllowAnonymous]
        public IActionResult ExternalLogin(string provider = "Discord", string? returnUrl = null)
        {
            if (string.IsNullOrEmpty(returnUrl) || returnUrl == Url.Action(nameof(ExternalLoginCallback), "Account"))
                returnUrl = Url.Content("~/");

            var props = _signInManager.ConfigureExternalAuthenticationProperties(
                provider,
                Url.Action(nameof(ExternalLoginCallback), "Account", new { ReturnUrl = returnUrl })
            );
            return Challenge(props, provider);
        }

        // Discord Callback
        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            _logger.LogInformation("ExternalLoginCallback gestartet.");
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                _logger.LogError("ExternalLoginInfo war null.");
                return RedirectToAction("Login");
            }

            // Prüfen vorhandenen User
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.DiscordId == info.ProviderKey);

            if (user != null)
            {
                // Anmeldung des bestehenden Users
                await _signInManager.SignInAsync(user, isPersistent: false);
                user.LastLogin = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
                await _audit.LogAsync("User.Login", $"{user.Displayname} hat sich eingeloggt.");

                // Prüfen auf offene Dienstanweisungen
                bool hasPending = _db.ServiceInstructions
                    .Any(si => si.IsActive
                               && !_db.UserInstructionAcceptances
                                   .Any(a => a.UserId == user.Id && a.ServiceInstructionId == si.Id));
                if (hasPending)
                    return RedirectToAction(nameof(ConfirmInstructions));

                return RedirectToAction("Index", "Home");
            }

            // Neuer User: Registrierung nur für LSPD-Mitarbeiter
            bool isUserInLSPD = false; // TODO: Implementiere LSPD-Check
            if (!isUserInLSPD)
            {
                _logger.LogError("Benutzer ist nicht im LSPD.");
                return View("NotInLSPD");
            }

            // User anlegen
            user = new ApplicationUser
            {
                UserName = info.Principal.Identity.Name,
                DiscordId = info.ProviderKey,
                LastLogin = DateTime.UtcNow
            };
            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                _logger.LogError("Fehler bei Benutzererstellung: {0}",
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));
                return View("Error");
            }

            // Externen Login hinzufügen und Rolle zuweisen
            await _userManager.AddLoginAsync(user, info);
            await _userManager.AddToRoleAsync(user, "Mitarbeiter");
            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("Benutzer {0} wurde registriert und eingeloggt.", user.UserName);

            // Nach Neuregistrierung: Dienstanweisungen prüfen
            bool hasPendingNew = _db.ServiceInstructions
                .Any(si => si.IsActive
                           && !_db.UserInstructionAcceptances
                               .Any(a => a.UserId == user.Id && a.ServiceInstructionId == si.Id));
            if (hasPendingNew)
                return RedirectToAction(nameof(ConfirmInstructions));

            return RedirectToAction("Index", "Home");
        }

        // Confirm Instructions - GET
        [HttpGet, Authorize]
        public IActionResult ConfirmInstructions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pending = _db.ServiceInstructions
                .Where(si => si.IsActive)
                .Where(si => !_db.UserInstructionAcceptances
                    .Any(a => a.UserId == userId && a.ServiceInstructionId == si.Id))
                .OrderBy(si => si.EffectiveDate)
                .ToList();
            if (!pending.Any())
                return RedirectToAction("Index", "Home");

            return View(new ConfirmInstructionsViewModel { PendingInstructions = pending });
        }

        // Confirm Instructions - POST
        [HttpPost, Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmInstructions(List<int>? instructionIds)
        {
            if (instructionIds == null || !instructionIds.Any())
                return RedirectToAction(nameof(ConfirmInstructions));

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var now = DateTime.UtcNow;
            foreach (var id in instructionIds)
            {
                _db.UserInstructionAcceptances.Add(new UserInstructionAcceptance
                {
                    UserId = userId,
                    ServiceInstructionId = id,
                    AcceptedAt = now
                });
            }
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        // Edit Profile - GET
        [HttpGet, Authorize]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            var vm = new EditProfileViewModel
            {
                Birthday = user.Birthday,
                Phone = user.Phone.ToString()
            };
            return PartialView("_EditProfilePartial", vm);
        }

        // Edit Profile - POST
        [HttpPost, Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var oldBirthday = user.Birthday;
            var oldPhone = user.Phone;

            user.Birthday = model.Birthday;
            user.Phone = model.Phone;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(ModelState);

            var changes = new List<string>();
            if (oldBirthday != user.Birthday)
                changes.Add($"Geburtstag: '{oldBirthday:dd.MM.yyyy}' → '{user.Birthday:dd.MM.yyyy}'");
            if (oldPhone != user.Phone)
                changes.Add($"Telefon: '{oldPhone}' → '{user.Phone}'");

            if (changes.Any())
                await _audit.LogAsync("User.ProfileUpdate", $"Profil aktualisiert bei {user.Displayname}: {string.Join(", ", changes)}");

            TempData["Success"] = "Profil erfolgreich aktualisiert";
            return RedirectToAction("Profile");
        }

        // Profile
        [HttpGet, Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");
            return View(user);
        }

        // Access Denied
        [HttpGet, AllowAnonymous]
        public IActionResult AccessDenied()
        {
            _logger.LogWarning("Zugriff verweigert.");
            return View();
        }

        // Logout
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                await _audit.LogAsync("User.Logout", $"{user.Displayname} hat sich abgemeldet.");
                _logger.LogInformation("{User} abgemeldet.", user.Displayname);
            }
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}