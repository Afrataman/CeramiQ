using System.Diagnostics;
using CeramiQ.Web.Data;
using CeramiQ.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CeramiQ.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .AsNoTracking()
                .ToListAsync();

            var productionOrders =
                await _context.ProductionOrders
                    .AsNoTracking()
                    .ToListAsync();

            var today = DateTime.Today;

            var model = new DashboardViewModel
            {
                ProductCount = products.Count,

                TotalStock = products.Sum(product =>
                    product.StockQuantity),

                CriticalStockCount = products.Count(product =>
                    product.StockQuantity <=
                    product.MinimumStockLevel),

                ActiveProductionOrderCount =
                    productionOrders.Count(order =>
                        order.Status == "Planlandý" ||
                        order.Status == "Üretimde"),

                HighScrapOrderCount =
    productionOrders.Count(order =>
        order.ProducedQuantity +
        order.ScrapQuantity > 0 &&

        order.ScrapQuantity * 100 >=
        (order.ProducedQuantity +
         order.ScrapQuantity) * 10),

                DelayedProductionOrderCount =
                    productionOrders.Count(order =>
                        order.DueDate.Date < today &&
                        order.Status != "Tamamlandý")
            };

            return View(model);
        }
        public async Task<IActionResult> ExplainAlert(string? type)
        {
            var model = new AlertExplanationViewModel();

            if (type == "stock")
            {
                var affectedCount = await _context.Products
                    .AsNoTracking()
                    .CountAsync(product =>
                        product.StockQuantity <=
                        product.MinimumStockLevel);

                model.AlertType = "stock";
                model.Title = "Kritik Stok Uyarýsý";
                model.AffectedRecordCount = affectedCount;

                model.Explanation =
                    "Bazý ürünlerin mevcut stok miktarý, " +
                    "belirlenen minimum stok seviyesine ulaþmýþtýr.";

                model.PossibleCause =
                    "Ürün tüketiminin artmasý, yeni stok giriþinin " +
                    "gecikmesi veya minimum stok seviyesinin yüksek " +
                    "belirlenmesi bu duruma neden olabilir.";

                model.RecommendedAction =
                    "Kritik seviyedeki ürünleri inceleyin ve ihtiyaç " +
                    "varsa yeni stok giriþi planlayýn.";
            }
            else if (type == "scrap")
            {
                var affectedCount = await _context.ProductionOrders
    .AsNoTracking()
    .CountAsync(order =>
        order.ProducedQuantity +
        order.ScrapQuantity > 0 &&

        order.ScrapQuantity * 100 >=
        (order.ProducedQuantity +
         order.ScrapQuantity) * 10);

                model.AlertType = "scrap";
                model.Title = "Yüksek Fire Uyarýsý";
                model.AffectedRecordCount = affectedCount;

                model.Explanation =
                    "Bazý üretim emirlerinde fire oraný yüzde 10 " +
                    "veya daha yüksek seviyeye ulaþmýþtýr.";

                model.PossibleCause =
                    "Hammadde kalitesi, makine ayarlarý veya üretim " +
                    "sürecindeki uygulama hatalarý fireyi artýrmýþ olabilir.";

                model.RecommendedAction =
                    "Ýlgili üretim emirlerini, kullanýlan hattý ve " +
                    "üretim koþullarýný kontrol edin.";
            }
            else if (type == "delay")
            {
                var today = DateTime.Today;

                var affectedCount = await _context.ProductionOrders
                    .AsNoTracking()
                    .CountAsync(order =>
                        order.DueDate.Date < today &&
                        order.Status != "Tamamlandý");

                model.AlertType = "delay";
                model.Title = "Geciken Üretim Uyarýsý";
                model.AffectedRecordCount = affectedCount;

                model.Explanation =
                    "Bazý üretim emirlerinin teslim tarihi geçmiþ " +
                    "olmasýna raðmen üretim süreci tamamlanmamýþtýr.";

                model.PossibleCause =
                    "Üretim hattýndaki yoðunluk, malzeme eksikliði " +
                    "veya planlama sorunlarý gecikmeye neden olabilir.";

                model.RecommendedAction =
                    "Geciken emirlerin durumunu kontrol edin ve " +
                    "üretim planýndaki önceliklerini deðerlendirin.";
            }
            else
            {
                return NotFound();
            }

            return View(model);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            });
        }
    }
}