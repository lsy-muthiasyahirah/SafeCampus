
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SafeCampus.Models
{
    public class Pengaduan
    {
        [Key]
        public int IdPengaduan { get; set; }

        // Relasi mahasiswa
        public int IdUser { get; set; }

        // Informasi korban
        public string NamaKorban { get; set; } = "";
        public string NpmKorban { get; set; } = "";
        public string EmailKorban { get; set; } = "";
        public string JurusanKorban { get; set; } = "";

        // Informasi pengaduan
        public string Kategori { get; set; } = "";

        public DateTime TanggalPengaduan { get; set; }
            = DateTime.Now;

        public DateTime TanggalKejadian { get; set; }
            = DateTime.Now;

        public string LokasiKejadian { get; set; } = "";

        public string Deskripsi { get; set; } = "";

        public string? BuktiPendukung { get; set; }

        // Status pengaduan saat ini
        public string Status { get; set; }
            = "Menunggu";

        // Relasi riwayat status
        public ICollection<RiwayatStatus> RiwayatStatus
        { get; set; } = new List<RiwayatStatus>();

        // Relasi tanggapan
        public ICollection<Tanggapan> Tanggapan
        { get; set; } = new List<Tanggapan>();

        // Relasi notifikasi
        public ICollection<Notifikasi> Notifikasi
        { get; set; } = new List<Notifikasi>();
    }
}
