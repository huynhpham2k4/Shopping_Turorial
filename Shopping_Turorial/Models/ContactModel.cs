using Shopping_Tutorial.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping_Tutorial.Models
{
	public class ContactModel
	{
		[Key]
		[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập tiêu đè website")]
		public string Name { get; set; }

		[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập bản đồ")]
		public string Map { get; set; }

		[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập Email liên hệ")]
		public string Email { get; set; }

		[Required, MinLength(4, ErrorMessage = "Yêu cầu số điện thoại")]
		public string Phone { get; set; }

		[Required, MinLength(4, ErrorMessage = "Yêu cầu nhập thông tin liên hệ")]
		public string Description { get; set; }
		public string LogoImg { get; set; }

		[NotMapped]
		[FileExtension]
		public IFormFile? ImageUpload { get; set; }
	}
}
