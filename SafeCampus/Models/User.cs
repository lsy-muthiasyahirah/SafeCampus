using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeCampus.Models;

// SEMENTARA: file ini MILIK MUTI (modul autentikasi).
// Hapus file ini dan pakai versi Muti begitu sudah di-push.
[Table("users")]
public class User
{
    [Key, Column("id_user")] public int IdUser { get; set; }
    [Column("nama"), MaxLength(100)] public string Nama { get; set; } = string.Empty;
    [Column("npm"), MaxLength(20)] public string Npm { get; set; } = string.Empty;
    [Column("email"), MaxLength(100)] public string Email { get; set; } = string.Empty;
    [Column("password"), MaxLength(255)] public string Password { get; set; } = string.Empty;
    [Column("role"), MaxLength(20)] public string Role { get; set; } = "mahasiswa";
}
using System.ComponentModel.DataAnnotations.Schema; // Tambahkan ini

namespace SafeCampus.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nama { get; set; }

        [Required]
        public string NPM { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public int JurusanId { get; set; }

        [ForeignKey("JurusanId")]
        public Jurusan Jurusan { get; set; }
    }
}
