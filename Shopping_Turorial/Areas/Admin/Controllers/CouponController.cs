using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;

namespace Shopping_Tutorial.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Route("Admin/Coupon")]
	//[Authorize(Roles = "Admin, Publisher, Author")]// neu khong co dang nhap thi se khong cho vao : phan quyen

	public class CouponController : Controller
	{
		private readonly Datacontext _dataContext;
		public CouponController(Datacontext datacontext)
		{
			_dataContext = datacontext;
		}
		[Route("Index")]
		public async Task<IActionResult> Index()
		{
			var coupon_List = await _dataContext.Coupons.ToListAsync();
			ViewBag.Coupons = coupon_List;
			return View();
		}

		[Route("Create")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(CouponModel coupon)
		{
			if (ModelState.IsValid)// neu tinh trang model ok het
			{

				_dataContext.Coupons.Add(coupon);
				await _dataContext.SaveChangesAsync();
				TempData["success"] = "them coupon thanh cong";
				return RedirectToAction("Index");
			}
			else
			{
				TempData["error"] = "Model co mot vai thu dang bi loi";
				List<string> errors = new List<string>();
				foreach (var value in ModelState.Values)
				{
					foreach (var error in value.Errors)
					{
						errors.Add(error.ErrorMessage);
					}
				}
				string errorMessage = string.Join("\n", errors);
				return BadRequest(errorMessage);
			}

		}
	}

}
