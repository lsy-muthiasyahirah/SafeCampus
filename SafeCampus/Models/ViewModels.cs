using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SafeCampus.Models;

// ===== ViewModel Login =====
public class LoginViewModel
{
    [Required(ErrorMessage = "Username / NPM harus diisi")]
    [Display(Name = "Username / NPM")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password harus diisi")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;
}

// ===== ViewModel Register =====
public class RegisterViewModel
{
    [Required(ErrorMessage = "Nama lengkap harus diisi")]
    [Display(Name = "Nama Lengkap")]
    public string Nama { get; set; } = string.Empty;

    [Required(ErrorMessage = "NPM harus diisi")]
    [Display(Name = "NPM")]
    public string Npm { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email harus diisi")]
    [EmailAddress(ErrorMessage = "Format email tidak valid")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password harus diisi")]
    [MinLength(6, ErrorMessage = "Password minimal 6 karakter")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Konfirmasi password harus diisi")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Password dan konfirmasi tidak cocok")]
    [Display(Name = "Konfirmasi Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Jurusan wajib dipilih")]
    [Display(Name = "Jurusan")]
    public int JurusanId { get; set; }

    public IEnumerable<SelectListItem>? JurusanOptions { get; set; }
}

// ===== ViewModel Buat Pengaduan =====
public class BuatPengaduanViewModel
{
    public string NamaPelapor { get; set; } = string.Empty;
    public string NpmPelapor { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kategori wajib dipilih")]
    [Display(Name = "Kategori Pengaduan")]
    public int? IdKategori { get; set; }

    [Display(Name = "Saya adalah korban")]
    public bool SayaKorban { get; set; }

    [Required(ErrorMessage = "Nama korban wajib diisi")]
    [MaxLength(100)]
    [Display(Name = "Nama Korban")]
    public string NamaKorban { get; set; } = string.Empty;

    [Required(ErrorMessage = "NPM korban wajib diisi")]
    [MaxLength(20)]
    [Display(Name = "NPM Korban")]
    public string NpmKorban { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tanggal kejadian wajib diisi")]
    [Display(Name = "Tanggal Kejadian")]
    public DateTime? TanggalKejadian { get; set; }

    [Required(ErrorMessage = "Lokasi kejadian wajib diisi")]
    [MaxLength(255)]
    [Display(Name = "Lokasi Kejadian")]
    public string LokasiKejadian { get; set; } = string.Empty;

    [Required(ErrorMessage = "Deskripsi wajib diisi")]
    [MinLength(20, ErrorMessage = "Deskripsi minimal 20 karakter")]
    [Display(Name = "Kronologi / Deskripsi")]
    public string Deskripsi { get; set; } = string.Empty;

    [Display(Name = "Bukti Pendukung")]
    public List<IFormFile>? Bukti { get; set; }

    public List<SelectListItem> DaftarKategori { get; set; } = new();
}

// ===== ViewModel Riwayat Pengaduan =====
public class RiwayatItem
{
    public int IdPengaduan { get; set; }
    public string NamaKategori { get; set; } = string.Empty;
    public string LokasiKejadian { get; set; } = string.Empty;
    public DateTime TanggalKejadian { get; set; }
    public DateTime TanggalPengaduan { get; set; }
    public StatusLaporan Status { get; set; }
    public int JumlahBukti { get; set; }
}

// ===== ViewModel Dashboard Admin =====
public class AdminDashboardItem
{
    public int IdPengaduan { get; set; }
    public string NamaPelapor { get; set; } = string.Empty;
    public string NamaKategori { get; set; } = string.Empty;
    public string LokasiKejadian { get; set; } = string.Empty;
    public DateTime TanggalPengaduan { get; set; }
    public StatusLaporan Status { get; set; }
}

public class AdminDashboardViewModel
{
    public List<AdminDashboardItem> Items { get; set; } = new();
    public StatusLaporan? FilterStatus { get; set; }
    public int Halaman { get; set; } = 1;
    public int TotalHalaman { get; set; } = 1;
    public int TotalData { get; set; }

    public int JumlahMenunggu { get; set; }
    public int JumlahDiproses { get; set; }
    public int JumlahSelesai { get; set; }
    public int JumlahTotal { get; set; }
}

// ===== ViewModel Tanggapan =====
public class KirimTanggapanViewModel
{
    [Required]
    public int IdPengaduan { get; set; }

    [Required(ErrorMessage = "Isi tanggapan tidak boleh kosong")]
    [MinLength(5, ErrorMessage = "Tanggapan terlalu pendek")]
    [MaxLength(2000)]
    [Display(Name = "Isi Tanggapan")]
    public string IsiTanggapan { get; set; } = string.Empty;
}