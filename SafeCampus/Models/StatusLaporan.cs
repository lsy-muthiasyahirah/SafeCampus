namespace SafeCampus.Models;

// Daftar status HARUS sama dengan yang ada di flowchart. Jika berubah, kabari tim.
public enum StatusLaporan
{
    MenungguValidasi,
    Valid,
    ButuhInformasi,
    Diproses,
    Selesai,
    Ditolak
}

public static class StatusLaporanExtensions
{
    public static string Label(this StatusLaporan s) => s switch
    {
        StatusLaporan.MenungguValidasi => "Menunggu Validasi",
        StatusLaporan.Valid => "Valid",
        StatusLaporan.ButuhInformasi => "Butuh Informasi Tambahan",
        StatusLaporan.Diproses => "Sedang Diproses",
        StatusLaporan.Selesai => "Selesai",
        StatusLaporan.Ditolak => "Ditolak",
        _ => s.ToString()
    };

    // Dipakai di view untuk memilih warna badge (lihat safecampus.css)
    public static string CssClass(this StatusLaporan s)
        => "sc-badge sc-badge-" + s.ToString().ToLower();
}
namespace SafeCampus.Models
{
    public class StatusLaporan
    {
    }
}
