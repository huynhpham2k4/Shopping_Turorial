using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shopping_Tutorial.Areas.Admin.Repository;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;
using System.Security.Claims;

namespace Shopping_Tutorial.Controllers
{
	public class CheckoutController : Controller
	{
		public readonly Datacontext _dataContext;
		private readonly IEmailSender _emailSender;

		public CheckoutController(IEmailSender emailSender, Datacontext dataContext)
		{
			_emailSender = emailSender;
			_dataContext = dataContext;
		}

		public async Task<IActionResult> Checkout()
		{
			var userEmail = User.FindFirstValue(ClaimTypes.Email);
			if (userEmail == null)
			{
				return RedirectToAction("Login", "Account");
			}
			else
			{
				var orderItem = new OrderModel();
				var ordercode = Guid.NewGuid().ToString();
				orderItem.OrderCode = ordercode;


				//Nhan shipping cost tu cookie
				var shippingPriceCookie = Request.Cookies["ShippingPrice"];
				decimal shippingPrice = 0;

				//nhan coupon code tu cookie
				var coupon_code = Request.Cookies["CouponTitle"];

				if (shippingPriceCookie != null)
				{
					var shippingPriceJson = shippingPriceCookie;
					shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
				}
				orderItem.ShippingCost = shippingPrice;
				orderItem.CouponCode = coupon_code;
				orderItem.UserName = userEmail;
				orderItem.Status = 1;
				orderItem.CreatedDate = DateTime.Now;
				_dataContext.Add(orderItem);
				_dataContext.SaveChanges();
				List<CartItemModel> cartItem = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
				foreach (var cart in cartItem)
				{
					var orderDetails = new OrderDetail();
					orderDetails.UserName = userEmail;
					orderDetails.OrderCode = ordercode;
					orderDetails.ProductId = cart.ProductId;
					orderDetails.Price = cart.Price;
					orderDetails.Quantity = cart.Quantity;
					//update Product quantity
					var product = await _dataContext.Products.Where(pp => pp.Id == cart.ProductId).FirstOrDefaultAsync();
					product.Quantity -= cart.Quantity;
					product.Sold += cart.Quantity;
					_dataContext.SaveChanges();
					//update Product quantity//

					_dataContext.Add(orderDetails);
					_dataContext.SaveChanges();

				}
				HttpContext.Session.Remove("Cart");

				//send mail order when success
				var receiver = userEmail;
				var subject = "dat hang thanh cong.";
				var message = "dat hang thang cong, trai nghiem dich vu nhe.";
				await _emailSender.SendEmailAsync(receiver, subject, message);

				TempData["success"] = "Checkout thanh cong, vui long cho duyet don hang";
				return RedirectToAction("History", "Account");

			}
			return View();
		}




		//public IActionResult Index()
		//{
		//	return View();
		//}
		//public IActionResult Checkout()
		//{
		//	return View("~/Views/Checkout/Index.cshtml");
		//}
	}
}
