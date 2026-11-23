using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeCampus.Models;

[Table("jurusan")]
public class Jurusan
{
    [Key, Column("id_jurusan")]
    public int Id { get; set; }

    [Column("nama_jurusan"), MaxLength(100), Required]
    public string NamaJurusan { get; set; } = string.Empty;

    // Navigasi balik
    public List<User> Users { get; set; } = new();
}
