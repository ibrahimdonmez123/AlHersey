using Microsoft.AspNetCore.Mvc;
using AlHersey.Models;
using XAct;
using PagedList.Core;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Specialized;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using XAct.Domain.Repositories;
using XAct.State;
using System.Net;
using System.Net.Mail;

namespace AlHersey.Controllers
{

    public class HomeController : Controller
    {
        

        private readonly MainPageModel mpm;
        private readonly AlHerseyContext context;
        private readonly IProductRepository p;
        private readonly IOrderRepository cls_order;
        private readonly IUserRepository u;

        public HomeController(IProductRepository _p , IOrderRepository _cls_order , IUserRepository _u , MainPageModel _mpm , AlHerseyContext _context)
        {
            p = _p;
            cls_order = _cls_order;
            u = _u;
            mpm = _mpm;
            context = _context;
        }

        public async Task<IActionResult> Index()
        {
            mpm.SliderProducts = await p.ProductSelectAsync("Slider", "", 0);
            mpm.NewProducts = await p.ProductSelectAsync("New", "", 0);
            mpm.Productofday =  p.ProductDetails();
            mpm.SpecialProducts = await p.ProductSelectAsync("Special", "", 0);
            mpm.DiscountedProducts = await p.ProductSelectAsync("Discounted", "", 0);
            mpm.HighLightedProducts = await p.ProductSelectAsync("HighLighted", "", 0);
            mpm.TopsellerProducts = await p.ProductSelectAsync("TopSeller", "", 0);
            mpm.StarProducts = await p.ProductSelectAsync("Star", "", 0);
            mpm.FeaturedProducts = await p.ProductSelectAsync("Featured", "", 0);
            mpm.NotableProducts = await p.ProductSelectAsync("Notable", "", 0);

            return View(mpm);
        }

        public async Task<IActionResult> CartProcess(int id)
        {
            await p.Highlighted_IncreaseAsync(id);

            cls_order.ProductID = id;
            cls_order.Quantity = 1;

            var cookieOptions = new CookieOptions();
            var cookie = Request.Cookies["sepetim"];
            if (cookie == null)
            {
                cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTime.Now.AddDays(7);
                cookieOptions.Path = "/";
                cls_order.MyCart = "";
                cls_order.AddToMyCartAsync(id.ToString());
                Response.Cookies.Append("sepetim", cls_order.MyCart, cookieOptions);
                HttpContext.Session.SetString("Message", "Ürün Sepetinize Eklendi");
                TempData["Message"] = "Ürün Sepetinize Eklendi";
            }
            else
            {
                cls_order.MyCart = cookie;

                if (await cls_order.AddToMyCartAsync(id.ToString()) == false)
                {
                    Response.Cookies.Append("sepetim", cls_order.MyCart, cookieOptions);
                    cookieOptions.Expires = DateTime.Now.AddDays(7);
                    HttpContext.Session.SetString("Message", "Ürün Sepetinize Eklendi");
                    TempData["Message"] = "Ürün Sepetinize Eklendi";
                }
                else
                {
                    HttpContext.Session.SetString("Message", "Ürün Sepetinizde Zaten Var");
                    TempData["Message"] = "Ürün Sepetinizde Zaten Var";
                }

            }

            string url = Request.Headers["Referer"].ToString();
            return Redirect(url);
        }




        public async Task<IActionResult> Cart()
        {
            List<Cls_Order> MyCart;

            if (HttpContext.Request.Query["scid"].ToString() != "")
            {
                int scid = Convert.ToInt32(HttpContext.Request.Query["scid"].ToString());
                cls_order.MyCart = Request.Cookies["sepetim"];
                cls_order.DeleteFromMyCartAsync(scid.ToString());

                var cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTime.Now.AddDays(7);
                Response.Cookies.Append("sepetim", cls_order.MyCart, cookieOptions);
                TempData["Message"] = "Ürün Sepetinizden Silindi";
                return RedirectToAction("Cart");

            }

            var cookie = Request.Cookies["sepetim"];

            if (cookie == null)
            {
                cls_order.MyCart = "";
                MyCart = await cls_order.SelectMyCartAsync();
            }
            else
            {
                cls_order.MyCart = cookie;
                MyCart = await cls_order.SelectMyCartAsync();
            }

            if (MyCart == null || MyCart.Count == 0)
            {
                ViewBag.MyCart = null;
            }
            else
            {
                ViewBag.MyCart = MyCart;
                ViewBag.MyCart_Table_Details = MyCart;
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            cls_order.MyCart = Request.Cookies["sepetim"];
            await cls_order.UpdateQuantityInMyCartAsync(productId.ToString(), quantity.ToString());

            var cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Now.AddDays(7);

            Response.Cookies.Append("sepetim", cls_order.MyCart, cookieOptions);

            return RedirectToAction("Cart");
        }


        //public IActionResult Cart()
        //{
        //    List<Cls_Order> MyCart;

        //    if (HttpContext.Request.Query["scid"].ToString() != "")
        //    {
        //        int scid = Convert.ToInt32(HttpContext.Request.Query["scid"].ToString());
        //        cls_order.MyCart = Request.Cookies["sepetim"];
        //        cls_order.DeleteFromMyCart(scid.ToString());

        //        var cookieOptions = new CookieOptions();
        //        Response.Cookies.Append("sepetim", cls_order.MyCart, cookieOptions);
        //        cookieOptions.Expires = DateTime.Now.AddDays(7);
        //        TempData["Message"] = "Ürün Sepetinizden Silindi";
        //        MyCart = cls_order.SelectMyCart();
        //        ViewBag.MyCart = MyCart;
        //        ViewBag.MyCart_Table_Details = MyCart;
        //    }
        //    else
        //    {
        //        var cookie = Request.Cookies["sepetim"];


        //        if (cookie == null)
        //        {
        //            //SEPETTE HİÇ ÜRÜN OLMAYABİLİR
        //            var cookieOptions = new CookieOptions();
        //            cls_order.MyCart = "";
        //            MyCart = cls_order.SelectMyCart();
        //            ViewBag.MyCart = MyCart;
        //            ViewBag.MyCart_Table_Details = MyCart;

        //        }
        //        else
        //        {
        //            //SEPETTE ÜRÜN VAR
        //            var cookieOptions = new CookieOptions();
        //            cls_order.MyCart = Request.Cookies["sepetim"];
        //            MyCart = cls_order.SelectMyCart();
        //            ViewBag.MyCart = MyCart;
        //            ViewBag.MyCart_Table_Details = MyCart;

        //        }

        //    }

        //    if (MyCart.Count == 0)
        //    {
        //        ViewBag.MyCart = null;
        //    }

        //    return View();
        //}



        public async Task<IActionResult> Details(int id)
        {
            mpm.ProductDetails = await context.Products.FirstOrDefaultAsync(p => p.ProductID == id);

            mpm.CategoryName = await (from p in context.Products
                                      join c in context.Categories on p.CategoryID equals c.CategoryID
                                      where p.ProductID == id
                                      select c.CategoryName).FirstOrDefaultAsync();

            mpm.BrandName = await (from p in context.Products
                                   join s in context.Suppliers on p.SupplierID equals s.SupplierID
                                   where p.ProductID == id
                                   select s.BrandName).FirstOrDefaultAsync();

            mpm.RelatedProducts = await context.Products
                .Where(p => p.Related == mpm.ProductDetails!.Related && p.ProductID != id)
                .ToListAsync();

            p.Highlighted_IncreaseAsync(id);

            return View(mpm);
        }

        public async Task<PartialViewResult> gettingProducts(string id)
        {
            id = id.ToUpper(new System.Globalization.CultureInfo("tr-TR"));
            List<sp_arama> ulist = await p.GettingSearchProductsAsync(id);

            string json = JsonConvert.SerializeObject(ulist);
            var response = JsonConvert.DeserializeObject<List<Search>>(json);
            return PartialView(response);
        }

        public async Task<IActionResult> Order()
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                User? user = await u.SelectMemberInfoAsync(HttpContext.Session.GetString("Email").ToString());

                return View(user);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }




        [HttpPost]
        public async Task<IActionResult> Order(IFormCollection frm)
        {
            await Confirm_OrderAsync();

            if (TempData.ContainsKey("Message"))
            {
                ViewBag.Message = TempData["Message"].ToString();
            }

            return RedirectToAction("MyOrders");
        }




        public static string OrderGroupGUID = "";

        public async Task Confirm_OrderAsync()
        {
            var cookieOptions = new CookieOptions();
            var cookie = Request.Cookies["sepetim"];

            if (cookie != null)
            {
                cls_order.MyCart = cookie;

                OrderGroupGUID = await cls_order.WriteToOrderTableAsync(HttpContext.Session.GetString("Email"));

                cookieOptions.Expires = DateTime.Now.AddDays(7);
                Response.Cookies.Delete("sepetim");
                TempData["Message"] = "Proje gerçekten kullanılırken ödeme alımı gerçekleşir ardından müşteriye sms ve mail atılır ! Bu aşamada daha banka hesabı açılmadığı için tüm ödemeler gerçekten başarılıymış gibi görülerek kullanıcı siparişlerim sayfasına yönlendirileccektir ! ";
            }
        }


        //[HttpPost]
        //public IActionResult Order(IFormCollection frm)
        //{
        //    string kredikartno = frm["kredikartno"].ToString(); 
        //    string kredikartay = frm["kredikartay"].ToString();
        //    string kredikartyıl = frm["kredikartyıl"].ToString();
        //    string kredikartcvc = frm["kredikartcvc"].ToString();

        //    //bankaya git eger true gelirse(onay gelirse)
        //    //order tablosuna kayıt atacağız
        //    // digital-planet e(e-fatura ) bilgilerini gönder

        //    //payu
        //    //iyzico

        //    string txt_tckimlikno = frm["txt_tckimlikno"].ToString();
        //    string txt_vergino = frm["txt_vergino"].ToString();

        //    if (txt_tckimlikno != "")
        //    {
        //        WebServiceController.tckimlikno = txt_tckimlikno;
        //        //fatura bilgilerini digital planet şirketine vb gönderirsiniz(xml dosyası)
        //    }
        //    else
        //    {
        //        WebServiceController.vergino = txt_vergino;
        //    }

        //    NameValueCollection data = new NameValueCollection();
        //    string url = "https://www.inciturpcan.com/backref";

        //    data.Add("BACK_REF", url);
        //    data.Add("CC_CVC", kredikartcvc);
        //    data.Add("CC_NUMBER", kredikartno);
        //    data.Add("EXP_MONTH", kredikartay);
        //    data.Add("EXP_YEAR", kredikartyıl);

        //    var deger = "";
        //    foreach (var item in data)
        //    {
        //        var value = item as string;
        //        var byteCount = Encoding.UTF8.GetByteCount(data.Get(value));
        //        deger += byteCount + data.Get(value);
        //    }

        //    var signatureKey = "payu üyeliğinde size verilen SECRET_KEY burada yazılacak";
        //    var hash = HashWithSignature(deger, signatureKey);


        //    data.Add("ORDER_HASH", hash);

        //    var x = POSTFormPAYU("https://secure.payu.tr/order/...", data);

        //    //sanal kredi kartı
        //    if (x.Contains("<STATUS>SUCCESS</STATUS>") && x.Contains("<RETURN_CODE>3DS_ENROLLED</RETURN_CODE>"))
        //    {
        //        // sanal kart ok
        //    }
        //    else
        //    {
        //        //gerçek kredi kartı
        //    }

        //    return RedirectToAction("backref");
        //}





        public async Task<IActionResult> MyOrders()
        {
            if (HttpContext.Session.GetString("Email") != null)
            {
                List<Order> orders = await cls_order.SelectMyOrdersAsync(HttpContext.Session.GetString("Email").ToString());

                List<vw_MyOrders> vw_MyOrders = orders.Select(async order => new vw_MyOrders
                {
                    OrderID = order.OrderID,
                    ProductID = order.ProductID,
                    OrderDate = order.OrderDate,
                    OrderGroupGUID = order.OrderGroupGUID,
                    Qantity = order.Qantity,
                    UserID = order.UserID,
                    ProductName = await context.Products
                        .Where(p => p.ProductID == order.ProductID)
                        .Select(p => p.ProductName)
                        .FirstOrDefaultAsync(),
                    UnitPrice = await context.Products
                        .Where(p => p.ProductID == order.ProductID)
                        .Select(p => p.UnitPrice)
                        .FirstOrDefaultAsync(),
                    PhotoPath = await context.Products
                        .Where(p => p.ProductID == order.ProductID)
                        .Select(p => p.PhotoPath)
                        .FirstOrDefaultAsync()
                }).Select(t => t.Result).ToList();

                return View(vw_MyOrders);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public async Task<IActionResult> DetailedSearch()
        {
            ViewBag.Categories = await context.Categories.ToListAsync();
            ViewBag.Suppliers = await context.Suppliers.ToListAsync();
            return View();
        }

        public async Task<IActionResult> DProducts(int CategoryID, string[] SupplierID, string price, string IsInStock)
        {
            price = price.Replace(" ", "");
            string[] PriceArray = price.Split('-');
            string startprice = PriceArray[0];
            string endprice = PriceArray[1];

            string sign = ">";
            if (IsInStock == "0")
            {
                sign = ">=";
            }

            int count = 0;
            string suppliervalue = "";
            for (int i = 0; i < SupplierID.Length; i++)
            {
                if (count == 0)
                {
                    suppliervalue = "SupplierID =" + SupplierID[i];
                    count++;
                }
                else
                {
                    suppliervalue += " or SupplierID =" + SupplierID[i];
                }
            }

            string query = "select * from Products where  CategoryID = " + CategoryID + " and (" + suppliervalue + ") and (UnitPrice > " + startprice + " and UnitPrice < " + endprice + ") and Stock " + sign + " 0 order by ProductName";

            ViewBag.Products = await p.SelectProductsByDetailsAsync(query);
            return View();
        }


        //public static string HashWithSignature(string deger, string signatureKey)
        //{
        //    return "";
        //}

        //public IActionResult backref()
        //{
        //    Confirm_Order();
        //    return RedirectToAction("Confirm");
        //}
        //*/



        //public static string OrderGroupGUID = "";

        //public IActionResult Confirm_Order()
        //{



        //    var cookieOptions = new CookieOptions();
        //    var cookie = Request.Cookies["sepetim"];
        //    if (cookie != null)
        //    {

        //        cls_order.MyCart = cookie; 

        //        OrderGroupGUID = cls_order.WriteToOrderTable(HttpContext.Session.GetString("Email"));
        //        cookieOptions.Expires = DateTime.Now.AddDays(7);
        //        Response.Cookies.Delete("sepetim");

        //        bool result = Cls_User.SendSms(OrderGroupGUID);

        //        if (result == false)
        //        {
        //        }
        //    }
        //    return RedirectToAction("Confirm");

        //}

        //[HttpGet]
        //public IActionResult Confirm()
        //{
        //    ViewBag.OrderGroupGUID = OrderGroupGUID;
        //    return View();
        //}

        //public static string POSTFormPAYU(string url, NameValueCollection data)
        //{
        //    return "";
        //}








        public async Task<IActionResult> Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            string answer = await u.MemberControlAsync(user);

            if (answer == "error")
            {
                TempData["Message"] = "Email ve/veya Şifre yanlış";
            }
            else if (answer == "admin")
            {
                HttpContext.Session.SetString("Admin", "Admin");
                HttpContext.Session.SetString("Email", answer);
                return RedirectToAction("Login", "Admin");
            }
            else
            {
                HttpContext.Session.SetString("Email", answer);
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("Email");
            HttpContext.Session.Remove("Admin");
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (await u.LoginEmailControlAsync(user) == false)
            {
                bool answer = await u.AddUserAsync(user);

                if (answer)
                {
                    TempData["Message"] = "Kaydedildi.";
                    return RedirectToAction("Login");
                }

                TempData["Message"] = "Hata.Tekrar deneyiniz.";
            }
            else
            {
                TempData["Message"] = "Bu Email Zaten mevcut.Başka Deneyiniz.";
            }

            return View();
        }









        public async Task<IActionResult> NewProducts()
        {
            mpm.NewProducts = await p.ProductSelectAsync("New", "New", 0);
            return View(mpm);
        }

        public async Task<PartialViewResult> _partialNewProducts(string nextpagenumber)
        {
            int pagenumber = Convert.ToInt32(nextpagenumber);
            mpm.NewProducts = await p.ProductSelectAsync("New", "New", pagenumber);
            return PartialView(mpm);
        }

        public async Task<IActionResult> SpecialProducts()
        {
            mpm.SpecialProducts = await p.ProductSelectAsync("Special", "Special", 0);
            return View(mpm);
        }

        public async Task<PartialViewResult> _partialSpecialProducts(string nextpagenumber)
        {
            int pagenumber = Convert.ToInt32(nextpagenumber);
            mpm.SpecialProducts = await p.ProductSelectAsync("Special", "Special", pagenumber);
            return PartialView(mpm);
        }

        public async Task<IActionResult> DiscountedProducts()
        {
            mpm.DiscountedProducts = await p.ProductSelectAsync("Discounted", "Discounted", 0);
            return View(mpm);
        }

        public async Task<PartialViewResult> _partialDiscountedProducts(string nextpagenumber)
        {
            int pagenumber = Convert.ToInt32(nextpagenumber);
            mpm.DiscountedProducts = await p.ProductSelectAsync("Discounted", "Discounted", pagenumber);
            return PartialView(mpm);
        }

        public async Task<IActionResult> HighLightedProducts()
        {
            mpm.HighLightedProducts = await p.ProductSelectAsync("HighLighted", "HighLighted", 0);
            return View(mpm);
        }

        public async Task<PartialViewResult> _partialHighlighted(string nextpagenumber)
        {
            int pagenumber = Convert.ToInt32(nextpagenumber);
            mpm.HighLightedProducts = await p.ProductSelectAsync("HighLighted", "HighLighted", pagenumber);
            return PartialView(mpm);
        }



        //public IActionResult TopsellerProducts(int page = 1, int pagesize = 4)
        //{
        //    PagedList<Product> model = new PagedList<Product>(context.Products.OrderByDescending(p => p.TopSeller), page, pagesize);
        //    return View("TopsellerProducts", model);

        //}


        public async Task<IActionResult> TopsellerProducts()
        {
            mpm.TopsellerProducts = await p.ProductSelectAsync("Topseller", "Topseller", 0);
            return View(mpm);
        }

        public async Task<PartialViewResult> _partialTopSellerProducts(string nextpagenumber)
        {
            int pagenumber = Convert.ToInt32(nextpagenumber);
            mpm.TopsellerProducts = await p.ProductSelectAsync("Topseller", "Topseller", pagenumber);
            return PartialView(mpm);
        }

        public async Task<IActionResult> CategoryPage(int id)
        {
            List<Product> products = await p.ProductSelectWithCategoryIDAsync(id);
            return View(products);
        }

        public async Task<IActionResult> SupplierPage(int id)
        {
            List<Product> products = await p.ProductSelectWithSupplierIDAsync(id);
            return View(products);
        }

        public async Task<IActionResult> ContactUs()
        {
            return View();
        }

        public async Task<IActionResult> AboutUs()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> CancelOrder(string OrderGroupGUID)
        {
            Order order = await context.Orders.FirstOrDefaultAsync(c => c.OrderGroupGUID == OrderGroupGUID);

            if (order != null)
            {
                context.Remove(order);
                await context.SaveChangesAsync();

                ViewBag.Message = "Siparişiniz başarıyla iptal edilmiştir, 3 gün içerisinde ekibimiz size ulaşıp iadeyi alıp geri ödemenizi gerçekleştirecektir";

                return RedirectToAction("MyOrders");
            }
            else
            {
                // Order bulunamadıysa ilgili mesajı göster.
                ViewBag.Message = "Sipariş bulunamadı.";
                return RedirectToAction("MyOrders");
            }
        }

        public async Task<IActionResult> PharmacyOnDuty()
        {
            string json = await new WebClient().DownloadStringTaskAsync("https://openapi.izmir.bel.tr/api/ibb/nobetcieczaneler");

            var pharmacy = JsonConvert.DeserializeObject<List<Pharmacy>>(json);

            return View(pharmacy);
        }

        public async Task<IActionResult> ArtAndCulture()
        {
            string json = await new WebClient().DownloadStringTaskAsync("https://openapi.izmir.bel.tr/api/ibb/kultursanat/etkinlikler");

            var activite = JsonConvert.DeserializeObject<List<Activite>>(json);

            return View(activite);
        }

        public IActionResult Exchange()
        {
            // Eğer bu metotta asenkron bir işlem yoksa, sadece Task.FromResult kullanarak geri dönüş yapabilirsiniz.
            return View();
        }

        public IActionResult Degree()
        {
            // Eğer bu metotta asenkron bir işlem yoksa, sadece Task.FromResult kullanarak geri dönüş yapabilirsiniz.
            return View();
        }

    }
}












