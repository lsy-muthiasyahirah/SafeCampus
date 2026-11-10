using System.ComponentModel.DataAnnotations;
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