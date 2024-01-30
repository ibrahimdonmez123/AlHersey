using Microsoft.EntityFrameworkCore;

namespace AlHersey.Models
{
    public interface ISupplierRepository
    {
        Task<List<Supplier>> SupplierSelectAsync();
        Task<string> SupplierInsertAsync(Supplier supplier);
        Task<Supplier?> SupplierDetailsAsync(int? id);
        Task<bool> SupplierUpdateAsync(Supplier supplier);
        Task<bool> SupplierDeleteAsync(int? id);
    }

    public class Cls_Supplier : ISupplierRepository
    {
        private readonly AlHerseyContext context;
        public Cls_Supplier(AlHerseyContext _context)
        {
            context = _context;
        }

        public async Task<List<Supplier>> SupplierSelectAsync()
        {
            List<Supplier> suppliers = await context.Suppliers.ToListAsync();
            return suppliers;
        }

        public async Task<string> SupplierInsertAsync(Supplier supplier)
        {
            try
            {
                Supplier sup = await context.Suppliers.FirstOrDefaultAsync(c => c.BrandName.ToLower() == supplier.BrandName.ToLower());
                if (sup == null)
                {
                    context.Add(supplier);
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

        public async Task<Supplier?> SupplierDetailsAsync(int? id)
        {
            Supplier? supplier = await context.Suppliers.FindAsync(id);
            return supplier;
        }

        public async Task<bool> SupplierUpdateAsync(Supplier supplier)
        {
            try
            {
                context.Update(supplier);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SupplierDeleteAsync(int? id)
        {
            try
            {
                Supplier supplier = await context.Suppliers.FirstOrDefaultAsync(c => c.SupplierID == id);
                supplier.Active = false;
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
