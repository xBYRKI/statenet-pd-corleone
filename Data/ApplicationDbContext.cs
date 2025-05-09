using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using statenet_lspd.Models;

namespace statenet_lspd.Data
{
    public class ApplicationDbContext 
        : IdentityDbContext<ApplicationUser, ApplicationRole, string,
                             IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>,
                             IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        public DbSet<Sanktion> Sanktionen { get; set; }
        public DbSet<Duty> Duties { get; set; }
        public DbSet<Rank> Ranks { get; set; }
        public DbSet<Paygrade> Paygrades { get; set; }
        public DbSet<ServiceInstruction> ServiceInstructions { get; set; }
        public DbSet<UserInstructionAcceptance> UserInstructionAcceptances { get; set; }
        public DbSet<HRAction> HRActions { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<MenuItemRole> MenuItemRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        public DbSet<Unit> Units { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //
            // Identity-Tabellen (optional, falls du andere Namen möchtest)
            //

            //
            // ErrorLog & AuditLog
            //
            builder.Entity<ErrorLog>().ToTable("ErrorLogs");
            builder.Entity<AuditLog>().ToTable("AuditLogs");

            //
            // Sanktion
            //
            builder.Entity<Sanktion>()
                   .ToTable("Sanktionen")
                   .HasIndex(s => s.Kategorie);

            //
            // Duty (Enum-Conversion)
            //
            builder.Entity<Duty>()
                   .ToTable("Duties")
                   .Property(d => d.Status)
                   .HasConversion<string>()
                   .HasColumnType("enum('IN_SERVICE','OUT_OF_SERVICE','PENDING','CANCELLED')");

            //
            // Rank
            //
            builder.Entity<Rank>(entity =>
            {
                entity.ToTable("Ranks");
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

            //
            // Paygrade, ServiceInstruction, UserInstructionAcceptance
            // (Default-Mapping, bei Bedarf ergänzen)
            //
            builder.Entity<Paygrade>().ToTable("Paygrades");
            builder.Entity<ServiceInstruction>().ToTable("ServiceInstructions");
            builder.Entity<UserInstructionAcceptance>().ToTable("UserInstructionAcceptances");

            //
            // HRAction
            //
            builder.Entity<HRAction>(entity =>
            {
                entity.ToTable("HRActions");
                entity.HasKey(a => a.Id);
                entity.Property(a => a.ActionType)
                      .HasConversion<string>()
                      .HasColumnType("enum('Hire','Termination','Sanction','Promotion','Demotion','Suspension')");
                entity.HasOne(a => a.User)
                      .WithMany(u => u.HRActions)
                      .HasForeignKey(a => a.UserId);
            });

            //
            // Unit
            //
            builder.Entity<Unit>(entity =>
            {
                entity.ToTable("Units");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Name)
                      .IsRequired()
                      .HasMaxLength(100);
                entity.Property(u => u.Description)
                      .HasMaxLength(500);

                // Many-to-Many User ↔ Unit
                entity.HasMany(u => u.Users)
                      .WithMany(u => u.Units)
                      .UsingEntity<Dictionary<string, object>>(
                          "UserUnit",
                          r => r.HasOne<ApplicationUser>()
                                .WithMany()
                                .HasForeignKey("UserId"),
                          l => l.HasOne<Unit>()
                                .WithMany()
                                .HasForeignKey("UnitId"),
                          je =>
                          {
                              je.ToTable("UserUnits");
                              je.HasKey("UserId", "UnitId");
                          });
            });

            //
            // MenuItem & MenuItemRole
            //
            builder.Entity<MenuItem>(entity =>
            {
                entity.ToTable("MenuItems");
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Title)
                      .IsRequired()
                      .HasMaxLength(200);
                entity.Property(m => m.Order)
                      .HasDefaultValue(0);
                entity.HasOne(m => m.Parent)
                      .WithMany(p => p.Children)
                      .HasForeignKey(m => m.ParentId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<MenuItemRole>(entity =>
            {
                entity.ToTable("MenuItemRoles");
                entity.HasKey(mr => new { mr.MenuItemId, mr.RoleId });
                entity.HasOne(mr => mr.MenuItem)
                      .WithMany(m => m.MenuItemRoles)
                      .HasForeignKey(mr => mr.MenuItemId);
                entity.HasOne(mr => mr.Role)
                      .WithMany(r => r.MenuItemRoles)
                      .HasForeignKey(mr => mr.RoleId);
            });

            //
            // RolePermission
            //
            builder.Entity<RolePermission>(entity =>
            {
                entity.ToTable("RolePermissions");
                entity.HasKey(rp => new { rp.RoleId, rp.Permission });
                entity.Property(rp => rp.Permission)
                      .HasConversion<string>()
                      .HasMaxLength(50);
                entity.HasOne(rp => rp.Role)
                      .WithMany(r => r.RolePermissions)
                      .HasForeignKey(rp => rp.RoleId);
            });
        }
    }
}
