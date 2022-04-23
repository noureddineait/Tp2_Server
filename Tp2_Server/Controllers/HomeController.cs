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

        public HomeController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, AppDbContext appDbContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.appDbContext = appDbContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Empty()
        {
            medecin = appDbContext.Medecins.Where(m => m.Mail.ToLower() == userManager.GetUserName(HttpContext.User).ToLower()).First();
            return View(medecin);
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
                return RedirectToAction("Diagnostic", "home", patient);
            }
            return View(patientform);
        }
        [HttpGet]
        public IActionResult Diagnostic(KNN knn)
        {
            knn.Train("./wwwroot/train.csv", knn.k, knn.distance);
            float result = knn.Evaluate("./wwwroot/test.csv");
            Console.WriteLine(result);
            ViewBag.Knn = knn;
            Patient patient = new Patient();
            List<Patient> patients = appDbContext.Patients.ToList();
            if (patients.Count > 0)
            {
                ViewBag.Patients = patients;
                return View();
            }
            patients.Add(patient);
            ViewBag.Patients = patients;
            return View();
            Console.WriteLine(result);
        }
        [HttpPost]
        public IActionResult Diagnostic(Diagnostic diagnostic, KNN knn)
        {
            if (ModelState.IsValid)
            {

                appDbContext.Diagnostics.Add(diagnostic);
                return View();
            }
            return View(diagnostic);
        }
        public IActionResult InformationPatient(Patient patient)
        {
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
            return RedirectToAction("Diagnostic", "home", kNN);

        }

    }
}
