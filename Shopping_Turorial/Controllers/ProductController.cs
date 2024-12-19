using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Shopping_Tutorial.Migrations;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Models.ViewModels;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;
using System.Diagnostics;
using Shopping_Tutorial.Models.ViewModels;
namespace Shopping_Tutorial.Controllers
{
	public class ProductController : Controller
	{
		private readonly Datacontext _dataContext;
		public ProductController(Datacontext datacontext)
		{
			_dataContext = datacontext;
		}
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Search(string searchTerm)
		{
			var products = await _dataContext.Products
				.Where(p => p.Name.Contains(searchTerm) || p.Name.Contains(searchTerm)).ToListAsync();
			ViewBag.Keyword = searchTerm;// de hien thi tu khoa tim khiem
			return View(products);
		}

		public async Task<IActionResult> Details(int id)
		{
			if (id == null) return RedirectToAction("Index");
			var productId = _dataContext.Products.Include(p => p.Ratings).Where(p => p.Id == id).FirstOrDefault();// category = 4

			//related product
			var relatedProducts = await _dataContext.Products
			.Where(p => p.CategoryId == productId.CategoryId && p.Id != productId.Id)
			.Take(4)
			.ToListAsync();


			ViewBag.RelatedProducts = relatedProducts;

			var viewModel = new ProductDetailViewModel
			{
				ProductDetails = productId,
			};

			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CommentProduct(RatingModel rating)
		{
			if (ModelState.IsValid)
			{
				var ratingEntity = new RatingModel
				{
					ProductId = rating.ProductId,
					Name = rating.Name,
					Email = rating.Email,
					Comment = rating.Comment,
					Star = rating.Star,
				};
				_dataContext.Ratings.Add(ratingEntity);
				await _dataContext.SaveChangesAsync();
				TempData["Success"] = "Them danh gia thanh cong";
				return Redirect(Request.Headers["Referer"]);
			}
			else
			{
				TempData["error"] = "Model co 1 vai thu dang loi";
				List<string> errors = new List<string>();
				foreach (var value in ModelState.Values)
				{
					foreach (var error in value.Errors)
					{
						errors.Add(error.ErrorMessage);
					}

				}
				string errorMessage = string.Join("\n", errors);
				return RedirectToAction("Details", new { id = rating.ProductId });
			}

			return Redirect(Request.Headers["referer"]);

		}
	}
}
