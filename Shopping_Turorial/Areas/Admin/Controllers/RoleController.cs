using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;
using System.Diagnostics.CodeAnalysis;

namespace Shopping_Tutorial.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Route("Admin/Role")]
	//[Authorize(Roles = "Admin,")]// neu khong co dang nhap thi se khong cho vao : phan quyen
	public class RoleController : Controller
	{
		private readonly Datacontext _dataContext;
		private readonly RoleManager<IdentityRole> _roleManager;

		public RoleController(Datacontext dataContext, RoleManager<IdentityRole> roleManager)
		{
			_dataContext = dataContext;
			_roleManager = roleManager;
		}

		[Route("Index")]
		public async Task<IActionResult> Index()
		{
			return View(await _dataContext.Roles.OrderByDescending(p => p.Id).ToListAsync());
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
		public async Task<IActionResult> Create(IdentityRole model)
		{
			//avoid duplicate role
			if (!_roleManager.RoleExistsAsync(model.Name).GetAwaiter().GetResult())
			{
				_roleManager.CreateAsync(new IdentityRole(model.Name)).GetAwaiter().GetResult();
			}
			return Redirect("Index");
		}

		public async Task<IActionResult> Delete(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return NotFound();//handle missing Id
			}

			var role = await _roleManager.FindByIdAsync(id);

			if (role == null)
			{
				return NotFound();//Handle role not found
			}

			try
			{
				await _roleManager.DeleteAsync(role);
				TempData["success"] = "Role deleted successfully!";
			}
			catch (Exception)
			{
				ModelState.AddModelError("", "An Error occurred while deleting the role.");
			}
			return RedirectToAction("Index");
		}

		[HttpGet]
		[Route("Edit")]
		public async Task<IActionResult> Edit(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return NotFound();//handle missing Id
			}
			var role = await _roleManager.FindByIdAsync(id);

			return View(role);
		}


		[HttpPost]
		[Route("Edit")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(string id, IdentityRole model)
		{
			if (string.IsNullOrEmpty(id))
			{
				return NotFound();//handle missing Id
			}
			if (ModelState.IsValid)//validate model state before proceeding
			{
				var role = await _roleManager.FindByIdAsync(id);

				if (role == null)
				{
					return NotFound();//Handle role not found
				}

				role.Name = model.Name;//Update role properties with model data

				try
				{
					await _roleManager.UpdateAsync(role);
					TempData["success"] = "Role update successfully!";
					return RedirectToAction("Index");

				}
				catch (Exception)
				{
					ModelState.AddModelError("", "An Error occurred while updating the role.");
				}

			}

			//If model is invalid or role not found, return the view with the  model (or an empoty model for a new role)
			return View(model ?? new IdentityRole { Id = id });// pre=populate Id  or provide an empty model
		}


	}
}
