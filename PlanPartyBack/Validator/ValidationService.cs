using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PlanPartyBack.Models;
using System.Text.RegularExpressions;

namespace PlanPartyBack.Services
{
    public class ValidationService : IValidationService
    {
        private readonly MongoDbSettings _mongoDbSettings;

        public ValidationService(

            IOptions<MongoDbSettings> mongoDbSettings)
        {
            _mongoDbSettings = mongoDbSettings.Value;
        }

        public bool IsValidEmail(string email)
        {
            // Regex básico para validação de e-mail
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        public bool IsValidPhoneNumber(string phoneNumber)
        {
            var phonePattern = @"^\+?[1-9]\d{1,14}$";
            return Regex.IsMatch(phoneNumber, phonePattern);
        }

        public bool IsValidWhatsAppNumber(string whatsAppNumber)
        {
            return IsValidPhoneNumber(whatsAppNumber);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var client = new MongoClient(_mongoDbSettings.ConnectionString);
            var database = client.GetDatabase(_mongoDbSettings.DatabaseName);
            var usersCollection = database.GetCollection<User>("Users");

            var existingUser = await usersCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
            return existingUser != null;
        }

        public async Task<bool> PhoneExistsAsync(string phoneNumber)
        {
            var client = new MongoClient(_mongoDbSettings.ConnectionString);
            var database = client.GetDatabase(_mongoDbSettings.DatabaseName);
            var usersCollection = database.GetCollection<User>("Users");

            var existingUser = await usersCollection.Find(u => u.Phone == phoneNumber).FirstOrDefaultAsync();
            return existingUser != null;
        }

        public async Task<bool> WhatsAppExistsAsync(string whatsAppNumber)
        {
            var client = new MongoClient(_mongoDbSettings.ConnectionString);
            var database = client.GetDatabase(_mongoDbSettings.DatabaseName);
            var usersCollection = database.GetCollection<User>("Users");

            var existingUser = await usersCollection.Find(u => u.WhatsApp == whatsAppNumber).FirstOrDefaultAsync();
            return existingUser != null;
        }
    }
}
