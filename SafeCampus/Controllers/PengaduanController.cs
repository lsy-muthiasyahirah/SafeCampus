using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SafeCampus.Models;
using SafeCampus.Services;
using System.Security.Claims;

namespace SafeCampus.Controllers;

[Authorize]
public class PengaduanController : Controller
{
    private const int UkuranHalaman = 8;

    private readonly ApplicationDbContext  _db;
    private readonly IFileUploadService    _upload;
    private readonly IWebHostEnvironment   _env;
    private readonly ILogger<PengaduanController> _logger;

    public PengaduanController(
        ApplicationDbContext db,
        IFileUploadService upload,
        IWebHostEnvironment env,
        ILogger<PengaduanController> logger)
    {
        _db     = db;
        _upload = upload;
        _env    = env;
        _logger = logger;
    }

    // Ambil IdUser dari claim cookie
    private int AmbilIdUser()
    {
        var klaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(klaim, out var id) ? id : 0;
    }

    private bool IsAdmin => User.IsInRole("admin");

    // ============================================================
    //  INDEX — Daftar semua pengaduan (admin) / redirect (mahasiswa)
    // ============================================================

    [HttpGet]
    public async Task<IActionResult> Index(StatusLaporan? status, int halaman = 1)
    {
        if (!IsAdmin)
            return RedirectToAction(nameof(Riwayat));

        var query = _db.Pengaduan
            .AsNoTracking()
            .Include(p => p.Kategori)
            .Include(p => p.Pelapor)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        var total        = await query.CountAsync();
        var totalHalaman = Math.Max(1, (int)Math.Ceiling(total / (double)UkuranHalaman));
        halaman          = Math.Clamp(halaman, 1, totalHalaman);

        var items = await query
            .OrderByDescending(p => p.TanggalPengaduan)
            .Skip((halaman - 1) * UkuranHalaman)
            .Take(UkuranHalaman)
            .Select(p => new AdminDashboardItem
            {
                IdPengaduan      = p.IdPengaduan,
                NamaPelapor      = p.Pelapor != null ? p.Pelapor.Nama : "-",
                NamaKategori     = p.Kategori != null ? p.Kategori.NamaKategori : "-",
                LokasiKejadian   = p.LokasiKejadian,
                TanggalPengaduan = p.TanggalPengaduan,
                Status           = p.Status
            })
            .ToListAsync();

        // Statistik
        var semuaPengaduan = _db.Pengaduan.AsNoTracking();
        var vm = new AdminDashboardViewModel
        {
            Items        = items,
            FilterStatus = status,
            Halaman      = halaman,
            TotalHalaman = totalHalaman,
            TotalData    = total,
            JumlahTotal    = await semuaPengaduan.CountAsync(),
            JumlahMenunggu = await semuaPengaduan.CountAsync(p => p.Status == StatusLaporan.MenungguValidasi),
            JumlahDiproses = await semuaPengaduan.CountAsync(p => p.Status == StatusLaporan.Diproses),
            JumlahSelesai  = await semuaPengaduan.CountAsync(p => p.Status == StatusLaporan.Selesai),
        };

        return View(vm);
    }

    // ============================================================
    //  RIWAYAT — daftar pengaduan milik mahasiswa yang login
    // ============================================================

    [HttpGet]
    public async Task<IActionResult> Riwayat(StatusLaporan? status, int halaman = 1)
    {
        var idUser = AmbilIdUser();

        // Admin dialihkan ke Index
        if (IsAdmin)
            return RedirectToAction(nameof(Index));

        var query = _db.Pengaduan
            .AsNoTracking()
            .Where(p => p.IdPelapor == idUser);

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        var total        = await query.CountAsync();
        var totalHalaman = Math.Max(1, (int)Math.Ceiling(total / (double)UkuranHalaman));
        halaman          = Math.Clamp(halaman, 1, totalHalaman);

        var items = await query
            .OrderByDescending(p => p.TanggalPengaduan)
            .Skip((halaman - 1) * UkuranHalaman)
            .Take(UkuranHalaman)
            .Select(p => new RiwayatItem
            {
                IdPengaduan      = p.IdPengaduan,
                NamaKategori     = p.Kategori != null ? p.Kategori.NamaKategori : "-",
                LokasiKejadian   = p.LokasiKejadian,
                TanggalKejadian  = p.TanggalKejadian,
                TanggalPengaduan = p.TanggalPengaduan,
                Status           = p.Status,
                JumlahBukti      = p.DaftarBukti.Count
            })
            .ToListAsync();

        return View(new RiwayatPengaduanViewModel
        {
            Items        = items,
            FilterStatus = status,
            Halaman      = halaman,
            TotalHalaman = totalHalaman,
            TotalData    = total
        });
    }

    // ============================================================
    //  BUAT PENGADUAN
    // ============================================================

    [HttpGet]
    public async Task<IActionResult> Buat()
    {
        if (IsAdmin)
            return RedirectToAction(nameof(Index));

        var vm = new BuatPengaduanViewModel();
        await IsiDataTampilan(vm, AmbilIdUser());
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken, RequestSizeLimit(30_000_000)]
    public async Task<IActionResult> Buat(BuatPengaduanViewModel vm)
    {
        if (IsAdmin)
            return RedirectToAction(nameof(Index));

        var idUser = AmbilIdUser();
        var files  = vm.Bukti?.Where(f => f.Length > 0).ToList() ?? new List<IFormFile>();

        foreach (var pesan in await _upload.ValidasiAsync(files))
            ModelState.AddModelError(nameof(BuatPengaduanViewModel.Bukti), pesan);

        if (vm.IdKategori is int idKat && !await _db.Kategori.AnyAsync(k => k.IdKategori == idKat))
            ModelState.AddModelError(nameof(BuatPengaduanViewModel.IdKategori), "Kategori tidak valid.");

        if (!ModelState.IsValid)
        {
            await IsiDataTampilan(vm, idUser);
            return View(vm);
        }

        List<BuktiPengaduan> bukti = new();
        try
        {
            bukti = await _upload.SimpanAsync(files);

            var pengaduan = new Pengaduan
            {
                IdPelapor        = idUser,
                IdKategori       = vm.IdKategori!.Value,
                NamaKorban       = vm.NamaKorban.Trim(),
                NpmKorban        = vm.NpmKorban.Trim(),
                TanggalKejadian  = vm.TanggalKejadian!.Value.Date,
                TanggalPengaduan = DateTime.Now,
                LokasiKejadian   = vm.LokasiKejadian.Trim(),
                Deskripsi        = vm.Deskripsi.Trim(),
                DaftarBukti      = bukti
            };

            // Catat riwayat status awal
            pengaduan.DaftarRiwayat.Add(new RiwayatStatus
            {
                StatusSesudahnya = StatusLaporan.MenungguValidasi.ToString(),
                DiubahOleh       = User.FindFirstValue(ClaimTypes.Name) ?? "Sistem",
                Keterangan       = "Pengaduan berhasil dikirim dan menunggu penanganan."
            });

            _db.Pengaduan.Add(pengaduan);
            await _db.SaveChangesAsync();

            TempData["Sukses"] = "Pengaduan berhasil dikirim. Tim Satgas akan memeriksanya.";
            return RedirectToAction(nameof(Detail), new { id = pengaduan.IdPengaduan });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Gagal menyimpan pengaduan");
            _upload.Hapus(bukti.Select(b => b.PathFile));
            ModelState.AddModelError(string.Empty, "Pengaduan gagal disimpan. Silakan coba lagi.");
            await IsiDataTampilan(vm, idUser);
            return View(vm);
        }
    }

    // ============================================================
    //  DETAIL PENGADUAN
    // ============================================================

    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        var idUser = AmbilIdUser();

        var pengaduan = await _db.Pengaduan
            .AsNoTracking()
            .Include(p => p.Kategori)
            .Include(p => p.Pelapor)
            .Include(p => p.DaftarBukti)
            .Include(p => p.DaftarTanggapan)
                .ThenInclude(t => t.User)
            .Include(p => p.DaftarRiwayat)
            .FirstOrDefaultAsync(p => p.IdPengaduan == id);

        // NotFound (bukan Forbid) agar orang lain tidak tahu laporan itu ada
        if (pengaduan is null || !pengaduan.BisaDilihatOleh(idUser, IsAdmin))
            return NotFound();

        return View(pengaduan);
    }

    // ============================================================
    //  UBAH STATUS (admin only)
    // ============================================================

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UbahStatus(int id, StatusLaporan statusBaru, string? keterangan)
    {
        var pengaduan = await _db.Pengaduan
            .Include(p => p.DaftarRiwayat)
            .FirstOrDefaultAsync(p => p.IdPengaduan == id);

        if (pengaduan is null) return NotFound();

        var statusLama = pengaduan.Status;
        pengaduan.UbahStatus(statusBaru);

        // Catat riwayat perubahan
        pengaduan.DaftarRiwayat.Add(new RiwayatStatus
        {
            StatusSebelumnya = statusLama.ToString(),
            StatusSesudahnya = statusBaru.ToString(),
            DiubahOleh       = User.FindFirstValue(ClaimTypes.Name) ?? "Admin",
            Keterangan       = keterangan
        });

        // Kirim notifikasi ke pelapor
        _db.Notifikasi.Add(new Notifikasi
        {
            IdUser    = pengaduan.IdPelapor,
            IdPengaduan = pengaduan.IdPengaduan,
            Pesan     = $"Status pengaduan #{pengaduan.IdPengaduan} diubah menjadi: {statusBaru.Label()}"
        });

        await _db.SaveChangesAsync();

        TempData["Sukses"] = $"Status pengaduan berhasil diubah menjadi \"{statusBaru.Label()}\".";
        return RedirectToAction(nameof(Detail), new { id });
    }

    // ============================================================
    //  KIRIM TANGGAPAN
    // ============================================================

    [HttpGet]
    public async Task<IActionResult> Tanggapan(int id)
    {
        var idUser = AmbilIdUser();
        var pengaduan = await _db.Pengaduan.AsNoTracking()
            .FirstOrDefaultAsync(p => p.IdPengaduan == id);

        if (pengaduan is null || !pengaduan.BisaDilihatOleh(idUser, IsAdmin))
            return NotFound();

        ViewBag.IdPengaduan = id;
        return View(new KirimTanggapanViewModel { IdPengaduan = id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> KirimTanggapan(KirimTanggapanViewModel vm)
    {
        var idUser = AmbilIdUser();
        var pengaduan = await _db.Pengaduan
            .FirstOrDefaultAsync(p => p.IdPengaduan == vm.IdPengaduan);

        if (pengaduan is null || !pengaduan.BisaDilihatOleh(idUser, IsAdmin))
            return NotFound();

        if (!ModelState.IsValid)
        {
            ViewBag.IdPengaduan = vm.IdPengaduan;
            return View("Tanggapan", vm);
        }

        var tanggapan = new Tanggapan
        {
            IdPengaduan     = vm.IdPengaduan,
            IdUser          = idUser,
            Pengirim        = IsAdmin ? "admin" : "mahasiswa",
            IsiTanggapan    = vm.IsiTanggapan.Trim(),
            TanggalTanggapan = DateTime.Now
        };

        _db.Tanggapan.Add(tanggapan);

        // Notifikasi ke pihak lain
        int penerimaNot = IsAdmin ? pengaduan.IdPelapor : 1; // 1 = id admin seed
        _db.Notifikasi.Add(new Notifikasi
        {
            IdUser      = penerimaNot,
            IdPengaduan = vm.IdPengaduan,
            Pesan       = $"Tanggapan baru pada pengaduan #{vm.IdPengaduan}."
        });

        await _db.SaveChangesAsync();

        TempData["Sukses"] = "Tanggapan berhasil dikirim.";
        return RedirectToAction(nameof(Detail), new { id = vm.IdPengaduan });
    }

    // ============================================================
    //  UNDUH / LIHAT BUKTI (dengan cek hak akses)
    // ============================================================

    [HttpGet]
    public async Task<IActionResult> Bukti(int id)
    {
        var idUser = AmbilIdUser();

        var bukti = await _db.BuktiPengaduan
            .AsNoTracking()
            .Include(b => b.Pengaduan)
            .FirstOrDefaultAsync(b => b.IdBukti == id);

        if (bukti?.Pengaduan is null || !bukti.Pengaduan.BisaDilihatOleh(idUser, IsAdmin))
            return NotFound();

        var path = _upload.AmbilPathLengkap(bukti.PathFile);
        if (!System.IO.File.Exists(path)) return NotFound();

        Response.Headers["X-Content-Type-Options"] = "nosniff";
        return PhysicalFile(path, _upload.AmbilTipeKonten(bukti.PathFile));
    }

    // ============================================================
    //  Helper — isi data tampilan form Buat
    // ============================================================

    private async Task IsiDataTampilan(BuatPengaduanViewModel vm, int idUser)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.IdUser == idUser);
        vm.NamaPelapor = user?.Nama ?? string.Empty;
        vm.NpmPelapor  = user?.Npm  ?? string.Empty;

        vm.DaftarKategori = await _db.Kategori
            .AsNoTracking()
            .OrderBy(k => k.NamaKategori)
            .Select(k => new SelectListItem
            {
                Value = k.IdKategori.ToString(),
                Text  = k.NamaKategori
            })
            .ToListAsync();
    }
}
