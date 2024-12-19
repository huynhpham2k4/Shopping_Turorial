using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;

namespace Shopping_Tutorial.Controllers
{
	public class BrandController : Controller
	{
		private readonly Datacontext _datacontext;
		public BrandController(Datacontext datacontext)
		{
			_datacontext = datacontext;
		}
		public async Task<IActionResult> Index(string Slug = "")
		{

			BrandModel brand = _datacontext.Brands.Where(c => c.Slug == Slug).FirstOrDefault();
			if (brand == null) return RedirectToAction("Index");
			var productsByBrand = _datacontext.Products.Where(p => p.BrandId == brand.Id);

			return View(await productsByBrand.OrderByDescending(p => p.Id).ToListAsync());
		}

	}

}
