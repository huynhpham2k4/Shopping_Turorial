using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Shopping_Tutorial.Repository.Components
{
	public class FooterViewComponent : ViewComponent
	{
		private readonly Datacontext _dataContext;
		public FooterViewComponent(Datacontext context)
		{
			_dataContext = context;
		}
		public async Task<IViewComponentResult> InvokeAsync() => View(await _dataContext.Contacts.FirstOrDefaultAsync());

	}
}
