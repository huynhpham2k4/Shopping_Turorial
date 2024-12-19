using Microsoft.AspNetCore.Http.Metadata;
using Shopping_Tutorial.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping_Tutorial.Models
{
	public class SliderModel
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "Yêu cau khong duoc bo trong ten slider")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Yeu cau khong duoc bo trong ten mo ta")]
		public string Description { get; set; }

		public int Status { get; set; }
		public string Image { get; set; }

		[NotMapped]
		[FileExtension]
		public IFormFile ImageUpload { get; set; }



	}
}
