using System.ComponentModel.DataAnnotations;

namespace SafeCampus.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Nama harus diisi")]
        public string Nama { get; set; }

        [Required(ErrorMessage = "NPM harus diisi")]
        public string NPM { get; set; }

        [Required(ErrorMessage = "Email harus diisi")]
        [EmailAddress(ErrorMessage = "Format email tidak valid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password harus diisi")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Konfirmasi password harus diisi")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password dan konfirmasi tidak cocok")]
        public string ConfirmPassword { get; set; }
    }

public class LoginViewModel
    {
        [Required(ErrorMessage = "Username harus diisi")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password harus diisi")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}