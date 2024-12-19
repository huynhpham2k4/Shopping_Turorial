using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Shopping_Tutorial.Repository.Components
{
	public class BrandsViewComponent : ViewComponent
	{
		private readonly Datacontext _dataContext;
		public BrandsViewComponent(Datacontext context)
		{
			_dataContext = context;
		}
		public async Task<IViewComponentResult> InvokeAsync() => View(await _dataContext.Brands.ToListAsync());

	}
}
