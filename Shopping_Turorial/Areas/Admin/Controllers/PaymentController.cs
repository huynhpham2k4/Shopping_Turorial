using Microsoft.AspNetCore.Mvc;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Services.Momo;
using System.Reflection.Metadata.Ecma335;

namespace Shopping_Tutorial.Areas.Admin.Controllers
{
	public class PaymentController : Controller
	{
		private IMomoService _momoService;

		public PaymentController(IMomoService momoService)		{
			_momoService = momoService;
		}

		[HttpPost]
		public async Task<IActionResult> CreatePaymentMomo(OrderInfoModel model)
		{
			var response = await _momoService.CreatePaymentMomo(model);
			return Redirect(response.PayUrl ?? "/Cart");

		}

		[HttpGet]
		public IActionResult PaymentCallBack()
		{
			var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
			return View(response);
		}
	}
}
