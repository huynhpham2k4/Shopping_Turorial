using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Shopping_Tutorial.Models
{
	public class ProductQuantityModel
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Yeu cau khong duoc bo trong so luong sp")]
		public int Quantity { get; set; }
		public int ProductId { get; set; }
		public DateTime DateCreated { get; set; }
	}
}
