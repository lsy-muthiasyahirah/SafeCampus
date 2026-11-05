using Microsoft.AspNetCore.Mvc;
using SafeCampus.Models;

namespace SafeCampus.Controllers
{
    public class AccountController : Controller
    {
        // Menampilkan halaman Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Memproses data Register
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // TODO: Tulis logika untuk menyimpan data ke database di sini
                // Contoh: dbContext.Users.Add(newUser); dbContext.SaveChanges();

                TempData["SuccessMessage"] = "Pendaftaran berhasil! Silakan login.";
                return RedirectToAction("Login");
            }
            return View(model);
        }

        // Menampilkan halaman Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Memproses data Login
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // TODO: Tulis logika validasi cek database di sini
                // Contoh jika sukses:
                // if(user.Password == model.Password) { return RedirectToAction("Index", "Home"); }

                ModelState.AddModelError(string.Empty, "Username atau Password salah.");
            }
            return View(model);
        }
    }
}