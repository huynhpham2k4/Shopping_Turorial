using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Shopping_Tutorial.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Route("Admin/Slider")]
	//[Authorize(Roles = "Publisher, Admin")]
	public class SliderController : Controller
	{
		private readonly Datacontext _dataContext;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public SliderController(Datacontext dataContext, IWebHostEnvironment webHostEnvironment)
		{
			_dataContext = dataContext;
			_webHostEnvironment = webHostEnvironment;
		}

		[Route("Index")]
		public async Task<IActionResult> Index()
		{
			return View(await _dataContext.Sliders.OrderByDescending(b => b.Id).ToListAsync());
		}

		[Route("Create")]
		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		[Route("Create")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(SliderModel slider)
		{
			if (ModelState.IsValid)// tinh trang cua model product (muc dich cho nay de kiem tra khong co du lieu nao con trong)
			{
				if (slider.ImageUpload != null)
				{
					string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/sliders");
					string imageName = Guid.NewGuid().ToString() + "_" + slider.ImageUpload.FileName;
					string filePath = Path.Combine(uploadsDir, imageName);

					FileStream fs = new FileStream(filePath, FileMode.Create);
					await slider.ImageUpload.CopyToAsync(fs);
					fs.Close();
					slider.Image = imageName;
				}
				_dataContext.Add(slider);
				await _dataContext.SaveChangesAsync();
				TempData["success"] = "Thêm slider thành công";
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
			return View(slider);
		}

		[Route("Edit")]
		public async Task<IActionResult> Edit(int id)
		{
			var slider = await _dataContext.Sliders.FindAsync(id);
			return View(slider);
		}

		[Route("Edit")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(SliderModel slider)
		{
			var slider_existed = _dataContext.Sliders.Find(slider.Id);
			if (ModelState.IsValid)// tinh trang cua model product (muc dich cho nay de kiem tra khong co du lieu nao con trong)
			{
				if (slider.ImageUpload != null)
				{
					string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/sliders");
					string imageName = Guid.NewGuid().ToString() + "_" + slider.ImageUpload.FileName;
					string filePath = Path.Combine(uploadsDir, imageName);

					//delete old picture
					string oldfilePath = Path.Combine(uploadsDir, slider_existed.Image);
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

					FileStream fs = new FileStream(filePath, FileMode.Create);
					await slider.ImageUpload.CopyToAsync(fs);
					fs.Close();
					slider_existed.Image = imageName;
				}

				//update 
				slider_existed.Name = slider.Name;
				slider_existed.Description = slider.Description;
				slider_existed.Status = slider.Status;



				_dataContext.Update(slider_existed);
				await _dataContext.SaveChangesAsync();
				TempData["success"] = "Cập nhật slider thành công";
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
			return View(slider);
		}

		[Route("Delete")]
		public async Task<IActionResult> Delete(int id)
		{
			SliderModel slider = await _dataContext.Sliders.FindAsync(id);
			string UploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/sliders");
			string oldfilePath = Path.Combine(UploadsDir, slider.Image);

			if (System.IO.File.Exists(oldfilePath))
			{
				System.IO.File.Delete(oldfilePath);
			}
			_dataContext.Sliders.Remove(slider);
			await _dataContext.SaveChangesAsync();
			TempData["error"] = "san pham da xoa";

			return RedirectToAction("Index");
		}


	}
}
