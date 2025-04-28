// Program.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using statenet_lspd.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using statenet_lspd.Data;

var builder = WebApplication.CreateBuilder(args);

// Serilog-Konfiguration
Log.Logger = new LoggerConfiguration()
    // hier kannst du Sinks hinzufügen (Datei, Seq, etc.)
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

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Authorization Policies based on Permissions
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("HR.View", policy =>
        policy.RequireClaim("Permission", Permission.HR_View.ToString()));
    options.AddPolicy("HR.Create", policy =>
        policy.RequireClaim("Permission", Permission.HR_Create.ToString()));
    options.AddPolicy("HR.Delete", policy =>
        policy.RequireClaim("Permission", Permission.HR_Delete.ToString()));
    options.AddPolicy("HR.Sanction", policy =>
        policy.RequireClaim("Permission", Permission.HR_Sanction.ToString()));
    options.AddPolicy("HR.Promotion", policy =>
        policy.RequireClaim("Permission", Permission.HR_Promotion.ToString()));
    options.AddPolicy("HR.Demotion", policy =>
        policy.RequireClaim("Permission", Permission.HR_Demotion.ToString()));
    options.AddPolicy("HR.Suspension", policy =>
        policy.RequireClaim("Permission", Permission.HR_Suspension.ToString()));
    // weitere Policies hinzufügen
});

var app = builder.Build();

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

// Public controllers allow anonymous
app.MapControllerRoute(
    name: "public",
    pattern: "Public/{action=Index}/{id?}",
    defaults: new { controller = "Public", action = "Index" }
).WithMetadata(new AllowAnonymousAttribute());

// Account external login
app.MapControllerRoute(
    name: "login",
    pattern: "Account/{action=ExternalLogin}/{returnUrl?}",
    defaults: new { controller = "Account", action = "ExternalLogin" }
).WithMetadata(new AllowAnonymousAttribute());

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
).WithMetadata(new AllowAnonymousAttribute());

app.MapRazorPages();

app.Run();