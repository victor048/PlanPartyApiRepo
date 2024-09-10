using PlanPartyBack.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlanPartyBack.Services
{
    public interface IAccountService
    {
        Task<bool> CreateAccount(CreateAccountRequest request);
        Task SendPasswordResetCodeAsync(string email, List<string> recoveryOptions);
        Task<bool> ResetPassword(ResetPasswordRequest request);
    }
}
