using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PlanPartyBack.Models;

namespace PlanPartyBack.Services
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;

        public MongoDbService(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<PasswordResetToken> PasswordResetTokens => _database.GetCollection<PasswordResetToken>("PasswordResetTokens");
    }
}
