using Microsoft.AspNetCore.Mvc;
using AlHersey.Models;

namespace AlHersey.ViewComponents
{
    public class Footers : ViewComponent
    {
		AlHerseyContext context = new AlHerseyContext();

		public IViewComponentResult Invoke()
        {
            List<Supplier> suppliers = context.Suppliers.ToList();
            return View(suppliers);
        }

    }
}
