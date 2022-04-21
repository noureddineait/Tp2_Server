using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Tp2_Server.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Tp2_Server.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly AppDbContext appDbContext;
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,AppDbContext appDbContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.appDbContext = appDbContext;
            
        }
        [HttpPost]
        public async Task<ActionResult> Logout()
        {
            //HttpContext.Response.Cookies.Delete(".AspNetCore.Identity.cookies");
            
            await signInManager.SignOutAsync();
            TempData.Remove("myMedecin");
            return RedirectToAction("Index","Home");
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(Models.Register register)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = register.Email, Email = register.Email };
                var result = await userManager.CreateAsync(user, register.Password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    Medecin medecin = new Medecin { Nom = register.Nom, Prenom = register.Prenom, Mail = register.Email, Ville = register.Ville, Genre = register.Genre, Date = register.Date, Date_Entree = register.Date_Entree };
                    appDbContext.Medecins.Add(medecin);
                    appDbContext.SaveChanges();
                    TempData["myMedecin"] = JsonConvert.SerializeObject(medecin);
                    Console.WriteLine("");

                    return RedirectToAction("empty", "home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(register);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(Models.Login login)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(
                    login.Email, login.Password, login.RememberMe, false);

                if (result.Succeeded)
                {
                    try
                    {
                        Medecin medecin = appDbContext.Medecins.Where(m => m.Mail.ToLower() == login.Email.ToLower()).First();
                        TempData["myMedecin"] = JsonConvert.SerializeObject(medecin);
                        Console.WriteLine("");

                        return RedirectToAction("empty", "home");
                    }
                    catch (Exception)
                    {

                        return View(login);
                    }
                    
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }

            return View(login);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
