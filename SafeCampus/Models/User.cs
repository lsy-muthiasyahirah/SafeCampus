using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeCampus.Models;

[Table("users")]
public class User
{
    [Key, Column("id_user")]
    public int IdUser { get; set; }

    [Column("nama"), MaxLength(100), Required]
    public string Nama { get; set; } = string.Empty;

    // NPM dipakai sebagai username login mahasiswa
    [Column("npm"), MaxLength(20)]
    public string Npm { get; set; } = string.Empty;

    [Column("email"), MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Column("password"), MaxLength(255), Required]
    public string Password { get; set; } = string.Empty;

    // "mahasiswa" atau "admin"
    [Column("role"), MaxLength(20)]
    public string Role { get; set; } = "mahasiswa";

    // FK ke Jurusan (nullable — admin tidak perlu jurusan)
    [Column("jurusan_id")]
    public int? JurusanId { get; set; }

    [ForeignKey(nameof(JurusanId))]
    public Jurusan? Jurusan { get; set; }

    // Navigasi balik
    public List<Pengaduan> Pengaduan { get; set; } = new();
}
