using Microsoft.AspNetCore.Mvc;
using AlHersey.Models;
namespace AlHersey.ViewComponents
{
	public class Menus : ViewComponent
	{
		AlHerseyContext context = new AlHerseyContext();

		public IViewComponentResult Invoke()
		{
			List<Category> categories= context.Categories.ToList();
			return View(categories);


		}
	}
}
