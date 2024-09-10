using Amazon.Runtime.Internal.Util;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlanPartyBack.Models;
using PlanPartyBack.Services;
using System.Security.Claims;

namespace PlanPartyBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IUserService _userService;
        private readonly IPasswordResetService _passwordResetService;
        private readonly INotificationService _notificationService;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountController(IAccountService accountService, IUserService userService,
            IPasswordResetService passwordResetService, INotificationService notificationService,
            IPasswordHasher<User> passwordHasher)
        {
            _accountService = accountService;
            _userService = userService;
            _passwordResetService = passwordResetService;
            _notificationService = notificationService;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Contact) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Contato e senha são obrigatórios.");
            }

            var user = await _userService.AuthenticateAsync(request.Contact, request.Password);
            if (user == null)
            {
                return Unauthorized("Contato ou senha incorretos.");
            }

            return Ok(user);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var success = await _accountService.CreateAccount(request);
                if (success)
                {
                    return Ok("Conta criada com sucesso.");
                }
                return StatusCode(500, "Erro ao criar a conta.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) && string.IsNullOrEmpty(request.Phone) && string.IsNullOrEmpty(request.WhatsApp))
            {
                return BadRequest("Pelo menos um método de recuperação deve ser fornecido.");
            }

            var user = await _userService.GetUserByContactInfoAsync(request.Email, request.Phone, request.WhatsApp);

            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            var code = _passwordResetService.GenerateResetCode();
            await _passwordResetService.CreatePasswordResetTokenAsync(user.Id, code, DateTime.UtcNow.AddHours(1));

            try
            {
                if (!string.IsNullOrEmpty(request.Email))
                {
                    await _notificationService.SendEmailAsync(request.Email, "Código de Recuperação", $"Seu código de recuperação é: {code}");
                }
                else if (!string.IsNullOrEmpty(request.Phone))
                {
                    await _notificationService.SendSmsAsync(request.Phone, $"Seu código de recuperação é: {code}");
                }
                else if (!string.IsNullOrEmpty(request.WhatsApp))
                {
                    await _notificationService.SendWhatsAppMessageAsync(request.WhatsApp, $"Seu código de recuperação é: {code}");
                }
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ex.Message);
            }

            return Ok("Código de recuperação enviado.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var success = await _accountService.ResetPassword(request);
                if (success)
                {
                    return Ok("Senha redefinida com sucesso.");
                }
                return StatusCode(500, "Erro ao redefinir a senha.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
