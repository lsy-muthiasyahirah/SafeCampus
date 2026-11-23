using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeCampus.Models;

[Table("pengaduan")]
public class Pengaduan : EntitasLaporan
{
    [Key, Column("id_pengaduan")]
    public int IdPengaduan { get; set; }

    [Column("id_pelapor")]
    public int IdPelapor { get; set; }

    [ForeignKey(nameof(IdPelapor))]
    public User? Pelapor { get; set; }

    [Column("id_kategori")]
    public int IdKategori { get; set; }

    [ForeignKey(nameof(IdKategori))]
    public Kategori? Kategori { get; set; }

    [Column("nama_korban"), MaxLength(100)]
    public string NamaKorban { get; set; } = string.Empty;

    [Column("npm_korban"), MaxLength(20)]
    public string NpmKorban { get; set; } = string.Empty;

    [Column("tanggal_kejadian", TypeName = "date")]
    public DateTime TanggalKejadian { get; set; }

    // ENCAPSULATION: status tidak bisa diubah sembarangan dari luar class.
    [Column("status")]
    public StatusLaporan Status { get; private set; } = StatusLaporan.MenungguValidasi;

    // Diisi saat admin meminta info tambahan
    [Column("batas_waktu_balasan")]
    public DateTime? BatasWaktuBalasan { get; set; }

    // Navigasi
    public List<BuktiPengaduan> DaftarBukti { get; set; } = new();
    public List<Tanggapan>      DaftarTanggapan { get; set; } = new();
    public List<RiwayatStatus>  DaftarRiwayat   { get; set; } = new();
    public List<Notifikasi>     DaftarNotifikasi { get; set; } = new();

    // Perubahan status dipusatkan di sini.
    public void UbahStatus(StatusLaporan statusBaru) => Status = statusBaru;

    public bool DimilikiOleh(int idUser) => IdPelapor == idUser;

    // Aturan akses: pemilik laporan atau admin.
    public bool BisaDilihatOleh(int idUser, bool isAdmin) => isAdmin || DimilikiOleh(idUser);

    public bool SudahKedaluwarsa()
        => Status == StatusLaporan.ButuhInformasi
           && BatasWaktuBalasan.HasValue
           && BatasWaktuBalasan.Value < DateTime.Now;

    // Implementasi abstract method dari EntitasLaporan
    public override string BuatRingkasanLaporan()
        => $"[Pengaduan #{IdPengaduan}] {LokasiKejadian} ({TanggalKejadian:dd/MM/yyyy}) - Status: {Status.Label()}";
}
