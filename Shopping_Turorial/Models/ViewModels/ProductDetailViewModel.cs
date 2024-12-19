using Shopping_Tutorial.Models;
using System.ComponentModel.DataAnnotations;

namespace Shopping_Tutorial.Models.ViewModels
{
	public class ProductDetailViewModel
	{
		public ProductModel ProductDetails { get; set; }

		[Required(ErrorMessage = "Yeu cau nhap binh luan san pham")]
		public string Comment { get; set; }
		[Required(ErrorMessage = "Yeu cau nhap ten")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Yeu cau nhap email")]
		public string Email { get; set; }
	}
}
