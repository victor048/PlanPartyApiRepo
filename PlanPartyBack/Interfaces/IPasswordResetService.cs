using MongoDB.Bson;
using System;
using System.Threading.Tasks;

namespace PlanPartyBack.Services
{
    public interface IPasswordResetService
    {
        Task CreatePasswordResetTokenAsync(ObjectId userId, string code, DateTime expirationDate);
        Task<bool> ValidateResetCodeAsync(ObjectId userId, string code);
        string GenerateResetCode();
    }
}
