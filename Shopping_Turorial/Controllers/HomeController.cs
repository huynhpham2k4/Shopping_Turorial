using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Shopping_Tutorial.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ILogger<HomeController> _loggerFactory;
		private readonly UserManager<AppUserModel> _userManager;

		private readonly Datacontext _dataContext;
		public HomeController(ILogger<HomeController> logger, Datacontext context, UserManager<AppUserModel> userManager)
		{
			_logger = logger;
			_dataContext = context;
			_userManager = userManager;
		}

		public IActionResult Index()
		{
			var products = _dataContext.Products.Include("Category").Include("Brand").ToList();
			var sliders = _dataContext.Sliders.Where(s => s.Status == 1).ToList();
			ViewBag.Sliders = sliders;
			return View(products);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		public async Task<IActionResult> Contact()
		{
			var contact = await _dataContext.Contacts.FirstOrDefaultAsync();
			return View(contact);
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error(int statuscode)
		{
			if (statuscode == 404)
			{
				return View("NotFound");
			}
			else
			{
				return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
			}
		}

		//AddWishlist
		[HttpPost]
		public async Task<IActionResult> AddWishlist(int id, WishlistModel wishlistmodel)
		{
			var user = await _userManager.GetUserAsync(User);

			var wishlistProduct = new WishlistModel
			{
				ProductId = id,
				UserId = user.Id,
			};

			_dataContext.Wishlists.Add(wishlistProduct);
			try
			{
				await _dataContext.SaveChangesAsync();
				return Ok(new { success = true, message = "Add to wishlist Successfully" });
			}
			catch (Exception)
			{
				return StatusCode(500, "An error occurred while adding to wishlist table ");
			}
		}

		//AddCompare
		[HttpPost]
		public async Task<IActionResult> AddCompare(int id, CompareModel comparemodel)
		{
			var user = await _userManager.GetUserAsync(User);

			var compareModel = new CompareModel
			{
				ProductId = id,
				UserId = user.Id,
			};

			_dataContext.Compares.Add(compareModel);
			try
			{
				await _dataContext.SaveChangesAsync();
				return Ok(new { success = true, message = "Add to compare Successfully" });
			}
			catch (Exception)
			{
				return StatusCode(500, "An error occurred while adding to compare table ");
			}
		}

		public async Task<IActionResult> Wishlist()
		{
			var wishlist_product = await (from w in _dataContext.Wishlists
										  join p in _dataContext.Products on w.ProductId equals p.Id
										  join u in _dataContext.Users on w.UserId equals u.Id
										  select new { User = u, Product = p, Wishlist = w }).ToListAsync();
			return View(wishlist_product);
		}

		public async Task<IActionResult> Compare()
		{
			var compare_product = await (from w in _dataContext.Compares
										 join p in _dataContext.Products on w.ProductId equals p.Id
										 join u in _dataContext.Users on w.UserId equals u.Id
										 select new { User = u, Product = p, Compare = w }).ToListAsync();
			return View(compare_product);
		}

		public async Task<IActionResult> DeleteCompare(int id)
		{
			CompareModel compare = await _dataContext.Compares.FindAsync(id);

			_dataContext.Compares.Remove(compare);
			await _dataContext.SaveChangesAsync();
			TempData["success"] = "Compare đã được xóa thành công";
			return RedirectToAction("Compare", "Home");
		}
		public async Task<IActionResult> DeteleWishlist(int id)
		{
			WishlistModel wishlist = await _dataContext.Wishlists.FindAsync(id);

			_dataContext.Wishlists.Remove(wishlist);
			await _dataContext.SaveChangesAsync();
			TempData["success"] = "Wishlist đã được xóa thành công";
			return RedirectToAction("Wishlist", "Home");
		}
	}
}
