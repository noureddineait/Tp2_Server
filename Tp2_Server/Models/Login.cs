using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tp2_Server.Models
{
    public class Login
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        
        [NotMapped]
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
