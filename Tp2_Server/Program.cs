//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Tp2_Server.Models;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

string connectionString = "" +
               "server=localhost;" +
               "port=3306;" +
               "database=tp2_db;" +
               "user=tp2_user;" +
               "password=tp2_user;";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 13))));


builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddControllers();

builder.Services.AddMvc(option => option.EnableEndpointRouting = false);
var app = builder.Build();

//app.UseHttpsRedirection();
app.UseFileServer();
app.UseAuthentication();
app.UseAuthorization();
app.UseMvc(routes => routes.MapRoute("Default", "{controller=Account}/{action=Index}"));

app.Run();
