using Microsoft.EntityFrameworkCore;
using ProjectAPI.Models;

namespace ProjectAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<ProjectScreenshot> ProjectScreenshots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure ApplicationUser
            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Configure Project
            modelBuilder.Entity<Project>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,8)");

            // Configure Purchase
            modelBuilder.Entity<Purchase>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,8)");

            // Configure relationships
            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.Project)
                .WithMany(p => p.Purchases)
                .HasForeignKey(p => p.ProjectId);

            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.User)
                .WithMany(u => u.Purchases)
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<ProjectScreenshot>()
                .HasOne(ps => ps.Project)
                .WithMany(p => p.Screenshots)
                .HasForeignKey(ps => ps.ProjectId);
        }
    }
}
