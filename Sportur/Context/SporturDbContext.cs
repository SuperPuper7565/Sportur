using Microsoft.EntityFrameworkCore;
using Sportur.Models;

namespace Sportur.Context
{
    public class SporturDbContext : DbContext
    {
        public SporturDbContext(DbContextOptions<SporturDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<BicycleModel> BicycleModels { get; set; }
        public DbSet<BicycleColor> BicycleColors { get; set; }
        public DbSet<BicycleSize> BicycleSizes { get; set; }

        public DbSet<WholesalePrice> WholesalePrices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Category self-reference
            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.Children)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // BicycleModel → Colors
            modelBuilder.Entity<BicycleColor>()
                .HasOne(c => c.BicycleModel)
                .WithMany(m => m.Colors)
                .HasForeignKey(c => c.BicycleModelId);

            // BicycleModel → Sizes
            modelBuilder.Entity<BicycleSize>()
                .HasOne(s => s.BicycleModel)
                .WithMany(m => m.Sizes)
                .HasForeignKey(s => s.BicycleModelId);

            // WholesalePrice uniqueness
            modelBuilder.Entity<WholesalePrice>()
                .HasIndex(w => new { w.BicycleSizeId, w.UserId })
                .IsUnique();
        }
    }
}
