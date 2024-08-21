using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlanPartyBack.Models
{
    public class CreateAccountRequest
    {
        [Required]
        public string Contact { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "As senhas não coincidem.")]
        public string ConfirmPassword { get; set; }

        public List<string> RecoveryOptions { get; set; }

    }
}
