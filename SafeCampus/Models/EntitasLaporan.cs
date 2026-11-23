using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeCampus.Models;

// ABSTRACT CLASS: Kerangka dasar untuk entitas laporan di SafeCampus
public abstract class EntitasLaporan
{
    [Column("tanggal_pengaduan")]
    public DateTime TanggalPengaduan { get; set; } = DateTime.Now;

    [Column("lokasi_kejadian"), MaxLength(255)]
    public string LokasiKejadian { get; set; } = string.Empty;

    [Column("deskripsi", TypeName = "text")]
    public string Deskripsi { get; set; } = string.Empty;

    // ABSTRACT METHOD: Wajib diimplementasikan (override) oleh class turunan
    public abstract string BuatRingkasanLaporan();

    // VIRTUAL METHOD: Memiliki implementasi default namun dapat di-override oleh class turunan
    public virtual bool ApakahLaporanBaru()
        => (DateTime.Now - TanggalPengaduan).TotalHours <= 24;
}
