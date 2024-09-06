public interface IValidationService
{
    bool IsValidEmail(string email);
    bool IsValidPhoneNumber(string phoneNumber);
    bool IsValidWhatsAppNumber(string whatsAppNumber);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> PhoneExistsAsync(string phoneNumber);
    Task<bool> WhatsAppExistsAsync(string whatsAppNumber);
}
