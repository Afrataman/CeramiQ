using CeramiQ.Web.Services;
using CeramiQ.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace CeramiQ.Web.Controllers
{
    public class NaturalLanguageQueryController : Controller
    {
        private readonly SafeSqlQueryService _safeSqlQueryService;

        public NaturalLanguageQueryController(
            SafeSqlQueryService safeSqlQueryService)
        {
            _safeSqlQueryService = safeSqlQueryService;
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
                .ToLowerInvariant();

            string generatedSql;

            Match numberMatch = Regex.Match(question, @"\d+");

            if ((question.Contains("stok") ||
                 question.Contains("stoğu")) &&
                question.Contains("az") &&
                numberMatch.Success &&
                int.TryParse(numberMatch.Value, out int stockLimit))
            {
                generatedSql =
                    $"SELECT TOP (100) * FROM Products " +
                    $"WHERE StockQuantity < {stockLimit}";

                model.Explanation =
                    $"Bu sorgu stok miktarı {stockLimit}'den az olan ürünleri listeler.";
            }
            else if (question.Contains("fire") &&
          (question.Contains("yüksek") ||
           question.Contains("fazla")))
            {
                int fireLimit = 10;

                if (numberMatch.Success &&
                    int.TryParse(numberMatch.Value, out int enteredFireLimit))
                {
                    fireLimit = enteredFireLimit;
                }

                generatedSql =
                    "SELECT TOP (100) *, " +
                    "(ScrapQuantity * 100.0 / " +
                    "NULLIF(ProducedQuantity + ScrapQuantity, 0)) AS FireOrani " +
                    "FROM ProductionOrders " +
                    "WHERE (ScrapQuantity * 100.0 / " +
                    "NULLIF(ProducedQuantity + ScrapQuantity, 0)) >= " +
                    fireLimit;

                model.Explanation =
                    $"Bu sorgu fire oranı yüzde {fireLimit} veya daha yüksek " +
                    "olan üretim emirlerini listeler.";
            }
            
            else if (question.Contains("gecik") &&
                     question.Contains("üretim") &&
                     (question.Contains("emir") ||
                      question.Contains("sipariş")))
            {
                generatedSql =
                    "SELECT TOP (100) * FROM ProductionOrders " +
                    "WHERE DueDate < CAST(GETDATE() AS date) " +
                    "AND Status <> N'Tamamlandı'";

                model.Explanation =
                    "Bu sorgu teslim tarihi geçmiş ve henüz tamamlanmamış üretim emirlerini listeler.";
            }
            else if (question.Contains("üretim") &&
                     (question.Contains("emir") ||
                      question.Contains("sipariş")))
            {
                generatedSql =
                    "SELECT TOP (100) * FROM ProductionOrders";

                model.Explanation =
                    "Bu sorgu bütün üretim emirlerini listeler.";
            }
            else if (question.Contains("ürün"))
            {
                generatedSql =
                    "SELECT TOP (100) * FROM Products";

                model.Explanation =
                    "Bu sorgu bütün ürünleri listeler.";
            }
            else
            {
                model.ErrorMessage =
                    "Bu soru henüz desteklenmiyor. Ürünler veya üretim emirleri hakkında soru yazınız.";

                return View(model);
            }

            bool isSafe = _safeSqlQueryService.IsSafeQuery(
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
                    await _safeSqlQueryService.ExecuteSelectQueryAsync(
                        generatedSql);

                model.ResultColumns = result.Columns;
                model.ResultRows = result.Rows;
            }
            catch
            {
                model.ErrorMessage =
                    "Sorgu çalıştırılırken bir hata oluştu.";
            }

            return View(model);
        }
    }
}