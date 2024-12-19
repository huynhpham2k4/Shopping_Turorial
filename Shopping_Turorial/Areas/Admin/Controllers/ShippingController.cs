using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;
using System.Diagnostics.CodeAnalysis;

namespace Shopping_Tutorial.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Route("Admin/Shipping")]
	//[Authorize(Roles = "Admin, Publisher, Author")]// neu khong co dang nhap thi se khong cho vao : phan quyen
	public class ShippingController : Controller
	{
		private readonly Datacontext _dataContext;
		public ShippingController(Datacontext datacontext)
		{
			_dataContext = datacontext;
		}

		[Route("Index")]
		public async Task<IActionResult> Index()
		{
			var shippingList = await _dataContext.Shippings.ToListAsync();
			ViewBag.ShippingList = shippingList;
			return View();
		}

		[HttpPost]
		[Route("StoreShipping")]
		public async Task<IActionResult> StoreShipping(string tinh, string quan, string phuong, decimal price)
		{
			try
			{
				var existingShipping = await _dataContext.Shippings
					.AnyAsync(x => x.City == tinh && x.District == quan && x.Ward == phuong);
				// kei tra co data chua
				if (existingShipping)
				{
					return Ok(new { duplicate = true, message = "Dữ liệu trùng lặp." });
				}
				ShippingModel shippingModel = new ShippingModel();
				shippingModel.City = tinh;
				shippingModel.District = quan;
				shippingModel.Ward = phuong;
				shippingModel.Price = price;

				_dataContext.Shippings.Add(shippingModel);
				await _dataContext.SaveChangesAsync();
				return Ok(new { success = true, message = "Thêm shipping thành công" });
			}
			catch (Exception)
			{
				return StatusCode(500, "An error occured while adding shipping.");
			}
		}

		[Route("id")]
		public async Task<IActionResult> Delete(int Id)
		{
			ShippingModel shipping = await _dataContext.Shippings.FindAsync(Id);

			_dataContext.Shippings.Remove(shipping);
			await _dataContext.SaveChangesAsync();
			TempData["Success"] = "Shipping da duoc xoa thanh cong";
			return RedirectToAction("Index", "Shipping");
		}

	}
}
