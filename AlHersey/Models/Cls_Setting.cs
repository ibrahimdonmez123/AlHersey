using Microsoft.EntityFrameworkCore;

namespace AlHersey.Models
{
    public interface ISettingRepository
    {
        Task<List<Setting>> SettingSelectAsync();
        Task<Setting?> SettingDetailsAsync(int? id);
        Task<bool> SettingUpdateAsync(Setting setting);
    }

    public class Cls_Setting : ISettingRepository
    {
        private readonly AlHerseyContext context;

        public Cls_Setting(AlHerseyContext _context)
        {
            context = _context;
        }

        public async Task<List<Setting>> SettingSelectAsync()
        {
            List<Setting> settings = await context.Settings.ToListAsync();
            return settings;
        }

        public async Task<Setting?> SettingDetailsAsync(int? id)
        {
            Setting? settings = await context.Settings.FindAsync(id);
            return settings;
        }

        public async Task<bool> SettingUpdateAsync(Setting setting)
        {
            try
            {
                context.Update(setting);
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
