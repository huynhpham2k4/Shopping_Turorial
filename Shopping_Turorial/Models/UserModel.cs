using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Shopping_Tutorial.Models
{
	public class UserModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "lam on nhap UserName")]	
		public string Username { get; set; }

		[Required(ErrorMessage = "lam on nhap Email"), EmailAddress]
		public string Email { get; set; }

		[DataType(DataType.Password), Required(ErrorMessage = "Lam on nhap Password")]// ma hoa passworld
		public string Password { get; set; }
	}
}
