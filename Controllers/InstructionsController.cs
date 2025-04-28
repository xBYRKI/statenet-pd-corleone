using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using statenet_lspd.Models;
using statenet_lspd.ViewModels;
using statenet_lspd.Data;

namespace statenet_lspd.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InstructionsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public InstructionsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Instructions
        public async Task<IActionResult> Index()
        {
            var instructions = await _db.ServiceInstructions
                                        .OrderByDescending(si => si.EffectiveDate)
                                        .ToListAsync();
            return View(instructions);
        }

        // GET: Instructions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var instruction = await _db.ServiceInstructions
                                       .FirstOrDefaultAsync(si => si.Id == id);
            if (instruction == null)
                return NotFound();

            return View(instruction);
        }

        // GET: Instructions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Instructions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceInstruction model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.EffectiveDate = DateTime.UtcNow;
            model.IsActive = true;
            _db.ServiceInstructions.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Instructions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var instruction = await _db.ServiceInstructions.FindAsync(id);
            if (instruction == null)
                return NotFound();

            return View(instruction);
        }

        // POST: Instructions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceInstruction model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                _db.ServiceInstructions.Update(model);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceInstructionExists(model.Id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Instructions/Deactivate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            var instruction = await _db.ServiceInstructions.FindAsync(id);
            if (instruction == null)
                return NotFound();

            instruction.IsActive = false;
            _db.ServiceInstructions.Update(instruction);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Instructions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var instruction = await _db.ServiceInstructions
                                       .FirstOrDefaultAsync(si => si.Id == id);
            if (instruction == null)
                return NotFound();

            return View(instruction);
        }

        // POST: Instructions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instruction = await _db.ServiceInstructions.FindAsync(id);
            if (instruction != null)
            {
                _db.ServiceInstructions.Remove(instruction);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceInstructionExists(int id)
        {
            return _db.ServiceInstructions.Any(e => e.Id == id);
        }
    }
}
