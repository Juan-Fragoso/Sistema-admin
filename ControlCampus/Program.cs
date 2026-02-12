using Microsoft.EntityFrameworkCore;
using ControlCampus.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 1.Obtener el ConnectionString del appsettings.json
var connectionString = builder.Configuration.GetConnectionString("querySql");

// 2. Registrar el DbContext (Connection) para que esté disponible en todo el proyecto
builder.Services.AddDbContext<Connection>(options =>
    options.UseSqlServer(connectionString));

// Configuración de Autenticación por Cookies
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Ruta a la que redirige si no está logueado
        options.AccessDeniedPath = "/Account/AccessDenied"; // Ruta si no tiene permisos
        options.ExpireTimeSpan = TimeSpan.FromHours(8); // Duración de la sesión
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
