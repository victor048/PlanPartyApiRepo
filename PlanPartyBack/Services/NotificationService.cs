using System;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace PlanPartyBack.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public NotificationService(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var smtpSettings = _configuration.GetSection("Smtp");

            var smtpClient = new SmtpClient(smtpSettings["Host"])
            {
                Port = int.Parse(smtpSettings["Port"]),
                Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings["FromEmail"]),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (SmtpException ex)
            {
                if (ex.StatusCode == SmtpStatusCode.ClientNotPermitted || ex.Message.Contains("5.7.0 Authentication Required"))
                {
                    throw new InvalidOperationException("Falha na autenticação SMTP. Verifique se você configurou uma senha de aplicativo em sua conta de email. Para instruções, visite: https://support.google.com/accounts/answer/185833?hl=pt-BR");
                }
                throw new InvalidOperationException("Erro ao enviar o e-mail.", ex);
            }
        }

        public async Task SendSmsAsync(string toPhoneNumber, string message)
        {
            // Garantir que o número está no formato correto (E.164)
            if (!toPhoneNumber.StartsWith("+"))
            {
                throw new ArgumentException("Número de telefone deve estar no formato E.164, começando com '+'.");
            }
            var telesignSettings = _configuration.GetSection("Telesign");
            var customerId = telesignSettings["CustomerId"];
            var apiKey = telesignSettings["ApiKey"];

            var url = "https://rest-api.telesign.com/v1/messaging";

            var payload = new
            {
                phone_number = toPhoneNumber,
                message = message,
                message_type = "ARN"
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{customerId}:{apiKey}"))}");

            var response = await _httpClient.PostAsync(url, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
               throw new InvalidOperationException($"Erro ao enviar o SMS: {responseBody}");
            }
        }

        public async Task SendWhatsAppMessageAsync(string toWhatsAppNumber, string message)
        {
            var telesignSettings = _configuration.GetSection("Telesign");
            var customerId = telesignSettings["CustomerId"];
            var apiKey = telesignSettings["ApiKey"];
            var whatsAppServiceId = telesignSettings["WhatsAppServiceId"];

            var url = $"https://rest-api.telesign.com/v1/messaging";

            var payload = new
            {
                phone_number = toWhatsAppNumber,
                message = message,
                message_type = "ARN"
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"{customerId}:{apiKey}"))}");

            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Erro ao enviar a mensagem pelo WhatsApp: {responseBody}");
            }
        }

    }
}
