using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeCampus.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace SafeCampus.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        // Kalau belum login, arahkan ke halaman login
        if (User.Identity?.IsAuthenticated != true)
            return RedirectToAction("Login", "Account");

        // Kalau sudah login, arahkan sesuai role
        return User.IsInRole("admin")
            ? RedirectToAction("Index",   "Pengaduan")
            : RedirectToAction("Riwayat", "Pengaduan");
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
