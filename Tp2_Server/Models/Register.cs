using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tp2_Server.Models
{
    public class Register
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }


        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Confirm Password required")]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password doesn't match.")]
        public string? ConfirmPassowrd { get; set; }
        
        [Required]
        public string? Nom { get; set; }

        [Required]
        public string? Prenom { get; set; }

        [Required]
        [Display(Name ="Date de naissance")]
        [DataType(DataType.Date)]
        public string? Date { get; set; }
        [Required]
        public string? Genre { get; set; }

        [Required]
        [Display(Name = "Date d'entrée en fonction")]
        [DataType(DataType.Date)]
        public string? Date_Entree { get; set; }

        [Required]
        public string? Ville { get; set; }

    }
}
