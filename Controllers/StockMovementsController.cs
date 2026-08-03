using CeramiQ.Web.Data;
using CeramiQ.Web.Models;
using CeramiQ.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CeramiQ.Web.Controllers
{
    public class StockMovementsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StockMovementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            int? productId,
            string? movementType)
        {
            var movementQuery = _context.StockMovements
                .AsNoTracking()
                .AsQueryable();

            if (productId.HasValue)
            {
                movementQuery = movementQuery.Where(
                    movement => movement.ProductId == productId.Value);
            }

            if (movementType == "Giriş" ||
                movementType == "Çıkış")
            {
                movementQuery = movementQuery.Where(
                    movement => movement.MovementType == movementType);
            }

            var movements =
                await (
                    from movement in movementQuery
                    join product in _context.Products.AsNoTracking()
                        on movement.ProductId equals product.Id
                    orderby movement.CreatedAt descending
                    select new StockMovementListItemViewModel
                    {
                        Id = movement.Id,
                        ProductCode = product.ProductCode,
                        ProductName = product.Name,
                        MovementType = movement.MovementType,
                        Quantity = movement.Quantity,
                        Description = movement.Description,
                        CreatedAt = movement.CreatedAt
                    })
                    .Take(500)
                    .ToListAsync();

            StockMovementIndexViewModel model =
                new StockMovementIndexViewModel
                {
                    ProductId = productId,
                    MovementType = movementType ?? string.Empty,
                    Products = await GetProductOptionsAsync(),
                    Movements = movements
                };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            StockMovementCreateViewModel model =
                new StockMovementCreateViewModel
                {
                    Products = await GetProductOptionsAsync()
                };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            StockMovementCreateViewModel model)
        {
            if (model.MovementType != "Giriş" &&
                model.MovementType != "Çıkış")
            {
                ModelState.AddModelError(
                    nameof(model.MovementType),
                    "Geçerli bir hareket türü seçiniz.");
            }

            Product? product = await _context.Products
                .FirstOrDefaultAsync(
                    product => product.Id == model.ProductId);

            if (product == null)
            {
                ModelState.AddModelError(
                    nameof(model.ProductId),
                    "Seçilen ürün bulunamadı.");
            }

            if (product != null &&
                model.MovementType == "Çıkış" &&
                model.Quantity > product.StockQuantity)
            {
                ModelState.AddModelError(
                    nameof(model.Quantity),
                    $"Yetersiz stok. Mevcut stok: " +
                    $"{product.StockQuantity:N2} {product.Unit}");
            }

            if (!ModelState.IsValid)
            {
                model.Products = await GetProductOptionsAsync();
                return View(model);
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                StockMovement movement = new StockMovement
                {
                    ProductId = model.ProductId,
                    MovementType = model.MovementType,
                    Quantity = model.Quantity,
                    Description =
                        model.Description?.Trim() ?? string.Empty,
                    CreatedAt = DateTime.Now
                };

                _context.StockMovements.Add(movement);

                if (model.MovementType == "Giriş")
                {
                    product!.StockQuantity += model.Quantity;
                }
                else
                {
                    product!.StockQuantity -= model.Quantity;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] =
                    $"{product.Name} ürünü için " +
                    $"{model.Quantity:N2} {product.Unit} stok " +
                    $"{model.MovementType.ToLower()} işlemi kaydedildi.";

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                await transaction.RollbackAsync();

                ModelState.AddModelError(
                    string.Empty,
                    "Stok hareketi kaydedilirken bir hata oluştu.");

                model.Products = await GetProductOptionsAsync();

                return View(model);
            }
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
                    Text =
                        product.ProductCode + " - " +
                        product.Name + " | Stok: " +
                        product.StockQuantity + " " +
                        product.Unit
                })
                .ToListAsync();
        }
    }
}