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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
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
        }
    }
}