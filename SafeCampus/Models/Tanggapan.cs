using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeCampus.Models;

[Table("tanggapan")]
public class Tanggapan
{
    [Key, Column("id_tanggapan")]
    public int IdTanggapan { get; set; }

    [Column("id_pengaduan")]
    public int IdPengaduan { get; set; }

    [ForeignKey(nameof(IdPengaduan))]
    public Pengaduan? Pengaduan { get; set; }

    [Column("id_user")]
    public int IdUser { get; set; }

    [ForeignKey(nameof(IdUser))]
    public User? User { get; set; }

    // "mahasiswa" atau "admin" — untuk tampilan label pengirim
    [Column("pengirim"), MaxLength(20)]
    public string Pengirim { get; set; } = string.Empty;

    [Column("isi_tanggapan", TypeName = "text"), Required]
    public string IsiTanggapan { get; set; } = string.Empty;

    [Column("tanggal_tanggapan")]
    public DateTime TanggalTanggapan { get; set; } = DateTime.Now;
}
