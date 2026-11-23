using SafeCampus.Models;

namespace SafeCampus.Services;

// ABSTRACTION: controller hanya tahu "interface", bukan cara penyimpanannya.
public interface IFileUploadService
{
    Task<List<string>>          ValidasiAsync(IReadOnlyCollection<IFormFile> files);
    Task<List<BuktiPengaduan>>  SimpanAsync(IReadOnlyCollection<IFormFile> files);
    string AmbilPathLengkap(string namaTersimpan);
    string AmbilTipeKonten(string namaTersimpan);
    void   Hapus(IEnumerable<string> namaTersimpan);
}
