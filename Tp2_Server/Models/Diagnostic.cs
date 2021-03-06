using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tp2_Server.Models
{
    
        internal interface IDiagnostic
        {
            //float[] Features { get; }
            bool? Label { get; }
            void PrintInfo();
        }
    public class Diagnostic : IDiagnostic
    {
        [Ignore]

        public int DiagnosticID { get; set; }

        [Name("cp")]
        [Required]
        [Display(Name = "Type de douleur Thoracique")]

        public float cp { get; set; }
        [Required]
        [Name("ca")]
        [Display(Name = "Nombre de gros vaisseaux colorés par fluoroscopie")]

        public float ca { get; set; }
        [Required]
        [Name("oldpeak")]
        [Display(Name = "Dépression ST induite par l'exercice par rapport au repos")]

        public float oldpeak { get; set; }
        [Name("thal")]
        [Required]
        [Display(Name = "Thalassémie")]

        public float thal { get; set; }
        [Name("target")]
        
        public int target { get; set; }
        //private float[] _features = new float[5] { 0, 0, 0, 0, 0 };
        [Ignore] 
        public int PID { get; set; }

        [Ignore]
        public int k { get; set; }
        [Ignore]
        public int distance { get; set; }

        //public float[] Features { get; }

        public bool? Label { get; set; }
        public void PrintInfo()
        {
            Console.WriteLine($"{thal} + {target}");
        }
    }
}
