using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.VisualBasic;
using Shopping_Tutorial.Areas.Admin.Repository;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Models.ViewModels;
using Shopping_Tutorial.Repository;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Shopping_Tutorial.Controllers
{
	public class AccountController : Controller
	{
		//quang li user
		private UserManager<AppUserModel> _userManager;
		//quang li dang nhap
		private SignInManager<AppUserModel> _signInManager;// cai  nay cau aspnetcoreIdentity
		private readonly Datacontext _dataContext;
		private readonly IEmailSender _emailSender;

		//dang inject
		public AccountController(SignInManager<AppUserModel> signInManager, UserManager<AppUserModel> userManager, Datacontext datacontext, IEmailSender emailSender
			)
		{
			_dataContext = datacontext;
			_signInManager = signInManager;
			_userManager = userManager;
			_emailSender = emailSender;
		}
		public async Task<IActionResult> Login(string returnUrl)
		{
			return View(new LoginViewModel { ReturnUrl = returnUrl });
		}

		public async Task<IActionResult> UpdateAccount()
		{
			if ((bool)!User.Identity?.IsAuthenticated)
			{
				return RedirectToAction("Login", "Account");
			}

			//Chua hieu cho nay sao lay ra duoc user
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var userEmail = User.FindFirstValue(ClaimTypes.Email);
			//get user by user email;
			var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
			if (user == null)
			{
				return NotFound();
			}
			return View(user);
		}

		[HttpPost]
		public async Task<IActionResult> UpdateInfoAccount(AppUserModel user)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			//get user by user email;
			var userById = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
			if (userById == null)
			{
				return NotFound();
			}
			else
			{
				var passwordHasher = new PasswordHasher<AppUserModel>();
				var passwordHash = passwordHasher.HashPassword(userById, user.PasswordHash);

				userById.PasswordHash = passwordHash;
				userById.PhoneNumber = user.PhoneNumber;
				//-Hash the new password
				_dataContext.Update(userById);
				await _dataContext.SaveChangesAsync();
				TempData["success"] = "Update Account Information Successfylly";
			}
			return RedirectToAction("UpdateAccount", "Account");

		}

		public async Task<IActionResult> NewPass(AppUserModel User, string token)
		{
			var checkuser = await _userManager.Users
				.Where(u => u.Email == User.Email)
				.Where(u => u.Token == User.Token).FirstOrDefaultAsync();
			if (checkuser != null)
			{
				ViewBag.Email = checkuser.Email;
				ViewBag.Token = token;
			}
			else
			{
				TempData["error"] = "Email not found or token is not right";
				return RedirectToAction("ForgetPass", "Account");
			}
			return View();
		}


		[HttpPost]
		public async Task<IActionResult> UpdateNewPassword(AppUserModel user, string token)
		{
			var checkuser = await _userManager.Users
				.Where(u => u.Email == user.Email)
				.Where(u => u.Token == user.Token).FirstOrDefaultAsync();
			if (checkuser != null)
			{
				//update user with new password
				string newtoken = Guid.NewGuid().ToString();
				//Hash the new password
				var passwordHasher = new PasswordHasher<AppUserModel>();
				var passwordHash = passwordHasher.HashPassword(checkuser, user.PasswordHash);

				checkuser.PasswordHash = passwordHash;
				checkuser.Token = newtoken;

				await _userManager.UpdateAsync(checkuser);
				_dataContext.SaveChanges();
				TempData["success"] = "Password update success";
				return RedirectToAction("Login", "Account");

			}
			else
			{
				TempData["error"] = "Email not found or token is not righr";
				return RedirectToAction("ForgetPass", "Account");

			}
			return View();
		}

		public async Task<IActionResult> ForgetPass(string returnUrl)
		{
			return View();
		}

		public async Task<IActionResult> SendMailForgetPass(AppUserModel user)
		{
			var checkMail = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
			if (checkMail == null)
			{
				TempData["error"] = "Email not found";
				return RedirectToAction("ForgetPass", "Account");
			}
			else
			{
				string token = Guid.NewGuid().ToString();
				//update token to user
				checkMail.Token = token;
				_dataContext.Update(checkMail);
				await _dataContext.SaveChangesAsync();
				var reciver = checkMail.Email;
				var subject = "Change password for user " + checkMail.Email; //TIeu de  
				var message = "Click on link to change password " +
					"<a href ='" + $"{Request.Scheme}://{Request.Host}/Account/NewPass?" +
					$"email=" + checkMail.Email + "&token=" + token + "'> </a>";// co 2 tham so

				await _emailSender.SendEmailAsync(reciver, subject, message);
			}
			TempData["success"] = "An email has been sent to your register email address with password";
			return RedirectToAction("ForgetPass", "Account");
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel loginVM)
		{
			if (ModelState.IsValid)
			{
				Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginVM.Username, loginVM.Password, false, false);
				if (result.Succeeded)
				{
					return Redirect(loginVM.ReturnUrl ?? "/");
				}
				ModelState.AddModelError("", "Invalid Username and Password");
			}
			return View(loginVM);
		}


		public async Task<IActionResult> Create()
		{
			return View();
		}

		public async Task<IActionResult> History()
		{
			if ((bool)!User.Identity?.IsAuthenticated)
			{
				return RedirectToAction("Login", "Account");
			}

			//Chua hieu cho nay sao lay ra duoc user
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var userEmail = User.FindFirstValue(ClaimTypes.Email);

			var Orders = await _dataContext.Orders
				.Where(od => od.UserName == userEmail).OrderByDescending(od => od.Id).ToListAsync();

			ViewBag.UserEmail = userEmail;
			return View(Orders);
		}

		public async Task<IActionResult> CancelOrder(string orderCode)
		{
			try
			{
				var order = await _dataContext.Orders.Where(o => o.OrderCode == orderCode).FirstOrDefaultAsync();
				order.Status = 3;
				_dataContext.Update(order);
				await _dataContext.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				return BadRequest("An error occurred while canceling the order.");
			}
			return RedirectToAction("History", "Account");
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(UserModel user)
		{
			if (ModelState.IsValid)
			{
				AppUserModel newUser = new AppUserModel
				{
					UserName = user.Username,
					Email = user.Email
				};
				IdentityResult result = await _userManager.CreateAsync(newUser, user.Password); // tao duco user moi

				if (result.Succeeded)
				{
					TempData["Success"] = "Tao user thanh cong.";
					return Redirect("/Account/Login");
				}
				foreach (IdentityError error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
			}
			return View(user);
		}

		public async Task<IActionResult> Logout(string returnUrl = "/")
		{
			await _signInManager.SignOutAsync();
			return Redirect(returnUrl);
		}

		public async Task LoginByGoogle()
		{
			await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
				new AuthenticationProperties
				{
					RedirectUri = Url.Action("GoogleResponse")
				});
		}

		public async Task<IActionResult> GoogleResponse()
		{
			var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
			{
				claim.Issuer,
				claim.OriginalIssuer,
				claim.Type,
				claim.Value
			});
			TempData["success"] = "Dang nhap thanh cong";
			return RedirectToAction("Index", "Home");
			return Json(claims);
		}

	}
}
