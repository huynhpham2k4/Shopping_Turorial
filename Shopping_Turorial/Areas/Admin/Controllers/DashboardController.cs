﻿//using AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Shopping_Tutorial.Repository;

namespace Shopping_Tutorial.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Route("Admin/Dashboard")]
	//[Authorize(Roles = "Admin")] 
	public class DashboardController : Controller
	{
		private readonly Datacontext _dataContext;
		private readonly IWebHostEnvironment _webHostEnvironment;// Moi truong file
		public DashboardController(Datacontext datacontext, IWebHostEnvironment _webHostEnvironment)
		{
			_dataContext = datacontext;
			this._webHostEnvironment = _webHostEnvironment;
		}
		public IActionResult Index()
		{
			var count_product = _dataContext.Products.Count();
			var count_order = _dataContext.Orders.Count();
			var count_category = _dataContext.Categories.Count();
			var count_user = _dataContext.Users.Count();
			ViewBag.CountProduct = count_product;
			ViewBag.CountOrder = count_order;
			ViewBag.CountCategory = count_category;
			ViewBag.CountUser = count_user;
			return View();
		}

		[HttpPost]
		[Route("GetChartData")]
		public async Task<IActionResult> GetChartData()
		{
			var data = _dataContext.Statisticals.Select(s => new
			{
				date = s.DateCreate.ToString("yyyy-MM-dd"),
				sold = s.Sold,
				quantity = s.Quantity,
				revenue = s.Revenue,
				profit = s.Profit,
			}).ToList();

			return Json(data);// tra ve kieu json de cap nhat ve trong biuee do 
		}

		[HttpPost]
		[Route("GetChartDataBySelect")]
		public IActionResult GetChartDataBySelect(DateTime startDate, DateTime endDate)
		{
			var data = _dataContext.Statisticals
				.Where(s => s.DateCreate >= startDate && s.DateCreate <= endDate)
				.Select(s => new
				{
					date = s.DateCreate.ToString("yyyy-MM-dd"),
					sold = s.Sold,
					quantity = s.Quantity,
					revenue = s.Revenue,
					profit = s.Profit,
				}).ToList();
			return Json(data);
		}

		[HttpPost]
		[Route("FilterData")]
		public IActionResult FilterData(DateTime? fromDate, DateTime? toDate)
		{
			var query = _dataContext.Statisticals.AsQueryable();

			if (fromDate.HasValue)
			{
				query = query.Where(s => s.DateCreate >= fromDate);

			}
			if (toDate.HasValue)
			{
				query = query.Where(s => s.DateCreate <= toDate);
			}

			var data = query
				.Select(s => new
				{
					date = s.DateCreate.ToString("yyyy-MM-dd"),
					sold = s.Sold,
					quantity = s.Quantity,
					revenue = s.Revenue,
					profit = s.Profit,
				})
				.ToList();
			return Json(data);
		}

	}
}
