using CeramiQ.Web.Data;
using CeramiQ.Web.Models;
using CeramiQ.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CeramiQ.Web.Controllers
{
    public class ProductionOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductionOrdersController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await (
                from order in _context.ProductionOrders
                    .AsNoTracking()
                join product in _context.Products
                    .AsNoTracking()
                    on order.ProductId equals product.Id
                orderby order.CreatedAt descending
                select new ProductionOrderListItemViewModel
                {
                    Id = order.Id,
                    OrderNumber = order.OrderNumber,
                    ProductCode = product.ProductCode,
                    ProductName = product.Name,
                    ProductionLine = order.ProductionLine,
                    PlannedQuantity = order.PlannedQuantity,
                    ProducedQuantity = order.ProducedQuantity,
                    ScrapQuantity = order.ScrapQuantity,
                    DueDate = order.DueDate,
                    Status = order.Status
                })
                .ToListAsync();

            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new ProductionOrderCreateViewModel
            {
                StartDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(7),
                Products = await GetProductOptionsAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            ProductionOrderCreateViewModel model)
        {
            model.OrderNumber =
                (model.OrderNumber ?? string.Empty)
                .Trim()
                .ToUpperInvariant();

            if (model.ProductId > 0)
            {
                var productExists = await _context.Products
                    .AnyAsync(product =>
                        product.Id == model.ProductId);

                if (!productExists)
                {
                    ModelState.AddModelError(
                        nameof(model.ProductId),
                        "Seçilen ürün bulunamadı.");
                }
            }

            if (model.DueDate.Date < model.StartDate.Date)
            {
                ModelState.AddModelError(
                    nameof(model.DueDate),
                    "Teslim tarihi başlangıç tarihinden önce olamaz.");
            }

            if (!string.IsNullOrWhiteSpace(model.OrderNumber))
            {
                var orderNumberExists =
                    await _context.ProductionOrders.AnyAsync(
                        order =>
                            order.OrderNumber == model.OrderNumber);

                if (orderNumberExists)
                {
                    ModelState.AddModelError(
                        nameof(model.OrderNumber),
                        "Bu emir numarası daha önce kullanılmış.");
                }
            }

            if (!ModelState.IsValid)
            {
                model.Products = await GetProductOptionsAsync();

                return View(model);
            }

            var productionOrder = new ProductionOrder
            {
                OrderNumber = model.OrderNumber,
                ProductId = model.ProductId,
                ProductionLine = model.ProductionLine.Trim(),
                PlannedQuantity = model.PlannedQuantity,
                ProducedQuantity = 0,
                ScrapQuantity = 0,
                StartDate = model.StartDate.Date,
                DueDate = model.DueDate.Date,
                Status = "Planlandı",
                CreatedAt = DateTime.Now
            };

            _context.ProductionOrders.Add(productionOrder);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Üretim emri başarıyla oluşturuldu.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var order = await _context.ProductionOrders
                .AsNoTracking()
                .FirstOrDefaultAsync(order => order.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var productName =
                await GetProductDisplayNameAsync(order.ProductId);

            var model = new ProductionOrderUpdateViewModel
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                ProductName = productName,
                PlannedQuantity = order.PlannedQuantity,
                ProducedQuantity = order.ProducedQuantity,
                ScrapQuantity = order.ScrapQuantity,
                Status = order.Status
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(
            int id,
            ProductionOrderUpdateViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            var order = await _context.ProductionOrders
                .FirstOrDefaultAsync(order => order.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var allowedStatuses = new[]
            {
                "Planlandı",
                "Üretimde",
                "Tamamlandı"
            };

            if (!allowedStatuses.Contains(model.Status))
            {
                ModelState.AddModelError(
                    nameof(model.Status),
                    "Geçerli bir üretim durumu seçmelisiniz.");
            }

            var totalProcessed =
                model.ProducedQuantity + model.ScrapQuantity;

            if (model.ProducedQuantity >= 0 &&
                model.ScrapQuantity >= 0 &&
                totalProcessed > order.PlannedQuantity)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Üretilen ve fire miktarlarının toplamı " +
                    "planlanan miktarı aşamaz.");
            }

            if (!ModelState.IsValid)
            {
                model.OrderNumber = order.OrderNumber;
                model.PlannedQuantity = order.PlannedQuantity;
                model.ProductName =
                    await GetProductDisplayNameAsync(order.ProductId);

                return View(model);
            }

            order.ProducedQuantity = model.ProducedQuantity;
            order.ScrapQuantity = model.ScrapQuantity;
            order.Status = model.Status;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Üretim emri başarıyla güncellendi.";

            return RedirectToAction(nameof(Index));
        }

        private async Task<string> GetProductDisplayNameAsync(
            int productId)
        {
            var productName = await _context.Products
                .AsNoTracking()
                .Where(product => product.Id == productId)
                .Select(product =>
                    product.ProductCode + " - " + product.Name)
                .FirstOrDefaultAsync();

            return productName ?? "Ürün bulunamadı";
        }

        private async Task<List<SelectListItem>>
            GetProductOptionsAsync()
        {
            var products = await _context.Products
                .AsNoTracking()
                .OrderBy(product => product.Name)
                .Select(product => new
                {
                    product.Id,
                    product.ProductCode,
                    product.Name
                })
                .ToListAsync();

            return products
                .Select(product => new SelectListItem
                {
                    Value = product.Id.ToString(),
                    Text =
                        $"{product.ProductCode} - {product.Name}"
                })
                .ToList();
        }
    }
}