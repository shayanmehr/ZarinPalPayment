

public class HomeController : Controller {

private readonly IZarinPalPaymentServices _zarinPalService;

public HomeController(IZarinPalPaymentServices zarinPalService)
{
   _zarinPalService = zarinPalService;
}

public async Task<IActionResult> PaymentOrder(int SumFactor)
{
   HttpContext.Session.SetInt32("SumFactor", SumFactor);	
		PaymentResponse result = await _zarinPalService.PaymentRequestAsync(
			new PaymentRequestDto(
		   "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
			100000,
			"توضیحات تراکنش",
            "https://localhost:44310/Home/VerifyPaymentOrder",
			"09150000000",
			"info@shaya-soft.ir"
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
		if(status == "OK")
		{
            TempData["result"] = "خرید با موفقیت انجام شد";
            return RedirectToAction("Index", "Home"); 
        }
		else 
		{
            TempData["result"] = "Error, Code:" + verification.code.ToString(); 
            return RedirectToAction("Index", "Home");
        }
}

}
