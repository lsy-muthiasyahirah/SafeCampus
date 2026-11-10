using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeCampus.Models;

[Table("bukti_pengaduan")]
public class BuktiPengaduan
{
    [Key, Column("id_bukti")]
    public int IdBukti { get; set; }

    [Column("id_pengaduan")]
    public int IdPengaduan { get; set; }

    [ForeignKey(nameof(IdPengaduan))]
    public Pengaduan? Pengaduan { get; set; }

    // Nama file asli dari pelapor (untuk ditampilkan)
    [Column("nama_file"), MaxLength(255)]
    public string NamaFile { get; set; } = string.Empty;

    // Nama file di server (acak, GUID + ekstensi). BUKAN path lengkap.
    [Column("path_file"), MaxLength(255)]
    public string PathFile { get; set; } = string.Empty;

    public bool IsGambar()
    {
        var ext = Path.GetExtension(PathFile).ToLowerInvariant();
        return ext is ".jpg" or ".jpeg" or ".png";
    }
}
