using CsvHelper.Configuration.Attributes;

namespace Tp2_Server.Models
{
    
        internal interface IDiagnostic
        {
            //float[] Features { get; }
            bool Label { get; }
            void PrintInfo();
        }
    public class Diagnostic : IDiagnostic
    {
        public int DiagnosticID { get; set; }

        [Name("cp")]
        public float cp { get; set; }
        [Name("ca")]
        public float ca { get; set; }
        [Name("oldpeak")]
        public float oldpeak { get; set; }
        [Name("thal")]
        public float thal { get; set; }
        [Name("target")]
        public string target { get; set; }
        //private float[] _features = new float[5] { 0, 0, 0, 0, 0 };



        //public float[] Features { get; }
        public bool Label { get; set; }
        public void PrintInfo()
        {
            Console.WriteLine($"{thal} + {target}");
        }
    }
}
