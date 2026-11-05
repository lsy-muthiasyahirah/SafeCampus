using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeCampus.Models;

[Table("pengaduan")]
public class Pengaduan
{
    [Key, Column("id_pengaduan")] public int IdPengaduan { get; set; }

    [Column("id_pelapor")] public int IdPelapor { get; set; }
    [ForeignKey(nameof(IdPelapor))] public User? Pelapor { get; set; }

    [Column("id_kategori")] public int IdKategori { get; set; }
    [ForeignKey(nameof(IdKategori))] public Kategori? Kategori { get; set; }

    [Column("nama_korban"), MaxLength(100)] public string NamaKorban { get; set; } = string.Empty;
    [Column("npm_korban"), MaxLength(20)] public string NpmKorban { get; set; } = string.Empty;

    [Column("tanggal_kejadian", TypeName = "date")] public DateTime TanggalKejadian { get; set; }
    [Column("tanggal_pengaduan")] public DateTime TanggalPengaduan { get; set; } = DateTime.Now;
    [Column("lokasi_kejadian"), MaxLength(255)] public string LokasiKejadian { get; set; } = string.Empty;
    [Column("deskripsi", TypeName = "text")] public string Deskripsi { get; set; } = string.Empty;

    // ENCAPSULATION: status tidak bisa diubah sembarangan dari luar class.
    [Column("status")] public StatusLaporan Status { get; private set; } = StatusLaporan.MenungguValidasi;

    // Diisi saat admin meminta info tambahan (dikerjakan modul Shafa/Tiara)
    [Column("batas_waktu_balasan")] public DateTime? BatasWaktuBalasan { get; set; }

    public List<BuktiPengaduan> DaftarBukti { get; set; } = new();

    // Perubahan status dipusatkan di sini. Shafa bisa menambahkan pencatatan riwayat di method ini.
    public void UbahStatus(StatusLaporan statusBaru) => Status = statusBaru;

    public bool DimilikiOleh(int idUser) => IdPelapor == idUser;

    // Aturan akses: pemilik laporan atau admin.
    public bool BisaDilihatOleh(int idUser, bool isAdmin) => isAdmin || DimilikiOleh(idUser);

    public bool SudahKedaluwarsa()
        => Status == StatusLaporan.ButuhInformasi
           && BatasWaktuBalasan.HasValue
           && BatasWaktuBalasan.Value < DateTime.Now;
}
﻿
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
