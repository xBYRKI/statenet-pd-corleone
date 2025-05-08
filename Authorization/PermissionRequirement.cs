using Microsoft.AspNetCore.Authorization;
using statenet_lspd.Models;

public class PermissionRequirement : IAuthorizationRequirement
{
    public Permission Permission { get; }
    public PermissionRequirement(Permission permission) => Permission = permission;
}
