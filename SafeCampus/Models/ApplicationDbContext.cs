using Microsoft.EntityFrameworkCore;

namespace SafeCampus.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // ===== DbSet (semua entitas) =====
    public DbSet<User>          Users          { get; set; }
    public DbSet<Jurusan>       Jurusan        { get; set; }
    public DbSet<Kategori>      Kategori       { get; set; }
    public DbSet<Pengaduan>     Pengaduan      { get; set; }
    public DbSet<BuktiPengaduan> BuktiPengaduan { get; set; }
    public DbSet<Tanggapan>     Tanggapan      { get; set; }
    public DbSet<RiwayatStatus> RiwayatStatus  { get; set; }
    public DbSet<Notifikasi>    Notifikasi     { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ===== Seed data Jurusan =====
        modelBuilder.Entity<Jurusan>().HasData(
            new Jurusan { Id = 1, NamaJurusan = "Teknik Informatika" },
            new Jurusan { Id = 2, NamaJurusan = "Sains Data" },
            new Jurusan { Id = 3, NamaJurusan = "Manajemen Bisnis" },
            new Jurusan { Id = 4, NamaJurusan = "Manajemen Logistik" },
            new Jurusan { Id = 5, NamaJurusan = "Akuntansi" },
            new Jurusan { Id = 6, NamaJurusan = "Logistik Bisnis" },
            new Jurusan { Id = 7, NamaJurusan = "Manajemen Rekayasa" },
            new Jurusan { Id = 8, NamaJurusan = "Administrasi Logistik" },
            new Jurusan { Id = 9, NamaJurusan = "Bisnis Digital" },
            new Jurusan { Id = 10, NamaJurusan = "Manajemen Transportasi" },
            new Jurusan { Id = 11, NamaJurusan = "Manajemen" }
        );

        // ===== Seed data Kategori =====
        modelBuilder.Entity<Kategori>().HasData(
            new Kategori { IdKategori = 1, NamaKategori = "Bullying / Perundungan",      Deskripsi = "Tindakan mengintimidasi, mengejek, atau menyakiti secara berulang." },
            new Kategori { IdKategori = 2, NamaKategori = "Kekerasan Seksual",            Deskripsi = "Pelecehan atau kekerasan yang bersifat seksual." },
            new Kategori { IdKategori = 3, NamaKategori = "Diskriminasi",                 Deskripsi = "Perlakuan tidak adil berdasarkan suku, agama, gender, atau lainnya." },
            new Kategori { IdKategori = 4, NamaKategori = "Pelanggaran Akademik",         Deskripsi = "Kecurangan, plagiarisme, atau pelanggaran aturan akademik lainnya." },
            new Kategori { IdKategori = 5, NamaKategori = "Lainnya",                      Deskripsi = "Pengaduan yang tidak termasuk kategori di atas." }
        );

        // ===== Seed data Admin =====
        // Password: Admin@123 (plain-text untuk demo — di produksi pakai BCrypt)
        modelBuilder.Entity<User>().HasData(
            new User
            {
                IdUser   = 1,
                Nama     = "Administrator SafeCampus",
                Npm      = "admin",
                Email    = "admin@safecampus.ac.id",
                Password = "Admin@123",
                Role     = "admin",
                JurusanId = null
            }
        );
    }
}
