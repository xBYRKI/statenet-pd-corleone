using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using statenet_lspd.Models;

namespace statenet_lspd.Data
{
    public class ApplicationDbContext 
        : IdentityDbContext<ApplicationUser, ApplicationRole, string>
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Sanktion index
            builder.Entity<Sanktion>().HasIndex(s => s.Kategorie);

            // Duty enum conversion
            builder.Entity<Duty>()
                .Property(d => d.Status)
                .HasConversion<string>()
                .HasColumnType("enum('IN_SERVICE','OUT_OF_SERVICE','PENDING','CANCELLED')");

            // HRAction mapping
            builder.Entity<HRAction>()
                .HasOne(a => a.User)
                .WithMany(u => u.HRActions)
                .HasForeignKey(a => a.UserId);

            // Rank configuration
            builder.Entity<Rank>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Name).IsRequired().HasMaxLength(100);
                entity.Property(r => r.SortOrder).HasDefaultValue(0);
                entity.Property(r => r.DiscordRoleId).HasMaxLength(50);
                entity.Property(r => r.ColorHex).IsRequired().HasMaxLength(7).HasDefaultValue("#FFFFFF");
            });

            // MenuItem <-> Role many-to-many
            builder.Entity<MenuItemRole>()
                .HasKey(mr => new { mr.MenuItemId, mr.RoleId });
            builder.Entity<MenuItemRole>()
                .HasOne(mr => mr.MenuItem)
                .WithMany(m => m.MenuItemRoles)
                .HasForeignKey(mr => mr.MenuItemId);
            builder.Entity<MenuItemRole>()
                .HasOne(mr => mr.Role)
                .WithMany(r => r.MenuItemRoles)
                .HasForeignKey(mr => mr.RoleId);

            // MenuItem configuration
            builder.Entity<MenuItem>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Title).IsRequired().HasMaxLength(200);
                entity.Property(m => m.Order).HasDefaultValue(0);
                entity.HasOne(m => m.Parent)
                      .WithMany(p => p.Children)
                      .HasForeignKey(m => m.ParentId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // RolePermission composite key
            builder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.Permission });
            builder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);
        }
    }
}