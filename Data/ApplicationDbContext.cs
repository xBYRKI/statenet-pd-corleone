using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using statenet_lspd.Models;

public class ApplicationDbContext 
    : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<ErrorLog> ErrorLogs { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    // Sanktionen & Duties
    public DbSet<Sanktion> Sanktionen { get; set; }
    public DbSet<Duty> Duties { get; set; }

    // Neu: Ränge
    public DbSet<Rank> Ranks { get; set; }

    public DbSet<Paygrade> Paygrades { get; set; }

    public DbSet<ServiceInstruction> ServiceInstructions { get; set; }
    public DbSet<UserInstructionAcceptance> UserInstructionAcceptances { get; set; }

    // MenuItems und MenuItemRoles für Many-to-Many Beziehung
    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<MenuItemRole> MenuItemRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Index für Sanktion.Kategorie
        builder.Entity<Sanktion>()
            .HasIndex(s => s.Kategorie);

        // Enum-Konversion für Duty.Status
        builder.Entity<Duty>()
            .Property(d => d.Status)
            .HasConversion<string>()
            .HasColumnType("enum('IN_SERVICE','OUT_OF_SERVICE','PENDING','CANCELLED')");

        // Konfiguration für Rank-Entität
        builder.Entity<Rank>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Name)
                  .IsRequired()
                  .HasMaxLength(100);
            entity.Property(r => r.SortOrder)
                  .HasDefaultValue(0);
            entity.Property(r => r.DiscordRoleId)
                  .HasMaxLength(50);
            entity.Property(r => r.ColorHex)
                  .IsRequired()
                  .HasMaxLength(7)
                  .HasDefaultValue("#FFFFFF");
        });

        // Many-to-Many Beziehung zwischen MenuItem und Role über MenuItemRole
        builder.Entity<MenuItemRole>()
            .HasKey(mr => new { mr.MenuItemId, mr.RoleId });

        builder.Entity<MenuItemRole>()
            .HasOne(mr => mr.MenuItem)
            .WithMany(m => m.MenuItemRoles)
            .HasForeignKey(mr => mr.MenuItemId);

        builder.Entity<MenuItemRole>()
            .HasOne(mr => mr.Role)
            .WithMany()
            .HasForeignKey(mr => mr.RoleId);

        // Menüstruktur-Konfiguration für MenuItem
        builder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(m => m.Id); // Primärschlüssel für MenuItem
            entity.Property(m => m.Title)
                  .IsRequired()
                  .HasMaxLength(200);
            entity.Property(m => m.Order)
                  .HasDefaultValue(0);

            // Optional: Foreign Key für ParentId
            entity.HasOne(m => m.Parent)
                  .WithMany(p => p.Children)
                  .HasForeignKey(m => m.ParentId)
                  .OnDelete(DeleteBehavior.SetNull); // Setzt ParentId auf null, wenn das Eltern-Element gelöscht wird
        });
    }
}
