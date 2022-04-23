using System.ComponentModel.DataAnnotations;

namespace Tp2_Server.Models
{
    public class Medecin
    {
        public int MedecinId { get; set; }
        public string? Nom { get; set; }
        public string? Prenom { get; set; }

        [Display(Name = "Date de naissance")]
        [DataType(DataType.Date)]
        public string? Date { get; set; }
        public string? Genre { get; set; }
        public string? Mail { get; set; }

        [Display(Name = "Date d'entrée en fonction")]
        [DataType(DataType.Date)]
        public string? Date_Entree { get; set; }
        public string? Ville { get; set; }

        public bool IsValid()
        {
            return (
                !string.IsNullOrEmpty(this.Nom) &&
                !string.IsNullOrEmpty(this.Prenom) &&
                !string.IsNullOrEmpty(this.Mail) &&
                !string.IsNullOrEmpty(this.Ville) &&
                !string.IsNullOrEmpty(this.Date) &&
                !string.IsNullOrEmpty(this.Date_Entree) &&
                !string.IsNullOrEmpty(this.Genre)
            );
        }
    }
}
