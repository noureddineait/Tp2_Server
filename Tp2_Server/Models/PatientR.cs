using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tp2_Server.Models
{
    public class PatientR
    {
        public int PatientId { get; set; }

        public Diagnostic? Diagnostic { get; set; }

        [Required]
        public string? Nom { get; set; }

        [Required]
        public string? Prenom { get; set; }
        [Required]
        [Display(Name = "Date de naissance")]
        [DataType(DataType.Date)]
        public string? Date { get; set; }
        [Required]
        public string? Ville { get; set; }
        [Required]
        public string? Genre { get; set; }
        public int? MID { get; set; }

        public bool IsValid()
        {
            return (!string.IsNullOrEmpty(this.Nom) &&
                !string.IsNullOrEmpty(this.Prenom) &&
                !string.IsNullOrEmpty(this.Date) &&
                !string.IsNullOrEmpty(this.Ville) &&
                !string.IsNullOrEmpty(this.Genre)
                );
        }
    }
}
