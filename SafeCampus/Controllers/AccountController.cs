using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SafeCampus.Models;
using System.Security.Claims;

namespace SafeCampus.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    // ============================================================
    //  LOGIN
    // ============================================================

    [HttpGet]
    public IActionResult Login()
    {
        // Kalau sudah login, langsung redirect sesuai role
        if (User.Identity?.IsAuthenticated == true)
            return RedirectSesuaiRole();

        return View(new LoginViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // Cari user berdasarkan NPM (atau field npm = "admin" untuk admin)
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Npm == model.Username.Trim());

        if (user is null || user.Password != model.Password)
        {
            ModelState.AddModelError(string.Empty, "Username / NPM atau Password salah.");
            return View(model);
        }

        // Buat claims untuk cookie authentication
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString()),
            new Claim(ClaimTypes.Name,            user.Nama),
            new Claim("Npm",                      user.Npm),
            new Claim(ClaimTypes.Role,            user.Role),
        };

        var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = false }
        );

        return RedirectSesuaiRole();
    }

    // ============================================================
    //  LOGOUT
    // ============================================================

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    // ============================================================
    //  REGISTER (hanya untuk mahasiswa)
    // ============================================================

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectSesuaiRole();

        var model = new RegisterViewModel
        {
            JurusanOptions = AmbilOpsiJurusan()
        };
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.JurusanOptions = AmbilOpsiJurusan();
            return View(model);
        }

        // Cek NPM sudah dipakai
        bool npmSudahAda = await _context.Users
            .AnyAsync(u => u.Npm == model.Npm.Trim());

        if (npmSudahAda)
        {
            ModelState.AddModelError(nameof(model.Npm), "NPM sudah terdaftar. Silakan login.");
            model.JurusanOptions = AmbilOpsiJurusan();
            return View(model);
        }

        // Cek email sudah dipakai
        bool emailSudahAda = await _context.Users
            .AnyAsync(u => u.Email == model.Email.Trim().ToLower());

        if (emailSudahAda)
        {
            ModelState.AddModelError(nameof(model.Email), "Email sudah terdaftar.");
            model.JurusanOptions = AmbilOpsiJurusan();
            return View(model);
        }

        var newUser = new User
        {
            Nama      = model.Nama.Trim(),
            Npm       = model.Npm.Trim(),
            Email     = model.Email.Trim().ToLower(),
            Password  = model.Password,          // di produksi: gunakan BCrypt
            Role      = "mahasiswa",
            JurusanId = model.JurusanId
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Pendaftaran berhasil! Silakan login dengan NPM Anda.";
        return RedirectToAction(nameof(Login));
    }

    // ============================================================
    //  Helpers
    // ============================================================

    private IActionResult RedirectSesuaiRole()
    {
        return User.IsInRole("admin")
            ? RedirectToAction("Index",   "Pengaduan")
            : RedirectToAction("Riwayat", "Pengaduan");
    }

    private List<SelectListItem> AmbilOpsiJurusan()
        => _context.Jurusan
            .OrderBy(j => j.NamaJurusan)
            .Select(j => new SelectListItem
            {
                Value = j.Id.ToString(),
                Text  = j.NamaJurusan
            })
            .ToList();
}
