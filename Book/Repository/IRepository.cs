using System.Collections.Generic;
using System.Threading.Tasks;

namespace Book.Models
{
    public interface IRepository
    {
        Task<IEnumerable<Message>> GetMessagesAsync();
        Task<User?> GetUserByNameAsync(string name);
        Task AddMessageAsync(Message message);
        Task AddUserAsync(User user);
        Task<bool> UserExistsAsync(string name);
        Task SaveAsync();
    }
}

