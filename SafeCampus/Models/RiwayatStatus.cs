using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeCampus.Models;

[Table("riwayat_status")]
public class RiwayatStatus
{
    [Key, Column("id_riwayat")]
    public int IdRiwayat { get; set; }

    [Column("id_pengaduan")]
    public int IdPengaduan { get; set; }

    [ForeignKey(nameof(IdPengaduan))]
    public Pengaduan? Pengaduan { get; set; }

    [Column("status_sebelumnya"), MaxLength(50)]
    public string StatusSebelumnya { get; set; } = string.Empty;

    [Column("status_sesudahnya"), MaxLength(50)]
    public string StatusSesudahnya { get; set; } = string.Empty;

    [Column("diubah_oleh"), MaxLength(100)]
    public string DiubahOleh { get; set; } = string.Empty;

    [Column("tanggal_perubahan")]
    public DateTime TanggalPerubahan { get; set; } = DateTime.Now;

    [Column("keterangan", TypeName = "text")]
    public string? Keterangan { get; set; }
}
