using AlHersey.Models;
using Microsoft.AspNetCore.Mvc;

namespace AlHersey.ViewComponents
{
	public class Adress :ViewComponent
	{
		AlHerseyContext context = new AlHerseyContext();

		public string Invoke()
		{
			string adress = context.Settings.FirstOrDefault(s => s.SettingID == 1).Address;

			return $"{adress}";
		}
	}
}
