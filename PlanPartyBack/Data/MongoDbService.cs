using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using PlanPartyBack.Models;

namespace PlanPartyBack.Services
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;

        public MongoDbService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDB:ConnectionString"));
            _database = client.GetDatabase(configuration.GetValue<string>("MongoDB:DatabaseName"));
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<PasswordResetToken> PasswordResetTokens => _database.GetCollection<PasswordResetToken>("PasswordResetTokens");
    }
}
