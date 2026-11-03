using System.ComponentModel.DataAnnotations;

namespace SafeCampus.Models
{
    public class Jurusan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string NamaJurusan { get; set; }

        // Relasi agar satu Fakultas bisa memiliki banyak User
        public ICollection<User> Users { get; set; }
    }
}