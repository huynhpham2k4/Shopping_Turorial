using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Models.Momo;

namespace Shopping_Tutorial.Services.Momo
{
	public interface IMomoService
	{
		Task<MomoCreatePaymentResponseModel> CreatePaymentMomo(OrderInfoModel model);
		MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
	}
}
