using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using PlanPartyBack.Models;
using System.Threading.Tasks;

namespace PlanPartyBack.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(IMongoDatabase database, IPasswordHasher<User> passwordHasher)
        {
            _users = database.GetCollection<User>("Users");
            _passwordHasher = passwordHasher;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByContactInfoAsync(string email, string phone, string whatsApp)
        {
            return await _users.Find(u => u.Email == email || u.Phone == phone || u.WhatsApp == whatsApp).FirstOrDefaultAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, user.Email);
            await _users.ReplaceOneAsync(filter, user);
        }

        public async Task<User> AuthenticateAsync(string contact, string password)
        {
            var user = await _users.Find(u => u.Email == contact || u.Phone == contact || u.WhatsApp == contact).FirstOrDefaultAsync();
            if (user == null)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return user;
        }

        public async Task AddUserAsync(User user)
        {
            await _users.InsertOneAsync(user);
        }
    }

}
