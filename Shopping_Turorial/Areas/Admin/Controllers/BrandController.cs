using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;

namespace Shopping_Tutorial.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Route("Admin/Brand")]
	//[Authorize(Roles = "Admin, Publisher, Author")]// neu khong co dang nhap thi se khong cho vao : phan quyen

	public class BrandController : Controller
	{
		private readonly Datacontext _dataContext;
		public BrandController(Datacontext datacontext)
		{
			_dataContext = datacontext;
		}

		[Route("Index")]
		public async Task<IActionResult> Index()
		{
			return View(await _dataContext.Brands.OrderByDescending(p => p.Id).ToListAsync());
		}

		[Route("Create")]
		public IActionResult Create()
		{
			return View();
		}

		[Route("Create")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(BrandModel brand)
		{
			if (ModelState.IsValid)// neu tinh trang model ok het
			{
				brand.Slug = brand.Name.Replace(" ", "-");
				var slug = await _dataContext.Brands.FirstOrDefaultAsync(p => p.Slug == brand.Slug);
				if (slug != null)
				{
					ModelState.AddModelError("", "thuong hien da co trong database");
					return View(brand);
				}

				_dataContext.Brands.Add(brand);
				await _dataContext.SaveChangesAsync();
				TempData["success"] = "thuong hieu muc thanh cong";
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

		[Route("Edit")]
		public async Task<IActionResult> Edit(int Id)
		{
			BrandModel brand = await _dataContext.Brands.FindAsync(Id);
			return View(brand);
		}


		[Route("Edit")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, BrandModel brand)
		{
			if (ModelState.IsValid)
			{
				var branddb = await _dataContext.Brands.FirstOrDefaultAsync(x => x.Id == id);
				if (branddb != null)
				{
					// Update the Slug property based on the new Name
					brand.Slug = brand.Name.Replace(" ", "-");

					// Copy all properties from 'brand' to 'branddb'
					_dataContext.Entry(branddb).CurrentValues.SetValues(brand);

					await _dataContext.SaveChangesAsync();
					TempData["success"] = "Cập nhật thương hiệu thành công";
					return RedirectToAction("Index");
				}
				else
				{
					return NotFound();
				}
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

		[Route("Delete")]
		public async Task<IActionResult> Delete(int id)
		{
			BrandModel brand = await _dataContext.Brands.FindAsync(id);
			_dataContext.Brands.Remove(brand);
			await _dataContext.SaveChangesAsync();
			TempData["success"] = "thuong hieu da xoa";
			return RedirectToAction("Index");
		}
	}
}
