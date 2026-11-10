using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SafeCampus.Models;
using Microsoft.EntityFrameworkCore;

namespace SafeCampus.Controllers
{
    public class AccountController : Controller
    {
        // 1. Tambahkan _context dan Constructor untuk injeksi database
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 2. [HttpGet] Register yang benar (mengambil data Jurusan)
        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterViewModel
            {
                JurusanOptions = _context.Jurusan.Select(j => new SelectListItem
                {
                    Value = j.Id.ToString(),
                    Text = j.NamaJurusan
                }).ToList()
            };

            return View(model);
        }

        // 3. [HttpPost] Register yang benar (menyimpan ke database)
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newUser = new User
                {
                    Nama = model.Nama,
                    NPM = model.NPM,
                    Email = model.Email,
                    Password = model.Password,
                    JurusanId = model.JurusanId
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Pendaftaran berhasil! Silakan login.";
                return RedirectToAction("Login");
            }

            // Jika error, kembalikan opsi dropdown
            model.JurusanOptions = _context.Jurusan.Select(j => new SelectListItem
            {
                Value = j.Id.ToString(),
                Text = j.NamaJurusan
            }).ToList();

            return View(model);
        }

        // 4. [HttpGet] Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // 5. [HttpPost] Login
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