using AlHersey.Models;
using Microsoft.AspNetCore.Mvc;

namespace AlHersey.ViewComponents
{
	public class Email : ViewComponent
	{
		AlHerseyContext context = new AlHerseyContext();
		public string Invoke()
		{
			string email = context.Settings.FirstOrDefault(s => s.SettingID == 1).Email;
			return $"{email}";


		}
	}
}
