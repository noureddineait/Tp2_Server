using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Tp2_Server.Models
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<Diagnostic> Diagnostics { get; set; }
        public DbSet<Medecin> Medecins { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<KNN> KNNs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            string connextionString = "" +
                "server=localhost;" +
                "port=3306;" +
                "database=tp2_db;" +
                "user=tp2_user;" +
                "password=tp2_user;";

            dbContextOptionsBuilder.UseMySql(connextionString, ServerVersion.AutoDetect(connextionString));



        }
    }
}
