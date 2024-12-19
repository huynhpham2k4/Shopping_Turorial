using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;

namespace Shopping_Tutorial.Controllers
{
	[Route("Category")]
	public class CategoryController : Controller
	{
		private readonly Datacontext _datacontext;

		public CategoryController(Datacontext datacontext)
		{
			_datacontext = datacontext;
		}


		[Route("Slug={Slug}")]
		public async Task<IActionResult> Index(string Slug = "", string sort_by = "", string startprice = "", string endprice = "")
		{
			CategoryModel category = _datacontext.Categories.Where(c => c.Slug == Slug).FirstOrDefault();


			if (category == null) return RedirectToAction("Index");

			ViewBag.Slug = Slug;
			//Lay tat ca cac san pham
			IQueryable<ProductModel> productsByCategory = _datacontext.Products.Where(p => p.CategoryId == category.Id);
			var count = await productsByCategory.CountAsync();
			if (count > 0)
			{
				if (sort_by == "price_increase")
				{
					productsByCategory = productsByCategory.OrderBy(p => p.Price);
				}
				else if (sort_by == "price_decrease") 
				{
					productsByCategory = productsByCategory.OrderByDescending(p => p.Price);
				}
				else if (sort_by == "newest")
				{
					productsByCategory = productsByCategory.OrderByDescending(p => p.Id);
				}
				else if (sort_by == "oldest")
				{
					productsByCategory = productsByCategory.OrderBy(p => p.Id);
				}
				else if (startprice != "" && endprice != "")
				{
					decimal startPriceValue;
					decimal endPriceValue;
					if (decimal.TryParse(startprice, out startPriceValue) && decimal.TryParse(endprice, out endPriceValue))
					{
						decimal endPriceValue1 = startPriceValue;
						productsByCategory = productsByCategory.Where(p => p.Price >= startPriceValue && p.Price <= endPriceValue);
					}
				}
				else
				{
					productsByCategory = productsByCategory.OrderByDescending(p => p.Id);
				}
			}
			return View(await productsByCategory.ToListAsync());
		}

		//public async Task<IActionResult> Index(string Slug = "")
		//{
		//	CategoryModel category = _datacontext.Categories.Where(c => c.Slug == Slug).FirstOrDefault();


		//	if (category == null) return RedirectToAction("Index");

		//	ViewBag.Slug = Slug;
		//	//Lay tat ca cac san pham
		//	IQueryable<ProductModel> productsByCategory = _datacontext.Products.Where(p => p.CategoryId == category.Id);
		//	var count = await productsByCategory.CountAsync();

		//	return View(await productsByCategory.OrderByDescending(p => p.Price).ToListAsync());
		//}
	}
}

