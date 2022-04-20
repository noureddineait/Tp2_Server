using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace Tp2_Server.Models
{
    internal interface IKNN
    {
        /* main methods */
        void Train(string filename_train_samples_csv, int k = 1, int distance = 1);
        float Evaluate(string filename_test_samples_csv);
        bool Predict(Diagnostic sample_to_predict);
        /* utils */
        float EuclideanDistance(Diagnostic first_sample, Diagnostic second_sample);
        float ManhattanDistance(Diagnostic first_sample, Diagnostic second_sample);
        bool Vote(List<bool> sorted_labels);
        void ShellSort(List<float> distances, List<bool> labels);
        List<Diagnostic> ImportSamples(string filename_samples_csv);

    }
    public class KNN : IKNN
    {
       
        public int k { get; set; }
        public int distance { get; set; }
        public List<Diagnostic> TrainData { get; set; }
        public bool Predict(Diagnostic sample_to_predict)
        {
            List<bool> labels;
            List<float> distances = new List<float>();
            labels = new List<bool>(this.TrainData.Count);
            bool result;
            for (int i = 0; i < this.TrainData.Count; i++)
            {
                Diagnostic sampleTrain = new Diagnostic() { thal = this.TrainData[i].thal, cp = this.TrainData[i].cp, oldpeak = this.TrainData[i].oldpeak, ca = this.TrainData[i].ca, target = this.TrainData[i].target };
                if (this.distance == 0)
                {
                    distances.Add(EuclideanDistance(sample_to_predict, sampleTrain));
                }
                else
                {
                    distances.Add(ManhattanDistance(sample_to_predict, sampleTrain));
                }
                labels.Add(this.TrainData[i].target == "0" ? false : true);
            }
            ShellSort(distances, labels);

            result = Vote(labels.GetRange(0, k));
            return result;
        }

        public void Train(string filename_train_samples_csv, int k = 1, int distance = 1)
        {
            this.k = k;
            this.distance = distance;
            this.TrainData = ImportSamples(filename_train_samples_csv);
        }
        public float Evaluate(string filename_test_samples_csv)
        {
            List<bool> labels;
            List<float> distances = new List<float>();
            List<Diagnostic> EvaluationSet = ImportSamples(filename_test_samples_csv);
            labels = new List<bool>(this.TrainData.Count);
            bool result;
            float taux = 0;
            foreach (Diagnostic eval in EvaluationSet)
            {
                distances.Clear();
                Diagnostic sampleFoo = new Diagnostic() { thal = eval.thal, cp = eval.cp, oldpeak = eval.oldpeak, ca = eval.ca, target = eval.target };
                labels.Clear();
                for (int i = 0; i < this.TrainData.Count; i++)
                {
                    Diagnostic sampleTrain = new Diagnostic() { thal = this.TrainData[i].thal, cp = this.TrainData[i].cp, oldpeak = this.TrainData[i].oldpeak, ca = this.TrainData[i].ca, target = this.TrainData[i].target };
                    if (this.distance == 0)
                        distances.Add(EuclideanDistance(sampleFoo, sampleTrain));
                    else
                        distances.Add(ManhattanDistance(sampleFoo, sampleTrain));
                    labels.Add(this.TrainData[i].target == "0" ? false : true);
                }
                ShellSort(distances, labels);
                result = Vote(labels.GetRange(0, k));
                if (result == (eval.target == "0" ? false : true))
                {
                    taux++;
                }
            }
            return taux / EvaluationSet.Count * 100;
        }
        public float EuclideanDistance(Diagnostic first_sample, Diagnostic second_sample)
        {
            float distanceThal = (float)(Math.Pow((first_sample.thal - second_sample.thal), 2));
            float distanceThor = (float)(Math.Pow((first_sample.cp - second_sample.cp), 2));
            float distanceDep = (float)(Math.Pow((first_sample.oldpeak - second_sample.oldpeak), 2));
            float distanceVai = (float)(Math.Pow((first_sample.ca - second_sample.ca), 2));
            return (float)(Math.Sqrt(distanceThal + distanceThor + distanceDep + distanceVai));
        }
        public float ManhattanDistance(Diagnostic first_sample, Diagnostic second_sample)
        {
            float distanceThal = Math.Abs(first_sample.thal - second_sample.thal);
            float distanceThor = Math.Abs(first_sample.cp - second_sample.cp);
            float distanceDep = Math.Abs(first_sample.oldpeak - second_sample.oldpeak);
            float distanceVai = Math.Abs(first_sample.ca - second_sample.ca);
            return distanceThal + distanceThor + distanceDep + distanceVai;
        }
        public void ShellSort(List<float> distances, List<bool> labels)
        {
            int i, j, pos;
            float temp;
            bool temp2;
            pos = 3;
            while (pos > 0)
            {
                for (i = 0; i < distances.Count(); i++)
                {
                    j = i;
                    temp = distances[i];
                    temp2 = labels[i];
                    while ((j >= pos) && (distances[j - pos] > temp))
                    {
                        distances[j] = distances[j - pos];
                        labels[j] = labels[j - pos];
                        j = j - pos;
                    }
                    distances[j] = temp;
                    labels[j] = temp2;
                }
                if (pos / 2 != 0)
                    pos = pos / 2;
                else if (pos == 1)
                    pos = 0;
                else
                    pos = 1;
            }
        }
        public bool Vote(List<bool> sorted_labels)
        {
            int _true = 0;
            int _false = 0;
            foreach (bool v in sorted_labels)
            {
                if (v == false)
                {
                    _false++;
                }
                else
                {
                    _true++;
                }
            }
            if (_true > _false)
            {
                return true;
            }
            else if (_false > _true)
            {
                return false;
            }
            else
            {
                var random = new Random();
                var randomBool = random.Next(2) == 1;
                return randomBool;
            }

        }
        public List<Diagnostic> ImportSamples(string test)
        {
            using (var reader = new StreamReader(test))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<DiagnosticMap>();
                var records = csv.GetRecords<Diagnostic>().ToList();

                return records;
            }
        }

    }
    public sealed class DiagnosticMap : ClassMap<Diagnostic>
    {
        public DiagnosticMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.Label).Ignore();
        }
    }
}
