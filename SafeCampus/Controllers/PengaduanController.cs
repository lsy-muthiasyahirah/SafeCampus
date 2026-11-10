using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SafeCampus.Data;
using SafeCampus.Models;
using SafeCampus.Services;
using SafeCampus.ViewModels;

namespace SafeCampus.Controllers;

public class PengaduanController : Controller
{
    private const int UkuranHalaman = 8;

    private readonly AppDbContext _db;
    private readonly IFileUploadService _upload;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;
    private readonly ILogger<PengaduanController> _logger;

    public PengaduanController(AppDbContext db, IFileUploadService upload,
        IWebHostEnvironment env, IConfiguration config, ILogger<PengaduanController> logger)
    {
        _db = db;
        _upload = upload;
        _env = env;
        _config = config;
        _logger = logger;
    }

    // Id user diambil dari claim yang dibuat modul login (Muti).
    // Saat Development dan login belum jadi, boleh pakai "Dev:UserId" di appsettings.Development.json.
    private int? AmbilIdUser()
    {
        var klaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(klaim, out var id)) return id;

        if (_env.IsDevelopment() && int.TryParse(_config["Dev:UserId"], out var devId))
            return devId;

        return null;
    }

    private bool IsAdmin => User.IsInRole("Admin");

    // ---------- BUAT PENGADUAN ----------

    [HttpGet]
    public async Task<IActionResult> Buat()
    {
        var idUser = AmbilIdUser();
        if (idUser is null) return RedirectToAction("Login", "Account");

        var vm = new BuatPengaduanViewModel();
        await IsiDataTampilan(vm, idUser.Value);
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken, RequestSizeLimit(30_000_000)]
    public async Task<IActionResult> Buat(BuatPengaduanViewModel vm)
    {
        var idUser = AmbilIdUser();
        if (idUser is null) return RedirectToAction("Login", "Account");

        var files = vm.Bukti?.Where(f => f.Length > 0).ToList() ?? new List<IFormFile>();

        foreach (var pesan in await _upload.ValidasiAsync(files))
            ModelState.AddModelError(nameof(BuatPengaduanViewModel.Bukti), pesan);

        if (vm.IdKategori is int idKat && !await _db.Kategori.AnyAsync(k => k.IdKategori == idKat))
            ModelState.AddModelError(nameof(BuatPengaduanViewModel.IdKategori), "Kategori tidak valid.");

        if (!ModelState.IsValid)
        {
            await IsiDataTampilan(vm, idUser.Value);
            return View(vm);
        }

        List<BuktiPengaduan> bukti = new();
        try
        {
            bukti = await _upload.SimpanAsync(files);

            var pengaduan = new Pengaduan
            {
                IdPelapor = idUser.Value,
                IdKategori = vm.IdKategori!.Value,
                NamaKorban = vm.NamaKorban.Trim(),
                NpmKorban = vm.NpmKorban.Trim(),
                TanggalKejadian = vm.TanggalKejadian!.Value.Date,
                TanggalPengaduan = DateTime.Now,
                LokasiKejadian = vm.LokasiKejadian.Trim(),
                Deskripsi = vm.Deskripsi.Trim(),
                DaftarBukti = bukti
            };

            _db.Pengaduan.Add(pengaduan);
            await _db.SaveChangesAsync();

            TempData["Sukses"] = "Pengaduan berhasil dikirim. Tim Satgas akan memeriksanya.";
            return RedirectToAction(nameof(Detail), new { id = pengaduan.IdPengaduan });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Gagal menyimpan pengaduan");
            _upload.Hapus(bukti.Select(b => b.PathFile)); // jangan tinggalkan file yatim
            ModelState.AddModelError(string.Empty, "Pengaduan gagal disimpan. Silakan coba lagi.");
            await IsiDataTampilan(vm, idUser.Value);
            return View(vm);
        }
    }

    // ---------- RIWAYAT ----------

    [HttpGet]
    public async Task<IActionResult> Riwayat(StatusLaporan? status, int halaman = 1)
    {
        var idUser = AmbilIdUser();
        if (idUser is null) return RedirectToAction("Login", "Account");

        var query = _db.Pengaduan.AsNoTracking().Where(p => p.IdPelapor == idUser.Value);
        if (status.HasValue) query = query.Where(p => p.Status == status.Value);

        var total = await query.CountAsync();
        var totalHalaman = Math.Max(1, (int)Math.Ceiling(total / (double)UkuranHalaman));
        halaman = Math.Clamp(halaman, 1, totalHalaman);

        var items = await query
            .OrderByDescending(p => p.TanggalPengaduan)
            .Skip((halaman - 1) * UkuranHalaman)
            .Take(UkuranHalaman)
            .Select(p => new RiwayatItem
            {
                IdPengaduan = p.IdPengaduan,
                NamaKategori = p.Kategori!.NamaKategori,
                LokasiKejadian = p.LokasiKejadian,
                TanggalKejadian = p.TanggalKejadian,
                TanggalPengaduan = p.TanggalPengaduan,
                Status = p.Status,
                JumlahBukti = p.DaftarBukti.Count
            })
            .ToListAsync();

        return View(new PagedResult<RiwayatItem>
        {
            Items = items,
            FilterStatus = status,
            Halaman = halaman,
            TotalHalaman = totalHalaman,
            TotalData = total
        });
    }

    // ---------- DETAIL ----------

    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        var idUser = AmbilIdUser();
        if (idUser is null) return RedirectToAction("Login", "Account");

        var pengaduan = await _db.Pengaduan.AsNoTracking()
            .Include(p => p.Kategori)
            .Include(p => p.Pelapor)
            .Include(p => p.DaftarBukti)
            .FirstOrDefaultAsync(p => p.IdPengaduan == id);

        // NotFound (bukan Forbid) supaya orang lain tidak tahu laporan itu ada
        if (pengaduan is null || !pengaduan.BisaDilihatOleh(idUser.Value, IsAdmin))
            return NotFound();

        return View(pengaduan);
    }

    // ---------- UNDUH / LIHAT BUKTI (dengan cek hak akses) ----------

    [HttpGet]
    public async Task<IActionResult> Bukti(int id)
    {
        var idUser = AmbilIdUser();
        if (idUser is null) return RedirectToAction("Login", "Account");

        var bukti = await _db.BuktiPengaduan.AsNoTracking()
            .Include(b => b.Pengaduan)
            .FirstOrDefaultAsync(b => b.IdBukti == id);

        if (bukti?.Pengaduan is null || !bukti.Pengaduan.BisaDilihatOleh(idUser.Value, IsAdmin))
            return NotFound();

        var path = _upload.AmbilPathLengkap(bukti.PathFile);
        if (!System.IO.File.Exists(path)) return NotFound();

        Response.Headers["X-Content-Type-Options"] = "nosniff";
        return PhysicalFile(path, _upload.AmbilTipeKonten(bukti.PathFile));
    }

    // ---------- helper ----------

    private async Task IsiDataTampilan(BuatPengaduanViewModel vm, int idUser)
    {
        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.IdUser == idUser);
        vm.NamaPelapor = user?.Nama ?? string.Empty;
        vm.NpmPelapor = user?.Npm ?? string.Empty;

        vm.DaftarKategori = await _db.Kategori.AsNoTracking()
            .OrderBy(k => k.NamaKategori)
            .Select(k => new SelectListItem { Value = k.IdKategori.ToString(), Text = k.NamaKategori })
            .ToListAsync();
    }
}
namespace SafeCampus.Controllers
{
    public class PengaduanController
    {
﻿
using Microsoft.AspNetCore.Mvc;
using SafeCampus.Models;
using System;
using System.Collections.Generic;

namespace SafeCampus.Controllers
{
    public class PengaduanController : Controller
    {
        // DETAIL PENGADUAN
        public IActionResult Detail(int id = 1)
        {
            var pengaduan = new Pengaduan
            {
                IdPengaduan = id,
                IdUser = 1,
                Kategori = "Verbal",
                TanggalPengaduan = new DateTime(2026, 11, 1),
                LokasiKejadian = "Lingkungan Kampus",
                Deskripsi = "Pengaduan mengenai tindakan pembullyan verbal.",
                Status = "Diproses"
            };

            // RIWAYAT STATUS
            var riwayat = new List<RiwayatStatus>
            {
                new RiwayatStatus
                {
                    StatusSebelumnya = "Menunggu",
                    StatusSesudahnya = "Diproses",
                    TanggalPerubahan = new DateTime(2026, 11, 3, 11, 6, 0),
                    Keterangan = "Pengaduan sedang ditindaklanjuti oleh admin."
                },

                new RiwayatStatus
                {
                    StatusSebelumnya = "",
                    StatusSesudahnya = "Menunggu",
                    TanggalPerubahan = new DateTime(2026, 11, 1, 10, 0, 0),
                    Keterangan = "Pengaduan berhasil dikirim dan menunggu penanganan."
                }
            };

            // TANGGAPAN
            var tanggapan = new List<Tanggapan>
            {
                new Tanggapan
                {
                    Pengirim = "Admin",
                    IsiTanggapan = "Terima kasih telah melaporkan kejadian ini. Kami sedang melakukan peninjauan lebih lanjut.",
                    TanggalTanggapan = new DateTime(2026, 11, 3, 11, 6, 0)
                },

                new Tanggapan
                {
                    Pengirim = "Mahasiswa",
                    IsiTanggapan = "Baik, terima kasih atas informasinya.",
                    TanggalTanggapan = new DateTime(2026, 11, 3, 11, 30, 0)
                }
            };

            ViewBag.Riwayat = riwayat;
            ViewBag.Tanggapan = tanggapan;

            // Memanggil DetailPengaduan.cshtml
            return View("DetailPengaduan", pengaduan);
        }

        // HALAMAN TANGGAPAN
        public IActionResult Tanggapan(int id)
        {
            ViewBag.IdPengaduan = id;
            return View();
        }

        // PROSES KIRIM TANGGAPAN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult KirimTanggapan(int idPengaduan, string isiTanggapan)
        {
            if (string.IsNullOrWhiteSpace(isiTanggapan))
            {
                TempData["Error"] = "Tanggapan tidak boleh kosong.";

                return RedirectToAction("Tanggapan", new { id = idPengaduan });
            }

            // Data sementara untuk pengujian
            TempData["PesanBerhasil"] = "Tanggapan berhasil dikirim.";

            return RedirectToAction("Detail", new { id = idPengaduan });
        }
    }
}
