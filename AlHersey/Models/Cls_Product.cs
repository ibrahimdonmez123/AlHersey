using AlHersey.Migrations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using XAct;

namespace AlHersey.Models
{
    public interface IProductRepository
    {
        int ProductID { get; set; }
        string? ProductName { get; set; }
        decimal UnitPrice { get; set; }
        string? PhotoPath { get; set; }

        Task<List<Product>> ProductSelectAsync();
        Task<string> ProductInsertAsync(Product product);
        Task<Product?> ProductDetailsAsync(int? id);
        Task<bool> ProductUpdateAsync(Product product);
        Task<bool> ProductDeleteAsync(int? id);
        Task<List<Product>> ProductSelectAsync(string mainPageName, string subPageName, int pageNumber);
        Product ProductDetails();
        Task<List<Cls_Product>> SelectProductsByDetailsAsync(string query);
        Task Highlighted_IncreaseAsync(int id);
        Task<List<Product>> ProductSelectWithCategoryIDAsync(int id);
        Task<List<Product>> ProductSelectWithSupplierIDAsync(int id);
        Task<List<sp_arama>> GettingSearchProductsAsync(string id);
    }

    public class Cls_Product : IProductRepository
    {
        private readonly AlHerseyContext context;

        public Cls_Product(AlHerseyContext _context)
        {
            context = _context;
        }

        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public string? PhotoPath { get; set; }

        public async Task<List<Product>> ProductSelectAsync()
        {
            List<Product> products = await context.Products.ToListAsync();
            return products;
        }

        public async Task<string> ProductInsertAsync(Product product)
        {
            using (AlHerseyContext context = new AlHerseyContext())
            {
                try
                {
                    Product pro = context.Products.FirstOrDefault(c => c.ProductName.ToLower() == product.ProductName.ToLower());

                    if (pro == null)
                    {
                        product.AddDate = DateTime.Now;
                        context.Add(product);
                        await context.SaveChangesAsync();
                        return "başarılı";
                    }
                    else
                    {
                        return "zaten var";
                    }
                }
                catch (Exception)
                {
                    return "başarısız";
                }
            }
        }



        public async Task<Product?> ProductDetailsAsync(int? id)
        {
            Product? product = await context.Products.FindAsync(id);
            return product;
        }

        public async Task<bool> ProductUpdateAsync(Product product)
        {
            using (AlHerseyContext context = new AlHerseyContext())
            {
                try
                {
                    context.Update(product);
                    await context.SaveChangesAsync();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public async Task<bool> ProductDeleteAsync(int? id)
        {
            try
            {
                using (AlHerseyContext context = new AlHerseyContext())
                {
                    Product product = await context.Products.FirstOrDefaultAsync(p => p.ProductID == id);
                    product.Active = false;
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Product>> ProductSelectAsync(string mainPageName, string subPageName, int pageNumber)
        {
            List<Product> products;

            if (mainPageName == "Slider")
            {
                products = await context.Products.Where(p => p.StatusID == 1).Take(4).ToListAsync();
            }
            else if (mainPageName == "New")
            {
                if (subPageName == "")
                {
                    products = await context.Products.OrderByDescending(p => p.AddDate).Take(8).ToListAsync();
                }
                else
                {
                    if (pageNumber == 0)
                    {
                        products = await context.Products.OrderByDescending(p => p.AddDate).Take(4).ToListAsync();
                    }
                    else
                    {
                        products = await context.Products.OrderByDescending(p => p.AddDate).Skip(4 * pageNumber).Take(4).ToListAsync();
                    }
                }
            }
            else if (mainPageName == "Special")
            {
                if (subPageName == "")
                {
                    products = await context.Products.Where(p => p.StatusID == 3).Take(8).ToListAsync();
                }
                else
                {
                    if (pageNumber == 0)
                    {
                        products = await context.Products.Where(p => p.StatusID == 3).Take(4).ToListAsync();
                    }
                    else
                    {
                        products = await context.Products.Where(p => p.StatusID == 3).Skip(4 * pageNumber).Take(4).ToListAsync();
                    }
                }
            }
            else if (mainPageName == "Discounted")
            {
                if (subPageName == "")
                {
                    products = await context.Products.OrderByDescending(p => p.Discount).Take(8).ToListAsync();
                }
                else
                {
                    if (pageNumber == 0)
                    {
                        products = await context.Products.OrderByDescending(p => p.Discount).Take(4).ToListAsync();
                    }
                    else
                    {
                        products = await context.Products.OrderByDescending(p => p.Discount).Skip(4 * pageNumber).Take(4).ToListAsync();
                    }
                }
            }
            else if (mainPageName == "HighLighted")
            {
                if (subPageName == "")
                {
                    products = await context.Products.OrderByDescending(p => p.HighLighted).Take(8).ToListAsync();
                }
                else
                {
                    if (pageNumber == 0)
                    {
                        products = await context.Products.OrderByDescending(p => p.HighLighted).Take(4).ToListAsync();
                    }
                    else
                    {
                        products = await context.Products.OrderByDescending(p => p.HighLighted).Skip(4 * pageNumber).Take(4).ToListAsync();
                    }
                }
            }
            else if (mainPageName == "TopSeller")
            {
                products = await context.Products.OrderByDescending(p => p.TopSeller).Take(8).ToListAsync();
            }
            else if (mainPageName == "Star")
            {
                products = await context.Products.Where(p => p.StatusID == 4).OrderBy(p => p.ProductID).Take(8).ToListAsync();
            }
            else if (mainPageName == "Featured")
            {
                products = await context.Products.Where(p => p.StatusID == 5).OrderBy(p => p.ProductID).Take(8).ToListAsync();
            }
            else
            {
                products = await context.Products.Where(p => p.StatusID == 6).OrderBy(p => p.ProductID).Take(8).ToListAsync();
            }

            return products;
        }

        public Product ProductDetails()
        {
            Product product = context.Products.FirstOrDefault(p => p.StatusID == 2);
            return product;
        }

        public async Task<List<Cls_Product>> SelectProductsByDetailsAsync(string query)
        {
            List<Cls_Product> products = new List<Cls_Product>();
            SqlConnection sqlConnection = Connection.ServerConnect;
            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
            sqlConnection.Open();
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            while (sqlDataReader.Read())
            {
                Cls_Product product = new Cls_Product(context);
                product.ProductID = Convert.ToInt32(sqlDataReader["ProductID"]);
                product.ProductName = sqlDataReader["ProductName"].ToString();
                product.UnitPrice = Convert.ToDecimal(sqlDataReader["UnitPrice"]);
                product.PhotoPath = sqlDataReader["PhotoPath"].ToString();
                products.Add(product);
            }
            return products;
        }

        public async Task Highlighted_IncreaseAsync(int id)
        {
            using (AlHerseyContext context = new AlHerseyContext())
            {
                Product? product = await context.Products.FirstOrDefaultAsync(p => p.ProductID == id);

                product.HighLighted += 1;
                await context.SaveChangesAsync();
                context.Update(product);
            }
        }

        public async Task<List<Product>> ProductSelectWithCategoryIDAsync(int id)
        {
            List<Product> products = await context.Products.Where(p => p.CategoryID == id).OrderBy(p => p.ProductName).ToListAsync();
            return products;
        }

        public async Task<List<Product>> ProductSelectWithSupplierIDAsync(int id)
        {
            List<Product> products = await context.Products.Where(p => p.SupplierID == id).OrderBy(p => p.ProductName).ToListAsync();
            return products;
        }

        public async Task<List<sp_arama>> GettingSearchProductsAsync(string id)
        {
            using (AlHerseyContext context = new AlHerseyContext())
            {
                var products = await context.sp_Aramas.FromSqlRaw($"sp_aramam {id}").ToListAsync();
                return products;
            }
        }
    }
}




//create procedure sp_arama
//(
//@id nvarchar(50)
//)
//as 
//select ID, KATEGORI, URUN, MARKA, ARAMAISMI from
//(select CategoryID as ID, CategoryID as KATEGORI,0 as URUN , 0 as MARKA,
//CategoryName as ARAMAISMI
//from Categories (NOLOCK) where ISNULL(CategoryID,0) <> 0

//UNION ALL
//select SupplierID as ID,0 as KATEORI,0 as URUN , SupplierID as MARKA,
//BrandName as ARAMAISMI
//from Suppliers(NOLOCK) where ISNULL(SupplierID,0) <> 0

//UNION ALL
//select ProductID as ID,0 as KATEGORI,ProductID as URUN , 0 as MARKA,
//ProductName as ARAMAISMI
//from Product(NOLOCK) where ISNULL(ProductID,0) <> 0
//) xyz where ARAMAISMI like '%'+@id+'%'

