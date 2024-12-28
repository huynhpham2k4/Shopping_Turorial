using System.ComponentModel;

namespace Shopping_Tutorial.Models
{
	public class StatisticalModel
	{
		public int Id { get; set; }
		public int Quantity { get; set; } // so luong ban ra
		public int Sold { get; set; }// so luong don hang
		public string Revenue { get; set; } //doang thu
		public int Profit { get; set; }

		public DateTime DateCreate { get; set; }//ngay dat hang
	}
}
