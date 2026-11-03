
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

// ROUTE DETAIL PENGADUAN
app.MapControllerRoute(
    name: "detailPengaduan",
    pattern: "Pengaduan/Detail/{id}",
    defaults: new { controller = "Pengaduan", action = "Detail", id = 1 })
    .WithStaticAssets();

// ROUTE DEFAULT LOGIN
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();
