using Microsoft.EntityFrameworkCore;

namespace AlHersey.Models
{
    public interface IMessageRepository
    {
        Task<List<Message>> MessageSelectAsync();
        Task<Message?> MessageDetailsAsync(int? id);
    }

    public class Cls_Message : IMessageRepository
    {
        private readonly AlHerseyContext context;

        public Cls_Message(AlHerseyContext _context)
        {
            context = _context;
        }

        public async Task<List<Message>> MessageSelectAsync()
        {
            List<Message> messages = await context.Messages.ToListAsync();
            return messages;
        }

        public async Task<Message?> MessageDetailsAsync(int? id)
        {
            Message? message = await context.Messages.FindAsync(id);
            return message;
        }
    }
}
