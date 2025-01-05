using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ZarinPal.Pay.DTOs;
using ZarinPal.Pay.Enums;
using ZarinPal.Pay.IService;
using ZarinPal.Pay.Requests;
using ZarinPal.Pay.Responses;
using ZarinPalPayment.Models;

namespace ZarinPalPayment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IZarinPalPaymentServices _zarinPalService;

        public HomeController(ILogger<HomeController> logger,IZarinPalPaymentServices zarinPal)
        {
            _zarinPalService = zarinPal;
            _logger = logger;
        }
        public async Task<IActionResult> PaymentOrder(int SumFactor)
        {
            HttpContext.Session.SetInt32("SumFactor", SumFactor);
            PaymentResponse result = await _zarinPalService.PaymentRequestAsync(
                new PaymentRequestDto(
               "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
                SumFactor,
                "توضیحات تراکنش",
                "https://localhost:7256/Home/VerifyPaymentOrder"
         
                ), PaymentType.Zarinpal);
            return Redirect($"https://www.zarinpal.com/pg/StartPay/{result.data.authority}");
        }

        public async Task<IActionResult> VerifyPaymentOrder(string authority, string status)
        {

            int SumFactor = HttpContext.Session.GetInt32("SumFactor") ?? 0;
            var verification = await _zarinPalService.VerifyPaymentAsync(new VerifyPaymentRequestDto(
                "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
                 SumFactor,
                 authority), ZarinPal.Pay.Enums.PaymentType.Zarinpal);
            if (status == "OK")
            {
                TempData["result"] = "sucsess";
                return RedirectToAction("Privacy", "Home");
            }
            else
            {
                TempData["result"] = "Error, Code:" + verification.code.ToString();
                return RedirectToAction("Index", "Home");
            }
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
