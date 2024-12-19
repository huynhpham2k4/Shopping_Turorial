using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Shopping_Tutorial.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Route("Admin/User")]
	//[Authorize(Roles = "Admin")]
	public class UserController : Controller
	{
		private readonly UserManager<AppUserModel> _userManager;// vi sao dung dbcontext cx duoc ma dun usermanager cx duoc
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly Datacontext _datacontext;
		public UserController(UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager, Datacontext datacontext)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_datacontext = datacontext;
		}

		[HttpGet]
		[Route("Index")]
		public async Task<IActionResult> Index()
		{
			var usersWithRoles = await (from u in _datacontext.Users
										join ur in _datacontext.UserRoles on u.Id equals ur.UserId
										join r in _datacontext.Roles on ur.RoleId equals r.Id
										select new { User = u, RoleName = r.Name }
										).ToListAsync();
			return View(usersWithRoles);
		}

		[HttpGet]
		[Route("Create")]
		public async Task<IActionResult> Create()
		{
			var roles = await _roleManager.Roles.ToListAsync();
			ViewBag.Roles = new SelectList(roles, "Id", "Name");
			return View(new AppUserModel());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("Create")]
		public async Task<IActionResult> Create(AppUserModel user)
		{
			if (ModelState.IsValid)
			{
				var createUserResult = await _userManager.CreateAsync(user, user.PasswordHash);// tao user
				if (createUserResult.Succeeded)
				{
					var createUser = await _userManager.FindByEmailAsync(user.Email);// tim user dua vao email
					var userId = createUser.Id;// lay user Id
					var role = _roleManager.FindByIdAsync(user.RoleId);// lay RoleId
																	   // gan quyen
					var addToRoleResult = await _userManager.AddToRoleAsync(createUser, role.Result.Name);
					if (!addToRoleResult.Succeeded)
					{
						foreach (var error in createUserResult.Errors)
						{
							ModelState.AddModelError(string.Empty, error.Description);
						}
					}
					return RedirectToAction("Index", "User");
				}
				else
				{
					AddIdentityErrors(createUserResult);
					return View(user);
				}
			}
			TempData["Error"] = "Model co mot vai thu dang loi";
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

		private void AddIdentityErrors(IdentityResult identityResult)
		{
			foreach (var error in identityResult.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
		}

		[HttpGet]
		[Route("Delete")]
		public async Task<IActionResult> Delete(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return (NotFound());

			}
			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				return NotFound();
			}
			var deleteUserRole = await _userManager.RemoveFromRoleAsync(user, _roleManager.FindByIdAsync(user.RoleId).Result.Name);
			var deleteResult = await _userManager.DeleteAsync(user);
			if (!deleteResult.Succeeded)
			{
				return View("Error");
			}
			TempData["success"] = "User da duoc xoa thanh cong";
			return RedirectToAction("Index");
		}

		[HttpGet]
		[Route("Edit")]
		public async Task<IActionResult> Edit(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return (NotFound());

			}
			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				return NotFound();
			}

			var roles = await _roleManager.Roles.ToArrayAsync();
			ViewBag.Roles = new SelectList(roles, "Id", "Name");
			return View(user);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("Edit")]
		public async Task<IActionResult> Edit(string id, AppUserModel user)
		{
			var existingUser = await _userManager.FindByIdAsync(id);
			if (existingUser == null)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				existingUser.UserName = user.UserName;
				existingUser.Email = user.Email;
				existingUser.PhoneNumber = user.PhoneNumber;
				existingUser.RoleId = user.RoleId;

				//update user
				var updateUserResult = await _userManager.UpdateAsync(existingUser);

				//delete userRoles
				var currentRoles = await _userManager.GetRolesAsync(existingUser);
				foreach (var role in currentRoles)
				{
					await _userManager.RemoveFromRoleAsync(existingUser, role);
				}

				//add userrole
				var updateUserRole = await _userManager.AddToRoleAsync(existingUser, _roleManager.FindByIdAsync(existingUser.RoleId).Result.Name);
				if (updateUserResult.Succeeded)
				{
					return RedirectToAction("Index", "User");
				}
				else
				{
					AddIdentityErrors(updateUserResult);
					return View(existingUser);
				}
			}

			var roles = await _roleManager.Roles.ToArrayAsync();
			ViewBag.Roles = new SelectList(roles, "Id", "Name");

			//Model validation failed
			TempData["error"] = "Model validation failed.";
			var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
			string errorMessage = string.Join("\n", errors);

			return View(existingUser);
		}
	}
}
