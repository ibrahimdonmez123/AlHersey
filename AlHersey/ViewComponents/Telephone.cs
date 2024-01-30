using Microsoft.AspNetCore.Mvc;
using AlHersey.Models;


namespace AlHersey.ViewComponents
{
	public class Telephone : ViewComponent
	{
		AlHerseyContext context = new AlHerseyContext();

		public string Invoke()
		{
			string telephone = context.Settings.FirstOrDefault(s => s.SettingID == 1).Telephone;

			return $"{telephone}";

		}

	}
}
