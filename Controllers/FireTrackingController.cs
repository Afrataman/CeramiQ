using CeramiQ.Web.Data;
using CeramiQ.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CeramiQ.Web.Controllers
{
    public class FireTrackingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FireTrackingController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            int? productId,
            string? productionLine)
        {
            var orderQuery = _context.ProductionOrders
                .AsNoTracking()
                .AsQueryable();

            if (productId.HasValue)
            {
                orderQuery = orderQuery.Where(
                    order => order.ProductId == productId.Value);
            }

            if (!string.IsNullOrWhiteSpace(productionLine))
            {
                orderQuery = orderQuery.Where(
                    order => order.ProductionLine == productionLine);
            }

            var orderData = await (
                from order in orderQuery
                join product in _context.Products.AsNoTracking()
                    on order.ProductId equals product.Id
                orderby order.CreatedAt descending
                select new
                {
                    order.Id,
                    order.OrderNumber,
                    product.ProductCode,
                    ProductName = product.Name,
                    order.ProductionLine,
                    order.ProducedQuantity,
                    order.ScrapQuantity,
                    order.Status,
                    order.DueDate
                })
                .ToListAsync();

            var orders = orderData.Select(order =>
            {
                decimal totalProcessed =
                    order.ProducedQuantity + order.ScrapQuantity;

                decimal scrapRate = totalProcessed > 0
                    ? Math.Round(
                        order.ScrapQuantity / totalProcessed * 100,
                        2)
                    : 0;

                return new FireTrackingListItemViewModel
                {
                    Id = order.Id,
                    OrderNumber = order.OrderNumber,
                    ProductCode = order.ProductCode,
                    ProductName = order.ProductName,
                    ProductionLine = order.ProductionLine,
                    ProducedQuantity = order.ProducedQuantity,
                    ScrapQuantity = order.ScrapQuantity,
                    TotalProcessedQuantity = totalProcessed,
                    ScrapRate = scrapRate,
                    Status = order.Status,
                    DueDate = order.DueDate
                };
            }).ToList();

            decimal totalProduced =
                orders.Sum(order => order.ProducedQuantity);

            decimal totalScrap =
                orders.Sum(order => order.ScrapQuantity);

            decimal totalProcessed = totalProduced + totalScrap;

            decimal overallScrapRate = totalProcessed > 0
                ? Math.Round(
                    totalScrap / totalProcessed * 100,
                    2)
                : 0;

            FireTrackingViewModel model =
                new FireTrackingViewModel
                {
                    ProductId = productId,
                    ProductionLine = productionLine ?? string.Empty,
                    TotalProducedQuantity = totalProduced,
                    TotalScrapQuantity = totalScrap,
                    OverallScrapRate = overallScrapRate,
                    HighRiskOrderCount = orders.Count(
                        order => order.ScrapRate >= 10),
                    Products = await GetProductOptionsAsync(),
                    ProductionLines =
                        await GetProductionLineOptionsAsync(),
                    Orders = orders
                };

            return View(model);
        }

        private async Task<List<SelectListItem>>
            GetProductOptionsAsync()
        {
            return await _context.Products
                .AsNoTracking()
                .OrderBy(product => product.Name)
                .Select(product => new SelectListItem
                {
                    Value = product.Id.ToString(),
                    Text = product.ProductCode + " - " + product.Name
                })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>>
            GetProductionLineOptionsAsync()
        {
            return await _context.ProductionOrders
                .AsNoTracking()
                .Where(order => order.ProductionLine != "")
                .Select(order => order.ProductionLine)
                .Distinct()
                .OrderBy(line => line)
                .Select(line => new SelectListItem
                {
                    Value = line,
                    Text = line
                })
                .ToListAsync();
        }
    }
}