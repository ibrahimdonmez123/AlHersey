using Microsoft.AspNetCore.Mvc;
using AlHersey.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using XAct;
using Microsoft.EntityFrameworkCore;
using XAct.State;

namespace AlHersey.Controllers
{
    public class AdminController : Controller
    {


        private readonly IUserRepository u;
        private readonly ICategoryRepository c;
        private readonly ISupplierRepository s;
        private readonly IStatusRepository st;
        private readonly IProductRepository p;
        private readonly ISettingRepository set;
        private readonly IOrderRepository o;
        private readonly IMessageRepository m;
        private readonly AlHerseyContext context;

        public AdminController(IUserRepository _u, ICategoryRepository _c, ISupplierRepository _s,
            IStatusRepository _st, IProductRepository _p, ISettingRepository _set, IOrderRepository _o,
            IMessageRepository _m, AlHerseyContext _context)
        {
            u = _u;
            c = _c;
            s = _s;
            st = _st;
            p = _p;
            set = _set;
            o = _o;
            m = _m;
            context = _context;
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Email,Password,NameSurname")] User user)
        {
            if (ModelState.IsValid)
            {
                User? usr = await u.LoginControlAsync(user);
                if (usr != null)
                {
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Email");
            HttpContext.Session.Remove("Admin");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CategoryIndex()
        {
            List<Category> categories = await c.CategorySelectAsync();
            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> CategoryCreate()
        {
            await CategoryFill();
            return View();
        }

        async Task CategoryFill()
        {
            List<Category> categories = await c.CategorySelectMainAsync();
            ViewData["categoryList"] = categories.Select(c => new SelectListItem
            {
                Text = c.CategoryName,
                Value = c.CategoryID.ToString()
            });
        }


        [HttpPost]
        public async Task<IActionResult> CategoryCreate(Category category)
        {
            string answer = await c.CategoryInsertAsync(category);

            if (answer == "başarılı")
            {
                TempData["Message"] = category.CategoryName + " Kategori Eklendi.";
            }
            else if (answer == "zaten var")
            {
                TempData["Message"] = "Bu Kategori Daha Önceden Eklenmiş.";
            }
            else
            {
                TempData["Message"] = "HATA!!! Kategori Eklenemedi.";
            }

            return RedirectToAction("CategoryCreate"); //[HttpGet]
        }

        public async Task<IActionResult> CategoryEdit(int? id)
        {
            await CategoryFill();
            if (id == null || context.Categories == null)
            {
                return NotFound();
            }

            var category = await c.CategoryDetailsAsync(id);

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> CategoryEdit(Category category)
        {
            bool answer = await c.CategoryUpdateAsync(category);

            if (answer)
            {
                TempData["Message"] = category.CategoryName + "Kategori Güncellendi";
                return RedirectToAction("CategoryIndex");
            }
            else
            {
                TempData["Message"] = "HATA!!! Kategori Güncellenemedi.";
                //return RedirectToAction("CategoryEdit");
                return RedirectToAction(nameof(CategoryEdit));
            }
        }


        public static int global_categoryid = 0;

        public async Task<IActionResult> CategoryDetails(int id)
        {
            if (id != 0)
            {
                global_categoryid = id;
            }
            if (id == 0)
            {
                id = global_categoryid;
            }
            var category = await c.CategoryDetailsAsync(id);

            ViewBag.categoryname = category?.CategoryName;

            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> CategoryDelete(int? id)
        {
            if (id == null || context.Categories == null)
            {
                return NotFound();
            }

            var category = await context.Categories.FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost, ActionName("CategoryDelete")]
        public async Task<IActionResult> CategoryDeleteConfirmend(int? id)
        {
            bool result = await c.CategoryDeleteAsync(id);

            if (result)
            {
                TempData["Message"] = "Kategori Silindi.";
                return RedirectToAction("CategoryIndex");
            }
            else
            {
                TempData["Message"] = "HATA!!! Kategori Silinemedi.";

                return RedirectToAction(nameof(CategoryDelete));
            }
        }







        public async Task<IActionResult> SupplierIndex()
        {
            List<Supplier> suppliers = await s.SupplierSelectAsync();
            return View(suppliers);
        }

        [HttpGet]
        public IActionResult SupplierCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SupplierCreate(Supplier supplier)
        {
            string answer = await s.SupplierInsertAsync(supplier);
            if (answer == "başarılı")
            {
                TempData["Message"] = supplier.BrandName + " Markası Eklendi.";
            }
            else if (answer == "zaten var")
            {
                TempData["Message"] = "Bu Marka Daha Önceden Eklenmiş.";
            }
            else
            {
                TempData["Message"] = "HATA!!! Marka Eklenemedi.";
            }
            return RedirectToAction("SupplierCreate"); //[HttpGet]
        }

        public async Task<IActionResult> SupplierEdit(int? id)
        {
            if (id == null || context.Suppliers == null)
            {
                return NotFound();
            }

            var supplier = await s.SupplierDetailsAsync(id);

            return View(supplier);
        }

        [HttpPost]
        public async Task<IActionResult> SupplierEdit(Supplier supplier)
        {
            if (supplier.PhotoPath == null)
            {
                string? PhotoPath = context.Suppliers.FirstOrDefault(s => s.SupplierID == supplier.SupplierID).PhotoPath;
                supplier.PhotoPath = PhotoPath;
            }

            bool answer = await s.SupplierUpdateAsync(supplier);

            if (answer)
            {
                TempData["Message"] = supplier.BrandName + "Marka Güncellendi";
                return RedirectToAction("SupplierIndex");
            }
            else
            {
                TempData["Message"] = "HATA!!! Marka Güncellenemedi.";
                return RedirectToAction(nameof(SupplierEdit));
            }
        }


        public static int global_supplierid = 0;

        public async Task<IActionResult> SupplierDetails(int id)
        {
            if (id != 0)
            {
                global_supplierid = id;
            }
            if (id == 0)
            {
                id = global_supplierid;
            }
            var supplier = await s.SupplierDetailsAsync(id);

            ViewBag.categoryname = supplier?.BrandName;

            return View(supplier);
        }

        [HttpGet]
        public async Task<IActionResult> SupplierDelete(int? id)
        {
            if (id == null || context.Suppliers == null)
            {
                return NotFound();
            }

            var supplier = await context.Suppliers.FirstOrDefaultAsync(c => c.SupplierID == id);

            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        [HttpPost, ActionName("SupplierDelete")]
        public async Task<IActionResult> SupplierDeleteConfirmend(int? id)
        {
            bool result = await s.SupplierDeleteAsync(id);

            if (result)
            {
                TempData["Message"] = "Marka Silindi.";
                return RedirectToAction("SupplierIndex");
            }
            else
            {
                TempData["Message"] = "HATA!!! Marka Silinemedi.";
                return RedirectToAction(nameof(SupplierDelete));
            }
        }






        public async Task<IActionResult> StatusIndex()
        {
            List<Status> statuses = await st.StatusSelectAsync();
            return View(statuses);
        }

        [HttpGet]
        public IActionResult StatusCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> StatusCreate(Status status)
        {
            string answer = await st.StatusInsertAsync(status);

            if (answer == "başarılı")
            {
                TempData["Message"] = status.StatusName + " Statüsü Eklendi.";
            }
            else if (answer == "zaten var")
            {
                TempData["Message"] = "Bu Statü Daha Önceden Eklenmiş.";
            }
            else
            {
                TempData["Message"] = "HATA!!! Statü Eklenemedi.";
            }

            return RedirectToAction("StatusCreate"); //[HttpGet]
        }

        public async Task<IActionResult> StatusEdit(int? id)
        {
            if (id == null || context.Statuses == null)
            {
                return NotFound();
            }

            var status = await st.StatusDetailsAsync(id);
            return View(status);
        }

        [HttpPost]
        public async Task<IActionResult> StatusEdit(Status status)
        {
            bool answer = await st.StatusUpdateAsync(status);

            if (answer)
            {
                TempData["Message"] = status.StatusName + " Statü Güncellendi";
                return RedirectToAction("StatusIndex");
            }
            else
            {
                TempData["Message"] = "HATA!!! Statü Güncellenemedi.";
                return RedirectToAction(nameof(StatusEdit));
            }
        }


        public static int global_statusid = 0;
        public async Task<IActionResult> StatusDetails(int id)
        {
            if (id != 0)
            {
                global_statusid = id;
            }
            if (id == 0)
            {
                id = global_statusid;
            }
            var status = await st.StatusDetailsAsync(id);

            ViewBag.statusname = status?.StatusName;

            return View(status);
        }

        [HttpGet]
        public async Task<IActionResult> StatusDelete(int? id)
        {
            if (id == null || context.Statuses == null)
            {
                return NotFound();
            }

            var status = await context.Statuses.FirstOrDefaultAsync(c => c.StatusID == id);

            if (status == null)
            {
                return NotFound();
            }

            return View(status);
        }

        [HttpPost, ActionName("StatusDelete")]
        public async Task<IActionResult> StatusDeleteConfirmend(int? id)
        {
            bool result = await st.StatusDeleteAsync(id);

            if (result)
            {
                TempData["Message"] = "Statü Silindi.";
                return RedirectToAction("StatusIndex");
            }
            else
            {
                TempData["Message"] = "HATA!!! Statü Silinemedi.";
                return RedirectToAction(nameof(StatusDelete));
            }
        }

        public async Task<IActionResult> ProductIndex()
        {
            List<Product> products = await p.ProductSelectAsync();
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> ProductCreate()
        {
            List<Category> categories = await c.CategorySelectAsync();
            ViewData["categoryList"] = categories.Select(c => new SelectListItem { Text = c.CategoryName, Value = c.CategoryID.ToString() });

            List<Supplier> suppliers = await s.SupplierSelectAsync();
            ViewData["supplierList"] = suppliers.Select(s => new SelectListItem { Text = s.BrandName, Value = s.SupplierID.ToString() });

            List<Status> statuses = await st.StatusSelectAsync();
            ViewData["StatusList"] = statuses.Select(s => new SelectListItem { Text = s.StatusName, Value = s.StatusID.ToString() });

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProductCreate(Product product)
        {
            string answer = await p.ProductInsertAsync(product);

            if (answer == "başarılı")
            {
                TempData["Message"] = product.ProductName + " Ürünü Eklendi";
            }
            else if (answer == "Bu ürün Zaten Var!!")
            {
                TempData["Message"] = product.ProductName + " Ürünü Daha Önceden Eklenmiştir!.";
            }
            else
            {
                TempData["Message"] = product.ProductName + " Ürünü Eklenemedi.";
            }
            return RedirectToAction("ProductCreate"); //[HttpGet]
        }

        public async Task<IActionResult> ProductEdit(int? id)
        {
            await CategoryFill();
            if (id == null || context.Categories == null)
            {
                return NotFound();
            }

            await SupplierFill();
            if (id == null || context.Suppliers == null)
            {
                return NotFound();
            }

            await StatusFill();
            if (id == null || context.Statuses == null)
            {
                return NotFound();
            }
            var product = await p.ProductDetailsAsync(id);

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> ProductEdit(Product product)
        {
            if (product.PhotoPath == null)
            {
                string? PhotoPath = context.Products.FirstOrDefault(p => p.ProductID == product.ProductID).PhotoPath;
                product.PhotoPath = PhotoPath;

            }
            bool answer = await p.ProductUpdateAsync(product);

            if (answer)
            {
                TempData["Message"] = product.ProductName + " Ürünü Güncellendi";
                return RedirectToAction("ProductIndex");
            }
            else
            {
                TempData["Message"] = "HATA!!! Ürün Güncellenemedi.";

                return RedirectToAction(nameof(ProductEdit));
            }
        }

        public static int global_productid = 0;
        public async Task<IActionResult> ProductDetails(int id)
        {
            if (id != 0)
            {
                global_productid = id;
            }
            if (id == 0)
            {
                id = global_productid;
            }
            var product = await p.ProductDetailsAsync(id);

            ViewBag.Productname = product?.ProductName;

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> ProductDelete(int? id)
        {
            if (id == null || context.Products == null)
            {
                return NotFound();
            }

            var product = await context.Products.FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("ProductDelete")]
        public async Task<IActionResult> ProductDeleteConfirmend(int? id)
        {
            bool result = await p.ProductDeleteAsync(id);

            if (result)
            {
                TempData["Message"] = "Ürün Silindi.";
                return RedirectToAction("ProductDelete");
            }
            else
            {
                TempData["Message"] = "HATA!!! Ürün Silinemedi.";

                return RedirectToAction(nameof(ProductDelete));
            }
        }

        async Task SupplierFill()
        {
            List<Supplier> suppliers = await s.SupplierSelectAsync();
            ViewData["supplierList"] = suppliers.Select(s => new SelectListItem { Text = s.BrandName, Value = s.SupplierID.ToString() });

        }

        async Task StatusFill()
        {
            List<Status> statuses = await st.StatusSelectAsync();
            ViewData["statusList"] = statuses.Select(st => new SelectListItem { Text = st.StatusName, Value = st.StatusID.ToString() });

        }

        async Task CategoryFillAll()
        {
            List<Category> categories = await c.CategorySelectAsync();
            ViewData["categoryList"] = categories.Select(c => new SelectListItem { Text = c.CategoryName, Value = c.CategoryID.ToString() });

        }

        public async Task<IActionResult> SettingIndex()
        {
            List<Setting> settings = await set.SettingSelectAsync();
            return View(settings);
        }




        public async Task<IActionResult> SettingEdit(int? id)
        {
            if (id == null || context.Settings == null)
            {
                return NotFound();
            }

            var setting = await set.SettingDetailsAsync(id);

            return View(setting);
        }

        [HttpPost]
        public async Task<IActionResult> SettingEdit(Setting setting)
        {
            bool answer = await set.SettingUpdateAsync(setting);

            if (answer)
            {
                return RedirectToAction("SettingIndex");
            }
            else
            {
                return RedirectToAction(nameof(SettingEdit));
            }
        }

        public async Task<IActionResult> UserIndex()
        {
            List<User> users = await u.UserSelectAsync();
            return View(users);
        }

        public static int global_userid = 0;
        public async Task<IActionResult> UserDetails(int id)
        {
            if (id != 0)
            {
                global_userid = id;
            }
            if (id == 0)
            {
                id = global_userid;
            }
            var user = await u.UserDetailsAsync(id);

            return View(user);
        }

        public async Task<IActionResult> OrderIndex()
        {
            List<Order> orders = await o.OrderSelectAsync();
            return View(orders);
        }






        public static int global_orderid = 0;
        public async Task<IActionResult> OrderDetails(int id)
        {
            if (id != 0)
            {
                global_orderid = id;
            }
            if (id == 0)
            {
                id = global_orderid;
            }
            var order = await o.OrderDetailsAsync(id);

            return View(order);
        }

        public async Task<IActionResult> MessageIndex()
        {
            List<Message> messages = await m.MessageSelectAsync();
            return View(messages);
        }

        public static int global_messageid = 0;
        public async Task<IActionResult> MessageDetails(int id)
        {
            if (id != 0)
            {
                global_messageid = id;
            }
            if (id == 0)
            {
                id = global_messageid;
            }
            var message = await m.MessageDetailsAsync(id);

            return View(message);
        }
    }
}

