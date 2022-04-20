namespace Tp2_Server.Models
{
    public class Patient
    {
        public int PatientId { get; set; }
        public Diagnostic? Diagnostic { get; set; }
        
        public string? Nom { get; set; }
        
        public string? Prénom { get; set; }
       
        public string? Date { get; set; }
        
        public string? Ville { get; set; }
        
        public string? Genre { get; set; }
        
        public bool IsValid()
        {
            return (!string.IsNullOrEmpty(this.Nom) &&
                !string.IsNullOrEmpty(this.Prénom) &&
                !string.IsNullOrEmpty(this.Date) &&
                !string.IsNullOrEmpty(this.Ville) &&
                !string.IsNullOrEmpty(this.Genre)
                );
        }
    }
}
