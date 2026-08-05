using CeramiQ.Web.Data;
using CeramiQ.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace CeramiQ.Web.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Veritabanı oluşturulmuş mu kontrol et
            context.Database.EnsureCreated();

            // Eğer veritabanında ürün varsa ekleme yapma
            if (context.Products.Any())
            {
                return;
            }

            // 1. Örnek Ürünler
            var products = new Product[]
            {
                new Product { ProductCode = "CRQ-001", Name = "Beyaz Duvar Seramiği", Category = "Duvar Karosu", StockQuantity = 120, MinimumStockLevel = 40, Unit = "Kutu", CreatedAt = DateTime.Now.AddDays(-10) },
                new Product { ProductCode = "CRQ-002", Name = "Antrasit Zemin Seramiği", Category = "Zemin Karosu", StockQuantity = 5, MinimumStockLevel = 50, Unit = "Kutu", CreatedAt = DateTime.Now.AddDays(-8) }
            };

            context.Products.AddRange(products);
            context.SaveChanges();

            // 2. Örnek Üretim Emirleri
            var productionOrders = new ProductionOrder[]
            {
                new ProductionOrder { OrderNumber = "URE-2026-001", ProductId = products[0].Id, ProductionLine = "Hat 1", PlannedQuantity = 1500, ProducedQuantity = 1000, ScrapQuantity = 500, StartDate = DateTime.Today.AddDays(-10), DueDate = DateTime.Today.AddDays(-5), Status = "Gecikti", CreatedAt = DateTime.Now.AddDays(-10) },
                new ProductionOrder { OrderNumber = "URE-2026-002", ProductId = products[1].Id, ProductionLine = "Hat 2", PlannedQuantity = 600, ProducedQuantity = 400, ScrapQuantity = 20, StartDate = DateTime.Today.AddDays(-4), DueDate = DateTime.Today.AddDays(2), Status = "Devam Ediyor", CreatedAt = DateTime.Now.AddDays(-4) },
                new ProductionOrder { OrderNumber = "URE-2026-003", ProductId = products[0].Id, ProductionLine = "Hat 3", PlannedQuantity = 300, ProducedQuantity = 250, ScrapQuantity = 10, StartDate = DateTime.Today.AddDays(-1), DueDate = DateTime.Today.AddDays(5), Status = "Planlandı", CreatedAt = DateTime.Now.AddDays(-1) }
            };

            context.ProductionOrders.AddRange(productionOrders);
            context.SaveChanges();

            // 3. Örnek Stok Hareketleri
            var stockMovements = new StockMovement[]
            {
                new StockMovement { ProductId = products[0].Id, MovementType = "Giriş", Quantity = 50, Description = "Üretimden depoya giriş", CreatedAt = DateTime.Now.AddDays(-3) },
                new StockMovement { ProductId = products[1].Id, MovementType = "Çıkış", Quantity = 35, Description = "Müşteri siparişi", CreatedAt = DateTime.Now.AddDays(-2) },
                new StockMovement { ProductId = products[0].Id, MovementType = "Giriş", Quantity = 30, Description = "Stok düzeltme işlemi", CreatedAt = DateTime.Now.AddDays(-1) }
            };

            context.StockMovements.AddRange(stockMovements);
            context.SaveChanges();
        }
    }
}