using System.Threading.Tasks;
using PlanPartyBack.Models;

namespace PlanPartyBack.Services
{
    public interface IAccountService
    {
        Task<bool> CreateAccount(CreateAccountRequest request);
        Task SendPasswordResetCodeAsync(string email, List<string> recoveryOptions);
        Task<bool> ResetPassword(ResetPasswordRequest request);
    }
}
