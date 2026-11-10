using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SafeCampus.Models;
using SafeCampus.Services;

var builder = WebApplication.CreateBuilder(args);

// =========================================================
//  SERVICES
// =========================================================

builder.Services.AddControllersWithViews();

// Database — MySQL via EF Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Cookie Authentication (login / logout)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath        = "/Account/Login";
        options.LogoutPath       = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan   = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

// File upload service
builder.Services.AddScoped<IFileUploadService, BuktiUploadService>();

// =========================================================
//  BUILD
// =========================================================

var app = builder.Build();

// =========================================================
//  MIGRASI & SEED — Jalankan otomatis saat startup
// =========================================================

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();   // jalankan migration yang belum diaplikasikan
}

// =========================================================
//  MIDDLEWARE
// =========================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();   // HARUS sebelum UseAuthorization
app.UseAuthorization();

app.MapStaticAssets();

// =========================================================
//  ROUTES
// =========================================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();
