// AuditController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using statenet_lspd.Models;
using statenet_lspd.ViewModels;
using System;
using System.Linq;
using statenet_lspd.Data;
using System.Threading.Tasks;

namespace statenet_lspd.Controllers;

[Authorize(Roles = "Mitarbeiter,Admin")]
public class AuditController : Controller
{
    private readonly ApplicationDbContext _context;
    private const int PageSize = 50;

    public AuditController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm, DateTime? from, DateTime? to, int page = 1)
    {
        var query = _context.AuditLogs
            .Include(a => a.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(log =>
                log.User.Displayname.Contains(searchTerm) ||
                log.User.DiscordId.Contains(searchTerm));
        }

        if (from.HasValue)
            query = query.Where(l => l.Timestamp >= from.Value);
        if (to.HasValue)
            query = query.Where(l => l.Timestamp <= to.Value);

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

        var logs = await query
            .OrderByDescending(l => l.Timestamp)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        var model = new AuditLogFilterViewModel
        {
            SearchTerm = searchTerm,
            From       = from,
            To         = to,
            PageNumber = page,
            TotalPages = totalPages,
            Logs       = logs
        };

        return View(model);
    }
}