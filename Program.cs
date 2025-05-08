using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using statenet_lspd.Data;
using statenet_lspd.Models;
using System;

var builder = WebApplication.CreateBuilder(args);

// Serilog-Konfiguration
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft.AspNetCore.Authorization", Serilog.Events.LogEventLevel.Debug)
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

// MySQL-Datenbankverbindung
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), mySqlOptions =>
    {
        mySqlOptions.EnableRetryOnFailure();
    })
);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity-Konfiguration
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = false;
    // weitere Password-Optionen bei Bedarf
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Cookie-Pfade
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/ExternalLogin";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// AuditService und HttpContextAccessor
builder.Services.AddScoped<AuditService>();
builder.Services.AddHttpContextAccessor();

// Discord OAuth2
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    options.DefaultChallengeScheme = DiscordAuthenticationDefaults.AuthenticationScheme;
})
.AddDiscord(options =>
{
    options.ClientId     = builder.Configuration["Authentication:Discord:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Discord:ClientSecret"]!;
    options.CallbackPath = "/signin-discord";
    options.Scope.Add("identify");
    options.Scope.Add("guilds");
});

// MVC + Razor
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Custom AuthorizationHandler registrieren
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

// Policies mit PermissionRequirement definieren
builder.Services.AddAuthorization(options =>
{
    foreach (Permission perm in Enum.GetValues(typeof(Permission)))
    {
        // Policy-Name = Enum-Name, Requirement = entsprechendes Permission
        options.AddPolicy(
            perm.ToString(),
            policy => policy.AddRequirements(new PermissionRequirement(perm))
        );
    }
});


var app = builder.Build();

// Migrationen anwenden
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// Middleware-Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

if (!app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.All
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Public controllers
app.MapControllerRoute(
    name: "public",
    pattern: "Public/{action=Index}/{id?}",
    defaults: new { controller = "Public", action = "Index" }
).WithMetadata(new AllowAnonymousAttribute());

// Account (Login / AccessDenied)
app.MapControllerRoute(
    name: "login",
    pattern: "Account/{action=ExternalLogin}/{returnUrl?}",
    defaults: new { controller = "Account", action = "ExternalLogin" }
).WithMetadata(new AllowAnonymousAttribute());

// Default route – hier greifen deine Policies
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.MapRazorPages();

app.Run();