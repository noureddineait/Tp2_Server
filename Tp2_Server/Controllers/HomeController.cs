using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Principal;
using Tp2_Server.Models;
using System.Web;
using System.Dynamic;

namespace Tp2_Server.Controllers
{
    public class HomeController : Controller
    {
        Medecin medecin;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly AppDbContext appDbContext;

        public HomeController(UserManager<IdentityUser> userManager,
                              SignInManager<IdentityUser> signInManager,
                              AppDbContext appDbContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.appDbContext = appDbContext;
        }
       
        public IActionResult Information()
        {
            medecin = appDbContext.Medecins.Where(m => m.Mail.ToLower() == userManager.GetUserName(HttpContext.User).ToLower()).First();
            return View(medecin);
        }
        [HttpGet]
        public IActionResult Modifier()
        {
            medecin = appDbContext.Medecins.Where(m => m.Mail.ToLower() == userManager.GetUserName(HttpContext.User).ToLower()).First();
            return View(medecin);
        }
        [HttpPost]
        public IActionResult Modifier(Medecin med)
        {
            medecin = appDbContext.Medecins.Where(m => m.Mail.ToLower() == userManager.GetUserName(HttpContext.User).ToLower()).First();

            if (ModelState.IsValid)
            {
                foreach (var item in med.GetType().GetProperties())
                {
                    if (item.Name != "MedecinId")
                    {
                        var objValue = item.GetValue(medecin);
                        var anotherValue = item.GetValue(med);
                        if (!objValue.Equals(anotherValue) && anotherValue is not null) item.SetValue(this.medecin, anotherValue);
                    }
                }
                appDbContext.SaveChanges();
                return RedirectToAction("information", "home");
            }
            return View(med);
        }
        [HttpGet]
        public IActionResult AjoutPatient()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AjoutPatient(PatientR patientform)
        {
            if (ModelState.IsValid)
            {
                Patient patient = new Patient { Nom = patientform.Nom, Prenom = patientform.Prenom, Ville = patientform.Ville, Genre = patientform.Genre, Date = patientform.Date };
                appDbContext.Patients.Add(patient);
                appDbContext.SaveChanges();
                return RedirectToAction("InformationPatient", patient);
            }
            return View(patientform);
        }
        [HttpGet]
        public IActionResult Diagnostic(KNN knn)
        {
            //knn.Train("./wwwroot/train.csv", knn.k, knn.distance);
            //KNN knn2 = appDbContext.KNNs.Where(k => k.k == knn.k && k.distance == knn.distance).First();
            knn.Train("./wwwroot/train.csv", knn.k, knn.distance);
            float result = knn.Evaluate("./wwwroot/test.csv");
            ViewBag.Resultat = result;
            ViewBag.Knn = knn;
            Diagnostic diagnostic = new Diagnostic() { k = knn.k , distance =  knn.distance};
            List<Patient> patients = appDbContext.Patients.ToList();
            if (patients.Count > 0)
            {
                //TempData["Knn"] = JsonConvert.SerializeObject(knn2);
                ViewBag.Patients = patients;
                return View(diagnostic);
            }
            Patient patient = new Patient();
            patients.Add(patient);
            ViewBag.Patients = patients;
            return View();
            Console.WriteLine(diagnostic);
        }
        [HttpPost]
        public IActionResult Diagnostic(Diagnostic diagnostic,string submit)
        {
            Console.WriteLine("");
            KNN knn = new KNN();
            knn.Train("./wwwroot/train.csv", diagnostic.k, diagnostic.distance);
            float result = knn.Evaluate("./wwwroot/test.csv");
            ViewBag.Resultat = result;
            ViewBag.Knn = knn;
            Patient patient = new Patient();
            List<Patient> patients = appDbContext.Patients.ToList();
            
            patients.Add(patient);
            ViewBag.Patients = patients;
            if(submit == "Diagnostiquer")
            {
                ModelState.Remove("KNN.TrainData");

                if (ModelState.IsValid)
                {
                    Patient patientD = appDbContext.Patients.Find(diagnostic.PID);
                    Diagnostic diagnostic1 = new Diagnostic() { ca = diagnostic.ca, cp = diagnostic.cp, oldpeak = diagnostic.oldpeak, thal = diagnostic.thal, PID = diagnostic.PID, k = diagnostic.k, distance = diagnostic.distance };

                    if (knn.Predict(diagnostic1))
                    {
                        diagnostic1.target = 1;
                        diagnostic1.Label = true;
                    }
                    else
                    {
                        diagnostic1.target = 0;
                        diagnostic1.Label = false;
                    }
                    appDbContext.Diagnostics.Add(diagnostic1);
                    appDbContext.SaveChanges();
                    return RedirectToAction("informationPatient", patientD);
                }
                return View();
            }
            else if(submit == "Information")
            {
                if (diagnostic.PID != 0)
                {
                    Patient patientSelected = appDbContext.Patients.Find(diagnostic.PID);
                    return RedirectToAction("InformationPatient", patientSelected);
                }
                else
                {
                    return View();
                }
                
            }

            return View();


        }
        [HttpGet]
        public IActionResult InformationPatient(Patient patient)
        {
            if(appDbContext.Diagnostics.Where(t => t.PID == patient.PatientId).Count() > 0)
            {
                Diagnostic diagnostic = appDbContext.Diagnostics.Where(t => t.PID == patient.PatientId).ToList().Last();
                if (diagnostic.Label == true)
                {
                    ViewBag.Result = "Malade";
                }
                else
                {
                    ViewBag.Result = "Pas de Maladie";
                }
                
            }
            else { ViewBag.Result = "Veuillez diagnostiquer ce patient pour avoir un resultat !"; }
            return View(patient);
        }
        [HttpPost]
        public IActionResult InformationPatient(Patient patient,string submit)
        {
            if(submit == "Supprimer")
            {
                if(appDbContext.Diagnostics.Where(t=>t.PID == patient.PatientId).Count() > 0)
                {
                    List<Diagnostic> diagnostics = appDbContext.Diagnostics.Where(t => t.PID == patient.PatientId).ToList();
                    foreach(Diagnostic diagnostic in diagnostics)
                    {
                        appDbContext.Diagnostics.Remove(diagnostic);
                    }                 
                }
                appDbContext.Patients.Remove(patient);
                appDbContext.SaveChanges();
                return RedirectToAction("Patients");
            }
            else if(submit == "Liste De Diagnostique")
            {
                return RedirectToAction("ListDiagnostics", patient);
            }
            return View(patient);
        }
        [HttpGet]
        public IActionResult Configurer()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Configurer(KNN kNN)
        {

            //kNN.Train("./wwwroot/train.csv",kNN.k,kNN.distance);
            //kNN.Evaluate("./wwwroot/test.csv");
            
            if (ModelState.IsValid)
            {
                
                return RedirectToAction("Diagnostic", "home", kNN);

            }
            return View(kNN);

        }
        public IActionResult Patients()
        {
            List<Models.Patient> patients = appDbContext.Patients.ToList();
            ViewBag.Patients = patients;
            return View();
        }
        [HttpPost]
        public IActionResult Patients(Patient patient)
        {
            List<Models.Patient> patients = appDbContext.Patients.ToList();
            ViewBag.Patients = patients;
            if (patient.PatientId != 0)
            {
                Patient patientSelected = appDbContext.Patients.Find(patient.PatientId);
                return RedirectToAction("InformationPatient", patientSelected);
            }
            return View();
        }
        [HttpGet]
        public IActionResult ListDiagnostics(Patient patient)
        {
            List<Diagnostic> diagnostics = appDbContext.Diagnostics.Where(t=>t.PID==patient.PatientId).ToList();
            List<string> results = new List<string>();
            if (diagnostics.Count > 0) {
                foreach (var t in diagnostics)
                {
                    //KNN knn = appDbContext.KNNs.Find(t.KnnId);
                    string distance = "";
                    if (t.distance==0)
                    {
                        distance = "Euclidean";
                    }
                    else distance = "Manhattan";
                    string maladie = "";
                    if (t.Label ==true)
                    {
                        maladie = "Malade";
                    } else maladie = "Non malade";
                    results.Add($"{t.DiagnosticID} || cp = {t.cp} || ca = {t.ca} || oldpeak = {t.oldpeak} || thal = {t.thal} || {maladie} || K = {t.k} || distance = {distance}");
                }
            }
            ViewBag.Diagnostics = diagnostics;
            ViewBag.DiagnosticsText = results;
            return View();
        }
        [HttpPost]
        public IActionResult ListDiagnostics(Patient patient,Diagnostic diagnostic,string submit)
        {
            List<Diagnostic> diagnostics = appDbContext.Diagnostics.Where(t => t.PID == patient.PatientId).ToList();
            List<string> results = new List<string>();
            if (diagnostics.Count > 0)
            {
                foreach (var t in diagnostics)
                {
                    //KNN knn = appDbContext.KNNs.Find(t.KnnId);
                    string distance = "";
                    if (t.distance == 0)
                    {
                        distance = "Euclidean";
                    }
                    else distance = "Manhattan";
                    string maladie = "";
                    if (t.Label == true)
                    {
                        maladie = "Malade";
                    }
                    else maladie = "Non malade";
                    results.Add($"{t.DiagnosticID} || cp = {t.cp} || ca = {t.ca} || oldpeak = {t.oldpeak} || thal = {t.thal} || {maladie} || K = {t.k} || distance = {distance}");
                }
            }
            ViewBag.DiagnosticsText = results;
            ViewBag.Diagnostics = diagnostics;
            if(submit == "Supprimer")
            {
                
                    if (diagnostic.DiagnosticID != 0)
                    {
                        Diagnostic diagnosticS = appDbContext.Diagnostics.Find(diagnostic.DiagnosticID);
                        appDbContext.Diagnostics.Remove(diagnosticS);
                        appDbContext.SaveChanges();
                        return RedirectToAction("ListDiagnostics", patient);
                    }
                
                    return View();
                
                
            }
            else if(submit == "Retour")
            {
                return RedirectToAction("informationPatient", patient);
            }
            
            return View(patient);
        }
    }
}
