using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Tp2_Server.Models;

namespace Tp2_Api.Controllers
{
    [Route("api/Home")]
    [Produces("application/json")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private AppDbContext appDbContext;

        public HomeController(UserManager<IdentityUser> userManager,
                              RoleManager<IdentityRole> roleManager,
                              AppDbContext appDbContext,
                              IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            this.appDbContext = appDbContext;
            _configuration = configuration;
        }
        
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = GetToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Email);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User already exists!" });

            IdentityUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                Medecin medecin = new Medecin() { Nom = model.Nom, Prenom = model.Prenom, Mail = model.Email, Ville = model.Ville, Genre = model.Genre, Date = model.Date, Date_Entree = model.Date_Entree };
                appDbContext.Medecins.Add(medecin);
                appDbContext.SaveChanges();
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = "User creation failed! Please check user details and try again." });
            }
            return Ok(new { Status = "Success", Message = "User created successfully!" });
        }
        
        
        [HttpGet,Authorize]
        [Route("Information")]
        public IActionResult GetInfo()
        {
            var currentUser = HttpContext.User;
            Medecin medecin = appDbContext.Medecins.Where(m => m.Mail.ToLower() == currentUser.Identity.Name.ToLower()).First();
            return Ok(medecin);
        }
        [HttpPut, Authorize]
        [Route("Information")]
        public IActionResult EditInfo([FromBody] Medecin model)
        {
            try
            {
                var currentUser = HttpContext.User;
                Medecin medecin = appDbContext.Medecins.Where(m => m.Mail.ToLower() == currentUser.Identity.Name.ToLower()).First();
                if (medecin != null)
                {
                    medecin.Prenom = model.Prenom ?? medecin.Prenom;
                    medecin.Nom = model.Nom ?? medecin.Nom;
                    medecin.Date = model.Date ?? medecin.Date;
                    medecin.Date_Entree = model.Date_Entree ?? medecin.Date_Entree;
                    medecin.Genre = model.Genre ?? medecin.Genre;
                    medecin.Ville = model.Ville ?? medecin.Ville;
                    appDbContext.SaveChanges();
                    return Ok();
                }
                else
                    return StatusCode((int)HttpStatusCode.NotFound);
            }
            catch (Exception)
            {

               
            }

            return BadRequest();
        }

        [HttpGet, Authorize]
        [Route("Patients")]
        public IActionResult GetPatients()
        {
            List<Patient> patients = appDbContext.Patients.ToList();
            return Ok(patients);
        }
        [HttpPost, Authorize]
        [Route("Patients")]
        public IActionResult AddPatients([FromBody] Patient model)
        {
            try
            {
                Patient patient = new Patient() { Date=model.Date,Nom=model.Nom,Prenom = model.Prenom, Genre=model.Genre,Ville=model.Ville};
                appDbContext.Patients.Add(patient);
                appDbContext.SaveChanges();
                return Ok(patient);
            }
            catch (Exception)
            {


            }

            return BadRequest();
        }


        [Authorize]
        [Route("Patient/{patientIdx}")]
        [HttpGet]
        
        public IActionResult GetPatient(int patientIdx)
        {
            try
            {
                Patient patient = appDbContext.Patients.Find(patientIdx);
                if(patient != null)
                {
                    return Ok(patient);
                }
                else
                    return StatusCode((int)HttpStatusCode.NotFound);
            }
            catch (Exception)
            {


            }

            return BadRequest();
        }

        [Authorize]
        [Route("Patient/{patientIdx}")]
        [HttpDelete]

        public IActionResult DeletePatient(int patientIdx)
        {
            try
            {
                Patient patient = appDbContext.Patients.Find(patientIdx);
                if (patient != null)
                {
                    List<Diagnostic> diagnostics = appDbContext.Diagnostics.Where(d=>d.PID==patientIdx).ToList();
                    foreach (Diagnostic diagnostic in diagnostics)
                    {
                        appDbContext.Diagnostics.Remove(diagnostic);
                    }
                    appDbContext.Patients.Remove(patient);
                    appDbContext.SaveChanges();
                    return Ok(patient);
                }
                else
                    return StatusCode((int)HttpStatusCode.NotFound);
            }
            catch (Exception)
            {


            }

            return BadRequest();
        }

        [Authorize]
        [Route("Patient/{patientIdx}/Diagnostics")]
        [HttpGet]

        public IActionResult GetDiagnostics(int patientIdx)
        {
            try
            {
                Patient patient = appDbContext.Patients.Find(patientIdx);
                if (patient != null)
                {
                    List<Diagnostic> diagnostics = appDbContext.Diagnostics.Where(d => d.PID == patientIdx).ToList();
                    return Ok(diagnostics);
                }
                else
                    return StatusCode((int)HttpStatusCode.NotFound);
            }
            catch (Exception)
            {


            }

            return BadRequest();
        }
        [Authorize]
        [Route("Diagnostic/{diagnosticIdx}")]
        [HttpGet]

        public IActionResult GetDiagnostic(int diagnosticIdx)
        {
            try
            {
                Diagnostic diagnostic = appDbContext.Diagnostics.Find(diagnosticIdx);
                if (diagnostic != null)
                {                  
                    return Ok(diagnostic);
                }
                else
                    return StatusCode((int)HttpStatusCode.NotFound);
            }
            catch (Exception)
            {


            }

            return BadRequest();
        }
        [Authorize]
        [Route("Diagnostic/{diagnosticIdx}")]
        [HttpDelete]

        public IActionResult DeleteDiagnostic(int diagnosticIdx)
        {
            try
            {
                Diagnostic diagnostic = appDbContext.Diagnostics.Find(diagnosticIdx);
                if (diagnostic != null)
                {
                    appDbContext.Diagnostics.Remove(diagnostic);
                    appDbContext.SaveChanges();
                    return Ok(diagnostic);
                }
                else
                    return StatusCode((int)HttpStatusCode.NotFound);
            }
            catch (Exception)
            {


            }

            return BadRequest();
        }


        [Authorize]
        [Route("Patient/{patientIdx}/diagnose")]
        [HttpPost]

        public IActionResult Diagnose(int patientIdx, [FromBody] Diagnostic model)
        {
            try
            {
                
                KNN knn = new KNN();
                knn.Train("./wwwroot/train.csv", model.k, model.distance);
                Diagnostic diagnostic = new Diagnostic() { ca=model.ca,cp=model.cp , oldpeak = model.oldpeak, thal=model.thal,k=model.k , distance = model.distance,PID = patientIdx};
                if (knn.Predict(diagnostic))
                {
                    diagnostic.target = 1;
                    diagnostic.Label = true;
                }
                else
                {
                    diagnostic.target = 0;
                    diagnostic.Label = false;
                }
                appDbContext.Diagnostics.Add(diagnostic);
                appDbContext.SaveChanges();
                return Ok(diagnostic);
            }
            catch (Exception)
            {


            }

            return BadRequest();
        }
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}
