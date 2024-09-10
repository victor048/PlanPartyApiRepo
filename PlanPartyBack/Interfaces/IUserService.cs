using PlanPartyBack.Models;
using System.Threading.Tasks;

namespace PlanPartyBack.Services
{
    public interface IUserService
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByContactInfoAsync(string email, string phone, string whatsApp);
        Task UpdateUserAsync(User user);
        Task<User> AuthenticateAsync(string contact, string password);
    }
}
