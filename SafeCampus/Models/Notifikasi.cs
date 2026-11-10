using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeCampus.Models;

[Table("notifikasi")]
public class Notifikasi
{
    [Key, Column("id_notifikasi")]
    public int IdNotifikasi { get; set; }

    [Column("id_user")]
    public int IdUser { get; set; }

    [ForeignKey(nameof(IdUser))]
    public User? User { get; set; }

    [Column("id_pengaduan")]
    public int IdPengaduan { get; set; }

    [ForeignKey(nameof(IdPengaduan))]
    public Pengaduan? Pengaduan { get; set; }

    [Column("pesan", TypeName = "text")]
    public string Pesan { get; set; } = string.Empty;

    [Column("tanggal_notifikasi")]
    public DateTime TanggalNotifikasi { get; set; } = DateTime.Now;

    [Column("sudah_dibaca")]
    public bool SudahDibaca { get; set; } = false;
}
