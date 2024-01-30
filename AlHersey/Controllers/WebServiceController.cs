using Microsoft.AspNetCore.Mvc;

namespace AlHersey.Controllers
{
    // ÖDEME İŞLEMİNE YARDIMCI OLACAK
    public class WebServiceController : Controller
    {
        public static string tckimlikno = "";
        public static string vergino = "";
        public IActionResult Index()
        {
            return View();
        }
    }
}
