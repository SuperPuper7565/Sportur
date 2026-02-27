using Microsoft.EntityFrameworkCore;
using Sportur.Models;

namespace Sportur.Context
{
    public class SporturDbContext : DbContext
    {
        public SporturDbContext(DbContextOptions<SporturDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<BicycleModel> BicycleModels { get; set; }
        public DbSet<BicycleColor> BicycleColors { get; set; }
        public DbSet<BicycleVariant> BicycleVariants { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<WholesalePrice> WholesalePrices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Soft delete для моделей
            modelBuilder.Entity<BicycleModel>()
                .HasQueryFilter(m => !m.IsDeleted);

            // Color → Model
            modelBuilder.Entity<BicycleColor>()
                .HasOne(c => c.BicycleModel)
                .WithMany(m => m.Colors)
                .HasForeignKey(c => c.BicycleModelId)
                .OnDelete(DeleteBehavior.Cascade);

            // Variant → Color
            modelBuilder.Entity<BicycleVariant>()
                .HasOne(v => v.BicycleColor)
                .WithMany(m => m.Variants)
                .HasForeignKey(v => v.BicycleColorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Уникальность SKU
            modelBuilder.Entity<BicycleVariant>()
                .HasIndex(v => new
                {
                    v.BicycleColorId,
                    v.FrameSize
                })
                .IsUnique();

            // Order → Items
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // OrderItem → Variant
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.BicycleVariant)
                .WithMany(v => v.OrderItems)
                .HasForeignKey(oi => oi.BicycleVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            // WholesalePrice → Variant
            modelBuilder.Entity<WholesalePrice>()
                .HasOne(w => w.BicycleVariant)
                .WithMany()
                .HasForeignKey(w => w.BicycleVariantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WholesalePrice>()
                .HasIndex(w => new
                {
                    w.BicycleVariantId,
                    w.UserId
                })
                .IsUnique();
        }
    }
}