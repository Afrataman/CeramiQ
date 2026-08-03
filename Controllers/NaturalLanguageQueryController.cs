using CeramiQ.Web.Services;
using CeramiQ.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CeramiQ.Web.Controllers
{
    public class NaturalLanguageQueryController : Controller
    {
        private readonly SafeSqlQueryService
            _safeSqlQueryService;

        private readonly ILogger
            <NaturalLanguageQueryController> _logger;

        private static readonly CultureInfo
            TurkishCulture =
                new CultureInfo("tr-TR");

        public NaturalLanguageQueryController(
            SafeSqlQueryService safeSqlQueryService,
            ILogger<NaturalLanguageQueryController> logger)
        {
            _safeSqlQueryService = safeSqlQueryService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            NaturalLanguageQueryViewModel model =
                new NaturalLanguageQueryViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(
            NaturalLanguageQueryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string question = model.Question
                .Trim()
                .ToLower(TurkishCulture);

            bool mentionsStock =
                question.Contains("stok") ||
                question.Contains("stoğu");

            bool mentionsProduct =
                question.Contains("ürün");

            bool mentionsProduction =
                question.Contains("üretim");

            bool mentionsOrder =
                question.Contains("emir") ||
                question.Contains("sipariş");

            Match numberMatch =
                Regex.Match(question, @"\d+");

            string generatedSql;

            if (mentionsStock &&
                mentionsProduct &&
                (question.Contains("yok") ||
                 question.Contains("olmayan") ||
                 question.Contains("tüken") ||
                 question.Contains("sıfır")))
            {
                generatedSql =
                    "SELECT TOP (100) * FROM Products " +
                    "WHERE StockQuantity <= 0 " +
                    "ORDER BY Name";

                model.Explanation =
                    "Bu sorgu stok miktarı sıfır olan ürünleri listeler.";
            }
            else if (mentionsStock &&
                     question.Contains("kritik"))
            {
                generatedSql =
                    "SELECT TOP (100) * FROM Products " +
                    "WHERE StockQuantity <= MinimumStockLevel " +
                    "ORDER BY StockQuantity";

                model.Explanation =
                    "Bu sorgu stok miktarı minimum stok seviyesine eşit veya daha düşük olan ürünleri listeler.";
            }
            else if (mentionsStock &&
                     question.Contains("az") &&
                     numberMatch.Success &&
                     int.TryParse(
                         numberMatch.Value,
                         out int stockLimit))
            {
                if (stockLimit < 0 ||
                    stockLimit > 1000000)
                {
                    model.ErrorMessage =
                        "Stok sınırı 0 ile 1.000.000 arasında olmalıdır.";

                    return View(model);
                }

                generatedSql =
                    "SELECT TOP (100) * FROM Products " +
                    $"WHERE StockQuantity < {stockLimit} " +
                    "ORDER BY StockQuantity";

                model.Explanation =
                    $"Bu sorgu stok miktarı {stockLimit}'den az olan ürünleri listeler.";
            }
            else if (question.Contains("fire") &&
                     (question.Contains("oran") ||
                      question.Contains("yüksek") ||
                      question.Contains("fazla")))
            {
                int fireLimit = 10;

                if (numberMatch.Success &&
                    int.TryParse(
                        numberMatch.Value,
                        out int enteredFireLimit))
                {
                    fireLimit = enteredFireLimit;
                }

                if (fireLimit < 0 ||
                    fireLimit > 100)
                {
                    model.ErrorMessage =
                        "Fire oranı 0 ile 100 arasında olmalıdır.";

                    return View(model);
                }

                generatedSql =
                    "SELECT TOP (100) *, " +
                    "CAST(ROUND(" +
                    "ScrapQuantity * 100.0 / " +
                    "NULLIF(ProducedQuantity + ScrapQuantity, 0), 2) " +
                    "AS DECIMAL(10,2)) AS FireOrani " +
                    "FROM ProductionOrders " +
                    "WHERE (ScrapQuantity * 100.0 / " +
                    "NULLIF(ProducedQuantity + ScrapQuantity, 0)) " +
                    $">= {fireLimit} " +
                    "ORDER BY FireOrani DESC";

                model.Explanation =
                    $"Bu sorgu fire oranı yüzde {fireLimit} veya daha yüksek olan üretim emirlerini listeler.";
            }
            else if (question.Contains("gecik") &&
                     (mentionsProduction ||
                      mentionsOrder))
            {
                generatedSql =
                    "SELECT TOP (100) * " +
                    "FROM ProductionOrders " +
                    "WHERE DueDate < CAST(GETDATE() AS date) " +
                    "AND Status <> N'Tamamlandı' " +
                    "ORDER BY DueDate";

                model.Explanation =
                    "Bu sorgu teslim tarihi geçmiş ve henüz tamamlanmamış üretim emirlerini listeler.";
            }
            else if (mentionsProduction &&
                     mentionsOrder &&
                     question.Contains("planlan"))
            {
                generatedSql =
                    "SELECT TOP (100) * " +
                    "FROM ProductionOrders " +
                    "WHERE Status = N'Planlandı' " +
                    "ORDER BY DueDate";

                model.Explanation =
                    "Bu sorgu planlandı durumundaki üretim emirlerini listeler.";
            }
            else if (mentionsProduction &&
                     mentionsOrder &&
                     (question.Contains("üretimde") ||
                      question.Contains("devam eden")))
            {
                generatedSql =
                    "SELECT TOP (100) * " +
                    "FROM ProductionOrders " +
                    "WHERE Status = N'Üretimde' " +
                    "ORDER BY DueDate";

                model.Explanation =
                    "Bu sorgu üretimi devam eden üretim emirlerini listeler.";
            }
            else if (mentionsProduction &&
                     mentionsOrder &&
                     question.Contains("tamamlan"))
            {
                generatedSql =
                    "SELECT TOP (100) * " +
                    "FROM ProductionOrders " +
                    "WHERE Status = N'Tamamlandı' " +
                    "ORDER BY DueDate DESC";

                model.Explanation =
                    "Bu sorgu tamamlanan üretim emirlerini listeler.";
            }
            else if (mentionsProduction &&
                     mentionsOrder &&
                     (question.Contains("kaç") ||
                      question.Contains("sayısı")))
            {
                generatedSql =
                    "SELECT COUNT(*) AS UretimEmriSayisi " +
                    "FROM ProductionOrders";

                model.Explanation =
                    "Bu sorgu sistemdeki üretim emri sayısını gösterir.";
            }
            else if (mentionsProduct &&
                     (question.Contains("kaç") ||
                      question.Contains("sayısı")))
            {
                generatedSql =
                    "SELECT COUNT(*) AS UrunSayisi " +
                    "FROM Products";

                model.Explanation =
                    "Bu sorgu sistemde kayıtlı ürün sayısını gösterir.";
            }
            else if (mentionsStock &&
                     (question.Contains("toplam") ||
                      question.Contains("miktarı")))
            {
                generatedSql =
                    "SELECT COALESCE(SUM(StockQuantity), 0) " +
                    "AS ToplamStok FROM Products";

                model.Explanation =
                    "Bu sorgu bütün ürünlerin toplam stok miktarını gösterir.";
            }
            else if (mentionsProduction &&
                     mentionsOrder)
            {
                generatedSql =
                    "SELECT TOP (100) * " +
                    "FROM ProductionOrders " +
                    "ORDER BY CreatedAt DESC";

                model.Explanation =
                    "Bu sorgu bütün üretim emirlerini listeler.";
            }
            else if (mentionsProduct)
            {
                generatedSql =
                    "SELECT TOP (100) * FROM Products " +
                    "ORDER BY Name";

                model.Explanation =
                    "Bu sorgu bütün ürünleri listeler.";
            }
            else
            {
                model.ErrorMessage =
                    "Bu soru henüz desteklenmiyor. Örnek sorulardan birini seçebilirsiniz.";

                return View(model);
            }

            bool isSafe =
                _safeSqlQueryService.IsSafeQuery(
                    generatedSql,
                    out string errorMessage);

            if (!isSafe)
            {
                model.ErrorMessage = errorMessage;
                return View(model);
            }

            model.GeneratedSql = generatedSql;

            try
            {
                var result =
                    await _safeSqlQueryService
                        .ExecuteSelectQueryAsync(
                            generatedSql,
                            HttpContext.RequestAborted);

                model.ResultColumns = result.Columns;
                model.ResultRows = result.Rows;
            }
            catch (OperationCanceledException)
            {
                model.ErrorMessage =
                    "Sorgu işlemi iptal edildi.";
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Doğal dil sorgusu çalıştırılırken hata oluştu.");

                model.ErrorMessage =
                    "Sorgu çalıştırılırken bir hata oluştu.";
            }

            return View(model);
        }
    }
}