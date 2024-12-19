using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;

namespace Shopping_Tutorial.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Route("Admin/Category")]
	//[Authorize(Roles = "Publisher, Author, Admin")]
	public class CategoryController : Controller
	{
		private readonly Datacontext _dataContext;
		public CategoryController(Datacontext datacontext)
		{
			_dataContext = datacontext;
		}

		[Route("Index")]
		public async Task<IActionResult> Index(int pg = 1)
		{
			List<CategoryModel> category = _dataContext.Categories.ToList(); //33 datas


			const int pageSize = 10; //10 items/trang

			if (pg < 1) //page < 1;
			{
				pg = 1; //page ==1
			}
			int recsCount = category.Count(); //33 items;

			var pager = new Paginate(recsCount, pg, pageSize);

			int recSkip = (pg - 1) * pageSize; //(3 - 1) * 10; 

			//category.Skip(20).Take(10).ToList()

			var data = category.Skip(recSkip).Take(pager.PageSize).ToList();

			ViewBag.Pager = pager;

			return View(data);
		}

		public IActionResult Create()
		{
			return View();
		}

		[Route("Create")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(CategoryModel category)
		{
			if (ModelState.IsValid)// neu tinh trang model ok het
			{
				category.Slug = category.Name.Replace(" ", "-");
				var slug = await _dataContext.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug);
				if (slug != null)
				{
					ModelState.AddModelError("", "danh muc da co trong database");
					return View(category);
				}

				_dataContext.Add(category);
				await _dataContext.SaveChangesAsync();
				TempData["success"] = "them danh muc thanh cong";
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

			return View(category);
		}

		[Route("Edit")]
		public async Task<IActionResult> Edit(int Id)
		{
			CategoryModel category = await _dataContext.Categories.FindAsync(Id);
			return View(category);
		}

		[Route("Edit")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, CategoryModel category)
		{
			if (ModelState.IsValid)// neu tinh trang model ok het
			{
				var categoryModel = await _dataContext.Categories.FirstOrDefaultAsync(x => x.Id == id);
				if (categoryModel != null)
				{
					category.Slug = category.Name.Replace(" ", "-");
					_dataContext.Entry(categoryModel).CurrentValues.SetValues(category);
					await _dataContext.SaveChangesAsync();
					TempData["success"] = "Cap nhat danh muc thanh cong";
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

			return View(category);
		}

		[Route("Delete")]
		public async Task<IActionResult> Delete(int id)
		{
			CategoryModel category = await _dataContext.Categories.FindAsync(id);
			_dataContext.Categories.Remove(category);
			await _dataContext.SaveChangesAsync();
			TempData["success"] = "Danh muc da xoa";
			return RedirectToAction("Index");
		}
	}
}
