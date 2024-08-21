using MongoDB.Bson;
using MongoDB.Driver;
using PlanPartyBack.Models;
using System;
using System.Threading.Tasks;

namespace PlanPartyBack.Services
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly IMongoCollection<PasswordResetToken> _resetTokens;

        public PasswordResetService(IMongoDatabase database)
        {
            _resetTokens = database.GetCollection<PasswordResetToken>("PasswordResetTokens");
        }

        public string GenerateResetCode()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 6);
        }

        public async Task<bool> ValidateResetCodeAsync(ObjectId userId, string code)
        {
            var filter = Builders<PasswordResetToken>.Filter.And(
                Builders<PasswordResetToken>.Filter.Eq(t => t.UserId, userId),
                Builders<PasswordResetToken>.Filter.Eq(t => t.Token, code),
                Builders<PasswordResetToken>.Filter.Gt(t => t.ExpirationDate, DateTime.UtcNow)
            );

            var count = await _resetTokens.CountDocumentsAsync(filter);
            return count > 0;
        }

        public async Task CreatePasswordResetTokenAsync(ObjectId userId, string code, DateTime expirationDate)
        {
            var token = new PasswordResetToken
            {
                UserId = userId,
                Token = code,
                ExpirationDate = expirationDate
            };

            await _resetTokens.InsertOneAsync(token);
        }
    }
}
