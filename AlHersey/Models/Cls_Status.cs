using Microsoft.EntityFrameworkCore;

namespace AlHersey.Models
{
    public interface IStatusRepository
    {
        Task<List<Status>> StatusSelectAsync();
        Task<string> StatusInsertAsync(Status status);
        Task<Status?> StatusDetailsAsync(int? id);
        Task<bool> StatusUpdateAsync(Status status);
        Task<bool> StatusDeleteAsync(int? id);
    }

    public class Cls_Status : IStatusRepository
    {
        private readonly AlHerseyContext context;
        public Cls_Status(AlHerseyContext _context)
        {
            context = _context;
        }

        public async Task<List<Status>> StatusSelectAsync()
        {
            List<Status> statuses = await context.Statuses.ToListAsync();
            return statuses;
        }

        public async Task<string> StatusInsertAsync(Status status)
        {
            try
            {
                Status st = await context.Statuses.FirstOrDefaultAsync(s => s.StatusName.ToLower() == status.StatusName.ToLower());
                if (st == null)
                {
                    context.Add(status);
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

        public async Task<Status?> StatusDetailsAsync(int? id)
        {
            Status? status = await context.Statuses.FindAsync(id);
            return status;
        }

        public async Task<bool> StatusUpdateAsync(Status status)
        {
            try
            {
                context.Update(status);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> StatusDeleteAsync(int? id)
        {
            try
            {
                Status status = await context.Statuses.FirstOrDefaultAsync(c => c.StatusID == id);
                status.Active = false;
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
