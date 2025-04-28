using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using statenet_lspd.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Serilog-Konfiguration
Log.Logger = new LoggerConfiguration()
    
    .CreateLogger();
builder.Host.UseSerilog();

   /* .WriteTo.File("./log/log.txt",
        rollingInterval: RollingInterval.Day,
        shared: true) */
// 🔌 MySQL-Datenbankverbindung
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), mySqlOptions =>
    {
        mySqlOptions.EnableRetryOnFailure();
    })
);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 🔐 Identity-Konfiguration mit ApplicationUser + Rollen
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Cookie-Pfade
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/ExternalLogin";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// AuditService, HttpContextAccessor
builder.Services.AddScoped<AuditService>();
builder.Services.AddHttpContextAccessor();

// ➕ Discord OAuth2 mit korrekten Default-Schemes
builder.Services.AddAuthentication(options =>
{
    // Anwendungscookie für eingeloggte Nutzer
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    // externer Cookie (speichert vorübergehend das ExternalLogin-Info)
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    // Challenge – wenn nicht authentifiziert → Discord
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

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// 🌐 Middleware-Pipeline
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

// explizite Ausnahme für Public-Controller
app.MapControllerRoute(
    name: "public",
    pattern: "Public/{action=Index}/{id?}",
    defaults: new { controller = "Public", action = "Index" }
).WithMetadata(new AllowAnonymousAttribute());

// explizite Ausnahme für Account/ExternalLogin
app.MapControllerRoute(
    name: "login",
    pattern: "Account/{action=ExternalLogin}/{returnUrl?}",
    defaults: new { controller = "Account", action = "ExternalLogin" }
).WithMetadata(new AllowAnonymousAttribute());

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
)
.WithMetadata(new AllowAnonymousAttribute());

app.MapRazorPages();

app.Run();
