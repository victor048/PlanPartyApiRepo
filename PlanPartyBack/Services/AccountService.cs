﻿using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using PlanPartyBack.Models;

namespace PlanPartyBack.Services
{
    public class AccountService : IAccountService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IPasswordResetService _passwordResetService;
        private readonly INotificationService _notificationService;
        private readonly IValidationService _validationService;

        public AccountService(
            IMongoDatabase database,
            IPasswordHasher<User> passwordHasher,
            IPasswordResetService passwordResetService,
            INotificationService notificationService,
            IValidationService validationService)
        {
            _users = database.GetCollection<User>("Users");
            _passwordHasher = passwordHasher;
            _passwordResetService = passwordResetService;
            _notificationService = notificationService;
            _validationService = validationService;
        }

        public async Task<bool> CreateAccount(CreateAccountRequest request)
        {
            if (request.Contact.Contains("@"))
            {
                if (!_validationService.IsValidEmail(request.Contact))
                {
                    throw new ArgumentException("O e-mail informado não é válido.");
                }

                if (await _validationService.EmailExistsAsync(request.Contact))
                {
                    throw new ArgumentException("O e-mail já está em uso. Por favor, utilize outro e-mail.");
                }
            }
            else if (request.Contact.StartsWith("+") || request.Contact.All(char.IsDigit))
            {
                if (!_validationService.IsValidPhoneNumber(request.Contact))
                {
                    throw new ArgumentException("O número de telefone informado não é válido.");
                }

                if (await _validationService.PhoneExistsAsync(request.Contact))
                {
                    throw new ArgumentException("O telefone já está em uso. Por favor, utilize outro telefone.");
                }
            }
            else
            {
                if (!_validationService.IsValidWhatsAppNumber(request.Contact))
                {
                    throw new ArgumentException("O número de WhatsApp informado não é válido.");
                }

                if (await _validationService.WhatsAppExistsAsync(request.Contact))
                {
                    throw new ArgumentException("O WhatsApp já está em uso. Por favor, utilize outro número.");
                }
            }

            if (request.Password != request.ConfirmPassword)
            {
                throw new ArgumentException("As senhas não coincidem.");
            }

            var passwordValidationResult = ValidatePassword(request.Password);
            if (!passwordValidationResult.IsValid)
            {
                throw new ArgumentException(passwordValidationResult.ErrorMessage);
            }

            // Criar o usuário
            var user = new User
            {
                PasswordHash = _passwordHasher.HashPassword(null, request.Password),
                RecoveryOption = request.RecoveryOptions,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true,
                Roles = new List<string>()
            };

            if (request.Contact.Contains("@"))
            {
                user.Email = request.Contact;
            }
            else if (request.Contact.StartsWith("+") || request.Contact.All(char.IsDigit))
            {
                user.Phone = request.Contact;
            }
            else
            {
                user.WhatsApp = request.Contact;
            }

            await _users.InsertOneAsync(user);
            return true;
        }

        public async Task SendPasswordResetCodeAsync(string email, List<string> recoveryOptions)
        {
            var user = await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new ArgumentException("Usuário não encontrado.");
            }

            var resetCode = Guid.NewGuid().ToString("N").Substring(0, 6);
            var expirationDate = DateTime.UtcNow.AddHours(1);

            await _passwordResetService.CreatePasswordResetTokenAsync(user.Id, resetCode, expirationDate);

            foreach (var option in recoveryOptions)
            {
                if (option.Equals("Email", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        await _notificationService.SendEmailAsync(
                            email,
                            "Código de Recuperação de Senha",
                            $"Seu código de recuperação de senha é: {resetCode}");
                    }
                    catch (InvalidOperationException ex)
                    {
                        throw new InvalidOperationException("Falha ao enviar e-mail. Verifique se você configurou uma senha de aplicativo em sua conta de email. Para instruções, visite: https://support.google.com/accounts/answer/185833?hl=pt-BR", ex);
                    }
                }
                else if (option.Equals("Phone", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(user.Phone))
                {
                    await _notificationService.SendSmsAsync(
                        user.Phone,
                        $"Seu código de recuperação de senha é: {resetCode}");
                }
                else if (option.Equals("WhatsApp", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(user.WhatsApp))
                {
                    await _notificationService.SendWhatsAppMessageAsync(
                        user.WhatsApp,
                        $"Seu código de recuperação de senha é: {resetCode}");
                }
            }
        }

        public async Task<bool> ResetPassword(ResetPasswordRequest request)
        {
            var user = await _users.Find(u => u.Email == request.Email).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new ArgumentException("Usuário não encontrado.");
            }

            var isValidCode = await _passwordResetService.ValidateResetCodeAsync(user.Id, request.Code);
            if (!isValidCode)
            {
                throw new ArgumentException("Código de recuperação inválido ou expirado.");
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                throw new ArgumentException("As senhas não coincidem.");
            }

            var passwordValidationResult = ValidatePassword(request.NewPassword);
            if (!passwordValidationResult.IsValid)
            {
                throw new ArgumentException(passwordValidationResult.ErrorMessage);
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
            return true;
        }

        private (bool IsValid, string ErrorMessage) ValidatePassword(string password)
        {
            if (password.Length < 8)
            {
                return (false, "A senha deve ter pelo menos 8 caracteres.");
            }
            if (!password.Any(char.IsDigit))
            {
                return (false, "A senha deve conter pelo menos um número.");
            }
            if (!password.Any(char.IsLetter))
            {
                return (false, "A senha deve conter pelo menos uma letra.");
            }
            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                return (false, "A senha deve conter pelo menos um caractere especial.");
            }

            return (true, string.Empty);
        }
    }
}
