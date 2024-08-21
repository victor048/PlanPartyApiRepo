using System.ComponentModel.DataAnnotations;

namespace PlanPartyBack.Models
{
    public class LoginRequest
    {
        [Required]
        public string Contact { get; set; }  // Pode ser Email, Phone ou WhatsApp

        [Required]
        public string Password { get; set; }
    }

}
