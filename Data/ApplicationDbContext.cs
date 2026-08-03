using CeramiQ.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace CeramiQ.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductionOrder> ProductionOrders { get; set; }

        public DbSet<StockMovement> StockMovements { get; set; }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProductionOrder>()
                .HasIndex(order => order.OrderNumber)
                .IsUnique();

            modelBuilder.Entity<ProductionOrder>()
                .HasOne<Product>()
                .WithMany()
                .HasForeignKey(order => order.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StockMovement>()
                .HasOne<Product>()
                .WithMany()
                .HasForeignKey(movement => movement.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StockMovement>()
                .HasIndex(movement => movement.ProductId);

            modelBuilder.Entity<StockMovement>()
                .HasIndex(movement => movement.CreatedAt);
        }
    }
}