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
// ==========================================
// 3. CONFIGURACIÓN DE IDENTITY (CORREGIDO)
// ==========================================
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options => // <-- Usa AddIdentity y especifica ApplicationRole
    {
        // Puedes copiar las opciones de contraseña que usaste en el proyecto API aquí, si quieres un control fino.
        // O dejar las opciones predeterminadas.
        options.SignIn.RequireConfirmedAccount = false; // Cambiado a false para simplificar el login, si no estás usando confirmación.
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 💡 Nota: Asegúrate de que tu proyecto web tenga la referencia al paquete Microsoft.AspNetCore.Identity.UI si la página de login/registro es predeterminada.

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

app.Run();
