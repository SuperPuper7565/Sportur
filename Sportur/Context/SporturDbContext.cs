using Microsoft.EntityFrameworkCore;
using Sportur.Models;

namespace Sportur.Context
{
    public class SporturDbContext : DbContext
    {
        public SporturDbContext(DbContextOptions<SporturDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }

        // Убираем Category, так как теперь это enum
        // public DbSet<Category> Categories { get; set; }

        public DbSet<BicycleModel> BicycleModels { get; set; }
        public DbSet<BicycleColor> BicycleColors { get; set; }
        public DbSet<BicycleSize> BicycleSizes { get; set; }
        public DbSet<BicycleVariant> BicycleVariants { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<WholesalePrice> WholesalePrices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =======================
            // BicycleModel → Colors
            // =======================
            modelBuilder.Entity<BicycleColor>()
                .HasOne(c => c.BicycleModel)
                .WithMany(m => m.Colors)
                .HasForeignKey(c => c.BicycleModelId)
                .OnDelete(DeleteBehavior.Cascade);

            // =======================
            // BicycleModel → Sizes
            // =======================
            modelBuilder.Entity<BicycleSize>()
                .HasOne(s => s.BicycleModel)
                .WithMany(m => m.Sizes)
                .HasForeignKey(s => s.BicycleModelId)
                .OnDelete(DeleteBehavior.Cascade);

            // =======================
            // BicycleVariant (SKU)
            // =======================
            modelBuilder.Entity<BicycleVariant>()
                .HasOne(v => v.BicycleModel)
                .WithMany(m => m.Variants)
                .HasForeignKey(v => v.BicycleModelId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BicycleVariant>()
                .HasOne(v => v.BicycleColor)
                .WithMany()
                .HasForeignKey(v => v.BicycleColorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BicycleVariant>()
                .HasOne(v => v.BicycleSize)
                .WithMany()
                .HasForeignKey(v => v.BicycleSizeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BicycleVariant>()
                .HasIndex(v => new
                {
                    v.BicycleModelId,
                    v.BicycleColorId,
                    v.BicycleSizeId
                })
                .IsUnique();

            // =======================
            // Orders
            // =======================
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.BicycleVariant)
                .WithMany()
                .HasForeignKey(oi => oi.BicycleVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            // =======================
            // Wholesale prices
            // =======================
            modelBuilder.Entity<WholesalePrice>()
                .HasOne(w => w.BicycleVariant)
                .WithMany()
                .HasForeignKey(w => w.BicycleVariantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
