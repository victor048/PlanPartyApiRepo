public interface INotificationService
{
    Task SendEmailAsync(string toEmail, string subject, string message);
    Task SendSmsAsync(string toPhoneNumber, string message);
    Task SendWhatsAppMessageAsync(string toWhatsAppNumber, string message);
}
