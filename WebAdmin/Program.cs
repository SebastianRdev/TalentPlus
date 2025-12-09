using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Infrastructure.Persistence;
using Infrastructure.Identity;
using Infrastructure.Services.Identity;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. CARGAR ARCHIVO .env
// ==========================================
Env.Load("/home/Coder/Música/sebas/TalentPlus/.env");
builder.Configuration.AddEnvironmentVariables();

// ==========================================
// 2. CONFIGURACIÓN DE LA BASE DE DATOS
// ==========================================
var host = Environment.GetEnvironmentVariable("DB_HOST");
var port = Environment.GetEnvironmentVariable("DB_PORT");
var user = Environment.GetEnvironmentVariable("DB_USER");
var pass = Environment.GetEnvironmentVariable("DB_PASS");
var name = Environment.GetEnvironmentVariable("DB_NAME");

var connectionString = $"Host={host};Port={port};Database={name};Username={user};Password={pass};";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// ==========================================
// 3. CONFIGURACIÓN DE IDENTITY
// ==========================================
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders(); // Añadido para configurar correctamente el sistema de Identity

// ==========================================
// 4. SERVICIOS NECESARIOS
// ==========================================
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IAuthService, AuthService>();

// ==========================================
// 5. CONSTRUCCIÓN DE LA APLICACIÓN
// ==========================================
var app = builder.Build();

// ==========================================
// 6. CONFIGURACIÓN DEL PIPELINE
// ==========================================
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();  // Migrations en desarrollo
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// ==========================================
// 7. RUTAS Y PÁGINAS
// ==========================================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
