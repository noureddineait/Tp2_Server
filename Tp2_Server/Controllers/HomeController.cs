using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Tp2_Server.Models;

namespace Tp2_Server.Controllers
{
    public class HomeController : Controller
    {
        Models.Medecin medecin;
        //Models.Medecin m;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly AppDbContext appDbContext;

        public HomeController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, AppDbContext appDbContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.appDbContext = appDbContext;
            //Console.WriteLine("User.Identity.Name");
            //var t=User.Identity.Name;
            //medecin = appDbContext.Medecins.Where(m => m.Mail.ToLower() == User.Identity.Name.ToString().ToLower()).First();
            //Console.WriteLine("");
            //m = new Models.Medecin() { Nom = "Yasmine", Prenom = "Outtassi", Mail = "noureddinelhaj@gmail.com", Genre = "Male", Date = "13/05/1999", Ville = "Rimouski", Date_Entree = "19/10/2005" };
            //TempData["myMedecin"] = JsonConvert.SerializeObject("");
            //medecin = JsonConvert.DeserializeObject<Models.Medecin>(TempData["myMedecin"].ToString());
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Empty()
        {
            medecin = JsonConvert.DeserializeObject<Models.Medecin>(TempData["myMedecin"].ToString());
            return View(medecin);
        }
        public IActionResult Information() 
        {
                        medecin = JsonConvert.DeserializeObject<Models.Medecin>(TempData["myMedecin"].ToString());

            //User.Identity.Name
            return View(medecin); 
        }
        [HttpGet]
        public IActionResult Modifier()
        {
            //Models.Medecin m = new Models.Medecin() { Nom = "Yasmine", Prenom = "Outtassi", Mail = "noureddinelhaj@gmail.com", Genre = "Male", Date = "13/05/1999", Ville = "Rimouski", Date_Entree = "19/10/2005" };

            return View(medecin);
        }
        [HttpPost]
        public IActionResult Modifier(Models.Medecin med)
        {
            if (ModelState.IsValid)
            {
                foreach(var item in med.GetType().GetProperties())
                {
                    var objValue = item.GetValue(medecin);
                    var anotherValue = item.GetValue(med);
                    if (!objValue.Equals(anotherValue) && anotherValue is not null) item.SetValue(this.medecin,anotherValue);
                    TempData["myMedecin"] = JsonConvert.SerializeObject(medecin);
                }
                return RedirectToAction("information", "home");
                //ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }
                
            return View(med);
        }
        
    }
}
