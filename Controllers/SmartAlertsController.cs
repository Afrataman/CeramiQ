using CeramiQ.Web.Data;
using CeramiQ.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CeramiQ.Web.Controllers
{
    public class SmartAlertsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SmartAlertsController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var criticalStocks = await _context.Products
                .AsNoTracking()
                .Where(product =>
                    product.StockQuantity <=
                    product.MinimumStockLevel)
                .OrderBy(product => product.StockQuantity)
                .Select(product =>
                    new CriticalStockAlertViewModel
                    {
                        ProductId = product.Id,
                        ProductCode = product.ProductCode,
                        ProductName = product.Name,
                        StockQuantity = product.StockQuantity,
                        MinimumStockLevel =
                            product.MinimumStockLevel,
                        Unit = product.Unit
                    })
                .ToListAsync();

            var productionData = await (
                from order in _context.ProductionOrders
                    .AsNoTracking()
                join product in _context.Products
                    .AsNoTracking()
                    on order.ProductId equals product.Id
                select new
                {
                    Order = order,
                    product.ProductCode,
                    ProductName = product.Name
                })
                .ToListAsync();

            var highScrapOrders = productionData
                .Where(item =>
                    item.Order.ProducedQuantity +
                    item.Order.ScrapQuantity > 0 &&
                    item.Order.ScrapQuantity * 100 >=
                    (item.Order.ProducedQuantity +
                     item.Order.ScrapQuantity) * 10)
                .Select(item =>
                {
                    decimal totalProcessed =
                        item.Order.ProducedQuantity +
                        item.Order.ScrapQuantity;

                    decimal scrapRate = Math.Round(
                        item.Order.ScrapQuantity /
                        totalProcessed * 100,
                        2);

                    return new SmartProductionAlertViewModel
                    {
                        OrderId = item.Order.Id,
                        OrderNumber = item.Order.OrderNumber,
                        ProductCode = item.ProductCode,
                        ProductName = item.ProductName,
                        ProductionLine =
                            item.Order.ProductionLine,
                        ProducedQuantity =
                            item.Order.ProducedQuantity,
                        ScrapQuantity =
                            item.Order.ScrapQuantity,
                        ScrapRate = scrapRate,
                        DueDate = item.Order.DueDate,
                        Status = item.Order.Status
                    };
                })
                .OrderByDescending(item => item.ScrapRate)
                .ToList();

            var today = DateTime.Today;

            var delayedOrders = productionData
                .Where(item =>
                    item.Order.DueDate.Date < today &&
                    item.Order.Status != "Tamamlandı")
                .Select(item =>
                    new SmartProductionAlertViewModel
                    {
                        OrderId = item.Order.Id,
                        OrderNumber = item.Order.OrderNumber,
                        ProductCode = item.ProductCode,
                        ProductName = item.ProductName,
                        ProductionLine =
                            item.Order.ProductionLine,
                        ProducedQuantity =
                            item.Order.ProducedQuantity,
                        ScrapQuantity =
                            item.Order.ScrapQuantity,
                        DueDate = item.Order.DueDate,
                        Status = item.Order.Status
                    })
                .OrderBy(item => item.DueDate)
                .ToList();

            var model = new SmartAlertsViewModel
            {
                CriticalStocks = criticalStocks,
                HighScrapOrders = highScrapOrders,
                DelayedOrders = delayedOrders
            };

            return View(model);
        }
    }
}