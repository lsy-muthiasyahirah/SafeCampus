
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
