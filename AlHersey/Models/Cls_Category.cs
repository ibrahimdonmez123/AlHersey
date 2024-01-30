using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AlHersey.Models
{
    public interface ICategoryRepository
    {
        Task<List<Category>> CategorySelectAsync();
        Task<List<Category>> CategorySelectMainAsync();
        Task<string> CategoryInsertAsync(Category category);
        Task<Category> CategoryDetailsAsync(int? id);
        Task<bool> CategoryUpdateAsync(Category category);
        Task<bool> CategoryDeleteAsync(int? id);
    }

    public class Cls_Category : ICategoryRepository
    {
        private readonly AlHerseyContext context;

        public Cls_Category(AlHerseyContext _context)
        {
            context = _context;
        }

        public async Task<List<Category>> CategorySelectAsync()
        {
            List<Category> categories = await context.Categories.ToListAsync();
            return categories;
        }

        public async Task<List<Category>> CategorySelectMainAsync()
        {
            List<Category> categories = await context.Categories.Where(c => c.ParentID == 0).ToListAsync();
            return categories;
        }

        public async Task<string> CategoryInsertAsync(Category category)
        {
            try
            {
                Category cat = await context.Categories.FirstOrDefaultAsync(c => c.CategoryName.ToLower() == category.CategoryName.ToLower());
                if (cat == null)
                {
                    context.Add(category);
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

        public async Task<Category> CategoryDetailsAsync(int? id)
        {
            Category category = await context.Categories.FindAsync(id);
            return category;
        }

        public async Task<bool> CategoryUpdateAsync(Category category)
        {
            using (AlHerseyContext context = new AlHerseyContext())
            {
                try
                {
                    context.Update(category);
                    await context.SaveChangesAsync();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public async Task<bool> CategoryDeleteAsync(int? id)
        {
            try
            {
                using (AlHerseyContext context = new AlHerseyContext())
                {
                    Category category = await context.Categories.FirstOrDefaultAsync(c => c.CategoryID == id);
                    category.Active = false;

                    List<Category> categories = await context.Categories.Where(c => c.ParentID == id).ToListAsync();

                    foreach (var item in categories)
                    {
                        item.Active = false;
                    }

                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
