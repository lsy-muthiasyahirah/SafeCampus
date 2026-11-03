using Microsoft.EntityFrameworkCore;

namespace SafeCampus.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Mendaftarkan model User agar menjadi tabel bernama 'Users' di database
        public DbSet<User> Users { get; set; }
        public DbSet<Jurusan> Jurusan { get; set; }
    }
}