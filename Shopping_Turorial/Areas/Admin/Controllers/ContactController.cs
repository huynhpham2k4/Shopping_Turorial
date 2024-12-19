using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;

namespace Shopping_Tutorial.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Route("Admin/Contact")]
	//[Authorize(Roles = "Admin")]
	public class ContactController : Controller
	{
		private readonly Datacontext _dataContext;
		private readonly IWebHostEnvironment _webHostEnvironment;
		public ContactController(Datacontext datacontext, IWebHostEnvironment _webHostEnvironment)
		{
			_dataContext = datacontext;
			this._webHostEnvironment = _webHostEnvironment;
		}

		[Route("Index")]
		public IActionResult Index()
		{
			var contact = _dataContext.Contacts.ToList();
			return View(contact);
		}

		[Route("Edit")]
		public async Task<IActionResult> Edit()
		{
			ContactModel contact = await _dataContext.Contacts.FirstOrDefaultAsync();
			return View(contact);
		}


		[Route("Edit")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(ContactModel contact)
		{

			var exited_contact = await _dataContext.Contacts.FirstOrDefaultAsync(); // lay dl dau tien

			if (ModelState.IsValid)// tinh trang cua model product
			{
				if (contact.ImageUpload != null)
				{

					//uplaod new image
					string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/logo");// lay duong dan chua hinh anh
					string imageName = Guid.NewGuid().ToString() + "_" + contact.ImageUpload.FileName;//tai ra chuoi string
					string filePath = Path.Combine(uploadsDir, imageName);// create a full 

					//delete old picture
					string oldfilePath = Path.Combine(uploadsDir, string.IsNullOrEmpty(exited_contact.LogoImg) ? "" : exited_contact.LogoImg);

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
					FileStream fs = new FileStream(filePath, FileMode.Create);//  cat nho cai file ra
					await contact.ImageUpload.CopyToAsync(fs);// copy content from ImageUpload to the fs
					fs.Close();
					exited_contact.LogoImg = imageName;// new image



				}
				//update other product properties
				//exited_contact.Name = contact.Name;// khong update duoc vi no la frimary key
				exited_contact.Description = contact.Description;
				exited_contact.Map = contact.Map;
				exited_contact.Phone = contact.Phone;
				exited_contact.Email = contact.Email;
				//...other properties

				_dataContext.Update(exited_contact);//update the exiting product object

				await _dataContext.SaveChangesAsync();
				TempData["success"] = "Cap nhat thong tin thanh cong";
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
