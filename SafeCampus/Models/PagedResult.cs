namespace SafeCampus.Models;

// GENERIC CLASS: T bisa diganti tipe apa saja (RiwayatItem, dll.)
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public StatusLaporan? FilterStatus { get; set; }
    public int Halaman { get; set; } = 1;
    public int TotalHalaman { get; set; } = 1;
    public int TotalData { get; set; }

    public bool AdaSebelumnya => Halaman > 1;
    public bool AdaBerikutnya => Halaman < TotalHalaman;
}