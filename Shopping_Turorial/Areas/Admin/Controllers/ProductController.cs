using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;
using System.Reflection.Metadata.Ecma335;
using System.Xml;

namespace Shopping_Tutorial.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Route("Admin/Product")]
	//[Authorize(Roles = "Admin")]
	public class ProductController : Controller
	{
		private readonly Datacontext _dataContext;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public ProductController(Datacontext datacontext, IWebHostEnvironment _webHostEnvironment)
		{
			_dataContext = datacontext;
			this._webHostEnvironment = _webHostEnvironment;
		}
		[Route("Index")]
		public async Task<IActionResult> Index()
		{
			return View(await _dataContext.Products.OrderByDescending(p => p.Id).Include(p => p.Category).Include(p => p.Brand).ToListAsync());
		}

		[Route("Create")]
		[HttpGet]
		public IActionResult Create()
		{
			ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
			ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");
			return View();
		}
		[Route("Create")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(ProductModel product)
		{
			ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
			ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

			if (ModelState.IsValid)// tinh trang cua model product
			{
				product.Slug = product.Name.Replace(" ", "-");
				var slug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
				if (slug != null)
				{
					ModelState.AddModelError("", "san pham da co trong database");
					return View(product);
				}
				if (product.ImageUpload != null)
				{
					string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
					string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
					string filePath = Path.Combine(uploadsDir, imageName);

					FileStream fs = new FileStream(filePath, FileMode.Create);
					await product.ImageUpload.CopyToAsync(fs);
					fs.Close();
					product.Image = imageName;


				}
				_dataContext.Add(product);
				await _dataContext.SaveChangesAsync();
				TempData["success"] = "cap nhat san pham thanh cong";
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

			return View(product);
		}

		[Route("Edit")]
		public async Task<IActionResult> Edit(int id)
		{
			ProductModel product = await _dataContext.Products.FindAsync(id);
			ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
			ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);
			return View(product);
		}


		[Route("Edit")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, ProductModel product)
		{
			ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
			ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

			var exited_product = _dataContext.Products.Find(product.Id); //tim sp theo id product

			if (ModelState.IsValid)// tinh trang cua model product
			{
				exited_product.Slug = product.Name.Replace(" ", "-");

				if (product.ImageUpload != null)
				{

					//uplaod new image
					string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
					string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;//new image
					string filePath = Path.Combine(uploadsDir, imageName);

					//delete old picture
					string oldfilePath = Path.Combine(uploadsDir, exited_product.Image);
					try
					{
						if (System.IO.File.Exists(oldfilePath))
						{
							System.IO.File.Delete(oldfilePath);
						}
					}
					catch (Exception e)
					{
						ModelState.AddModelError("", "An error occurred while deleting the product image");
					}

					//assign new image
					FileStream fs = new FileStream(filePath, FileMode.Create);
					await product.ImageUpload.CopyToAsync(fs);
					fs.Close();
					exited_product.Image = imageName;// new image



				}
				//update other product properties
				exited_product.Name = product.Name;
				exited_product.Description = product.Description;
				exited_product.Price = product.Price;
				exited_product.CategoryId = product.CategoryId;
				exited_product.BrandId = product.BrandId;
				//...other properties

				_dataContext.Update(exited_product);//update the exiting product object

				await _dataContext.SaveChangesAsync();
				TempData["success"] = "Them san pham thanh cong";
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

			return View(product);
		}

		[Route("Delete")]
		public async Task<IActionResult> Delete(int id)
		{
			ProductModel product = await _dataContext.Products.FindAsync(id);
			if (!string.Equals(product.Image, "noname.jpg"))
			{
				string UploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
				string oldfilePath = Path.Combine(UploadsDir, product.Image);

				if (System.IO.File.Exists(oldfilePath))
				{
					System.IO.File.Delete(oldfilePath);
				}
			}
			_dataContext.Products.Remove(product);
			await _dataContext.SaveChangesAsync();
			TempData["error"] = "san pham da xoa";

			return RedirectToAction("Index");
		}
		[Route("AddProduct")]
		public async Task<IActionResult> AddQuantity(int id)
		{
			var productbyquantity = await _dataContext.productQuantities.Where(pp => pp.ProductId == id).ToListAsync();
			ViewBag.ProductByQuantity = productbyquantity;
			ViewBag.Id = id;
			return View();
		}

		[Route("StoreProductQuantity")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> StoreProductQuantity(ProductQuantityModel productQuantityModel)
		{
			var product = _dataContext.Products.Find(productQuantityModel.ProductId);
			if (product == null)
			{
				return NotFound();
			}
			product.Quantity += productQuantityModel.Quantity;

			productQuantityModel.Quantity = productQuantityModel.Quantity;
			productQuantityModel.ProductId = productQuantityModel.ProductId;
			productQuantityModel.DateCreated = DateTime.Now;

			_dataContext.productQuantities.Add(productQuantityModel);
			_dataContext.SaveChangesAsync();
			TempData["Success"] = "Them so luong san pham thanh cong";
			return RedirectToAction("AddQuantity", "Product", new { Id = productQuantityModel.ProductId });
		}
	}
}
