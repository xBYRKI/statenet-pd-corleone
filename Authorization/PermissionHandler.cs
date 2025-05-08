using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using statenet_lspd.Data;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _http;
    private readonly ILogger<PermissionHandler> _logger;

    public PermissionHandler(
        ApplicationDbContext db,
        IHttpContextAccessor http,
        ILogger<PermissionHandler> logger)
    {
        _db = db;
        _http = http;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogDebug("→ Prüfe Permission {Permission} für User {User}", requirement.Permission, userId);

        if (userId == null)
        {
            _logger.LogWarning("Kein NameIdentifier-Claim gefunden, breche ab.");
            return;
        }

        var roleIds = await _db.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        _logger.LogDebug("User‐Rollen: {Roles}", string.Join(", ", roleIds));

        var has = await _db.RolePermissions
            .AnyAsync(rp =>
                roleIds.Contains(rp.RoleId) &&
                rp.Permission == requirement.Permission);

        _logger.LogDebug("Hat das Permission? {Has}", has);

        if (has)
        {
            _logger.LogInformation("→ Authorization SUCCEEDED für {Permission}", requirement.Permission);
            context.Succeed(requirement);
        }
        else
        {
            _logger.LogInformation("→ Authorization FAILED für {Permission}", requirement.Permission);
        }
    }
}
