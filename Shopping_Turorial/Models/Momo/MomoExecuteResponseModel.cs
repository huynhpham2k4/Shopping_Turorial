using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Shopping_Tutorial.Models.Momo
{
	public class MomoExecuteResponseModel
	{
		public string FullName { get; set; }
		public string OrderId { get; set; } // ma cuia don hang
		public string Amount { get; set; }// tong gia tri trong doi hang
		public string OrderInfo { get; set; } // thong tin dat hang cho cua hang abc

	}
}
