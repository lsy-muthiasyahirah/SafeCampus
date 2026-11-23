using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeCampus.Models;

[Table("kategori")]
public class Kategori
{
    [Key, Column("id_kategori")]
    public int IdKategori { get; set; }

    [Column("nama_kategori"), MaxLength(100), Required]
    public string NamaKategori { get; set; } = string.Empty;

    [Column("deskripsi", TypeName = "text")]
    public string? Deskripsi { get; set; }

    // Navigasi balik
    public List<Pengaduan> Pengaduan { get; set; } = new();
}
