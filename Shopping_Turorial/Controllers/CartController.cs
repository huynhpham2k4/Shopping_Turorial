using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Models.ViewModels;
using Shopping_Tutorial.Repository;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.InteropServices;

namespace Shopping_Tutorial.Controllers
{
	public class CartController : Controller
	{
		private readonly Datacontext _dataContext;
		public CartController(Datacontext dataContext)
		{
			_dataContext = dataContext;
		}
		public IActionResult Index()
		{
			List<CartItemModel> cartItem = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
			//Nhan shipping gia tu cookie
			var shippingPriceCookie = Request.Cookies["ShippingPrice"];
			decimal shippingPrice = 0;

			//Nhan Coupon Code tu cookie
			var coupon_code = Request.Cookies["CouponTitle"];

			if (shippingPriceCookie != null)
			{
				var shippingPriceJson = shippingPriceCookie;
				shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
			}

			CartItemViewModel cartVM = new CartItemViewModel
			{
				CartItems = cartItem,
				GrandTotal = cartItem.Sum(x => x.Quantity * x.Price),
				ShippingCost = shippingPrice,
				CouponCode = coupon_code,
			};

			return View(cartVM);
		}

		public async Task<IActionResult> Add(int Id)
		{
			ProductModel product = await _dataContext.Products.FindAsync(Id);
			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
			CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();

			if (cartItem == null)
			{
				cart.Add(new CartItemModel(product));
			}
			else
			{
				cartItem.Quantity += 1;
			}

			HttpContext.Session.SetJson("Cart", cart);// luu tru du lieu cart vao session cart

			TempData["success"] = "Add item to cart successfully";
			return Redirect(Request.Headers["Referer"].ToString());
		}

		public async Task<IActionResult> Decrease(int Id)
		{
			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");

			CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();

			if (cartItem.Quantity > 1)
			{
				--cartItem.Quantity;
			}
			else
			{
				cart.RemoveAll(p => p.ProductId == Id);
			}

			if (cart.Count == 0)
			{
				HttpContext.Session.Remove("Cart");

			}
			else
			{
				HttpContext.Session.SetJson("Cart", cart);
			}

			TempData["success"] = "Decrease item quantity to cart successfully";
			return RedirectToAction("Index");
		}

		public async Task<IActionResult> Increase(int Id)
		{
			var product = await _dataContext.Products.Where(p => p.Id == Id).FirstOrDefaultAsync();
			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");

			CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();

			if (cartItem.Quantity > 0 && product.Quantity >= cartItem.Quantity)
			{
				++cartItem.Quantity;
				TempData["success"] = "Increase quantity to cart successfully";
			}
			else
			{
				TempData["success"] = "Maximum Product Quantity to cart sucessfully";
				cartItem.Quantity = product.Quantity;
			}

			if (cart.Count == 0)
			{

				HttpContext.Session.Remove("Cart");

			}
			else
			{
				HttpContext.Session.SetJson("Cart", cart);
			}

			TempData["success"] = "Increase item quantity to cart successfully";
			return RedirectToAction("Index");
		}

		public async Task<IActionResult> Remove(int Id)
		{
			List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
			cart.RemoveAll(p => p.ProductId == Id);
			if (cart.Count == 0)
			{
				HttpContext.Session.Remove("Cart");
			}
			else
			{
				HttpContext.Session.SetJson("Cart", cart);
			}

			TempData["success"] = "Remove item of cart successfully";
			return RedirectToAction("Index");
		}

		public async Task<IActionResult> Clear()
		{
			HttpContext.Session.Remove("Cart");

			TempData["success"] = "Clear all item of cart successfully";
			return RedirectToAction("Index");
		}

		[HttpPost]
		[Route("Cart/GetShipping")]
		public async Task<IActionResult> GetShipping(string tinh, string quan, string phuong)
		{
			var existingShipping = await _dataContext.Shippings
			   .FirstOrDefaultAsync(x => x.City == tinh && x.District == quan && x.Ward == phuong);

			decimal shippingPrice = 0;// Set mac dinh gia tien

			if (existingShipping != null)
			{
				shippingPrice = existingShipping.Price;
			}
			else
			{
				//Set mac dinh gia tien neu khong tin thay
				shippingPrice = 50000;
			}
			var shippingPriceJson = JsonConvert.SerializeObject(shippingPrice);
			try
			{
				var cookieOptions = new CookieOptions
				{
					HttpOnly = true,
					Expires = DateTimeOffset.UtcNow.AddMinutes(30),
					Secure = true// using HTTPS
				};

				Response.Cookies.Append("ShippingPrice", shippingPriceJson, cookieOptions);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error adding shipping price cookie: {ex.Message}");
			}
			return Json(new { shippingPrice });
		}
		[HttpGet]
		[Route("Cart/DeleteShipping")]
		public IActionResult RemoveShippingCookie()
		{
			Response.Cookies.Delete("ShippingPrice");
			return RedirectToAction("Index", "Cart");
		}

		//Function GetCoupon code
		[HttpPost]
		[Route("Cart/GetCoupon")]
		public async Task<IActionResult> GetCoupon(string coupon_value)
		{
			var validCoupon = await _dataContext.Coupons
				.FirstOrDefaultAsync(x => x.Name == coupon_value);

			if (validCoupon != null)
			{
				string couponTitle = validCoupon.Name + " || " + validCoupon?.Description;
				TimeSpan remainingTime = validCoupon.DateExpired - DateTime.Now;
				int daysRemaining = remainingTime.Days;
				if (daysRemaining >= 0)
				{
					try
					{
						var cookieOptions = new CookieOptions
						{
							HttpOnly = true,
							Expires = DateTimeOffset.UtcNow.AddMinutes(30),
							Secure = true,
							SameSite = SameSiteMode.Strict,
						};

						Response.Cookies.Append("CouponTitle", couponTitle, cookieOptions);
						return Ok(new { success = true, message = "Coupon applied successfully" });
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.ToString());
						return Ok(new { success = false, message = "coupon applied failed" });
					}
				}
				else
				{
					return Ok(new { success = false, message = "Coupon has expired" });
				}
			}
			else
			{
				return Ok(new { success = false, message = "Coupon not existed" });
			}
		}
	}
}

