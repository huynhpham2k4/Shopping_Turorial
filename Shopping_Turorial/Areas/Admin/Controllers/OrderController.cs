using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Repository;
using System.Diagnostics.CodeAnalysis;

namespace Shopping_Tutorial.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Route("Admin/Order")]
	//[Authorize(Roles = "Admin")]
	public class OrderController : Controller
	{
		private readonly Datacontext _dataContext;
		public OrderController(Datacontext datacontext)
		{
			_dataContext = datacontext;
		}

		[Route("Index")]
		public async Task<IActionResult> Index()
		{
			return View(await _dataContext.Orders
			.OrderByDescending(p => p.Id)
			.ToListAsync());
		}

		public async Task<IActionResult> ViewOrder(int status, string ordercode)
		{

			var DetailsOrder = await _dataContext.OrderDetails.Include(od => od.Product).Where(od => od.OrderCode == ordercode).ToListAsync();
			ViewBag.Status = status;

			var orderItem = _dataContext.Orders.Where(o => o.OrderCode == ordercode).FirstOrDefault();
			ViewBag.ShippingCost = orderItem?.ShippingCost;
			ViewBag.Status = orderItem?.Status;
			return View(DetailsOrder);
		}

		[HttpPost]
		[Route("UpdateOrder")]
		public async Task<IActionResult> UpdateOrder(string ordercode, int status)
		{
			var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == ordercode);
			if (order == null)
			{
				return NotFound();
			}

			order.Status = status;

			try
			{
				await _dataContext.SaveChangesAsync();
				return Ok(new { success = true, message = "Order status update successfully" });
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An error occurred white updating the order status.");
			}
		}

	}
}
