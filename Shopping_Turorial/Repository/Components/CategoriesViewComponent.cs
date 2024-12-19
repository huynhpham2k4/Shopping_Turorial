using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Shopping_Tutorial.Repository.Components
{
	public class CategoriesViewComponent : ViewComponent
	{
		private readonly Datacontext _dataContext;
		public CategoriesViewComponent(Datacontext context)
		{
			_dataContext = context;
		}
		public async Task<IViewComponentResult> InvokeAsync() => View(await _dataContext.Categories.ToListAsync());

	}
}
