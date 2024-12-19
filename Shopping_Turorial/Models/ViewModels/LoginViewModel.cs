using System.ComponentModel.DataAnnotations;

namespace Shopping_Tutorial.Models.ViewModels
{
	public class LoginViewModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "lam on nhap UserName")]
		public string Username { get; set; }

		[DataType(DataType.Password), Required(ErrorMessage = "Lam on nhap Password")]// ma hoa passworld
		public string Password { get; set; }
		public string ReturnUrl { get; set; }
	}
}
