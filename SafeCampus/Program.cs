using Microsoft.EntityFrameworkCore;
using SafeCampus.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// --- KODE DATABASE HARUS DI SINI (SEBELUM BUILD) ---
// Mengambil URL koneksi dari appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Mendaftarkan ApplicationDbContext menggunakan MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
// ---------------------------------------------------

// BUILD APLIKASI DILAKUKAN SETELAH SEMUA LAYANAN DIDAFTARKAN
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
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();