using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Shopping_Tutorial.Models
{
	public class ProductModel
	{
		[Key]
		public int Id { get; set; }

		[Required, MinLength(4, ErrorMessage = "yêu cầu nhập tên danh mục")]
		public string? Name { get; set; }

		public string? Slug { get; set; }

		[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập mô tả danh mục")]
		public string? Description { get; set; }

		[Required(ErrorMessage = "Yêu cầu nhập giá sản phẩm")]
		[Range(0.01, double.MaxValue)]
		[Column(TypeName = "decimal(10,2)")]
		public decimal Price { get; set; }

		[Required, Range(1, int.MaxValue, ErrorMessage = "Chọn một thương hiệu")]
		public int BrandId { get; set; }

		[Required, Range(1, int.MaxValue, ErrorMessage = "Chọn một danh mục")]
		public int CategoryId { get; set; }

		public CategoryModel Category { get; set; }

		public BrandModel Brand { get; set; }

		public RatingModel Ratings { get; set; }

		public string Image { get; set; }
		public int Quantity { get; set; }

		public int Sold { get; set; }

		[NotMapped]
		[FileExtension]
		public IFormFile? ImageUpload { get; set; }
	}
}
