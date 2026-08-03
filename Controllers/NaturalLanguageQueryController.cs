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
     $"SELECT TOP (100) * FROM Products WHERE StockQuantity < {stockLimit}";
                model.Explanation =
                    $"Bu sorgu stok miktarı {stockLimit}'den az olan ürünleri listeler.";
            }
            else if (question.Contains("üretim") &&
                     question.Contains("sipariş"))
            {
                generatedSql =
     "SELECT TOP (100) * FROM ProductionOrders";

                model.Explanation =
                    "Bu sorgu bütün üretim siparişlerini listeler.";
            }
            else if (question.Contains("ürün"))
            {
                generatedSql =
    "SELECT TOP (100) * FROM ProductionOrders";
                model.Explanation =
                    "Bu sorgu bütün ürünleri listeler.";
            }
            else
            {
                model.ErrorMessage =
                    "Bu soru henüz desteklenmiyor. Ürünler veya üretim siparişleri hakkında soru yazınız.";

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