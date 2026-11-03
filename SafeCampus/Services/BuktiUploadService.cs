namespace SafeCampus.Services
{
    public class BuktiUploadService
    {
    }
}
using SafeCampus.Models;

namespace SafeCampus.Services;

public class BuktiUploadService : IFileUploadService
{
    public const int MaksFile = 5;
    public const long MaksUkuranByte = 5 * 1024 * 1024; // 5 MB

    // Ekstensi yang diizinkan + tanda awal (magic bytes) isi file yang sebenarnya
    private static readonly Dictionary<string, (byte[] Signature, string ContentType)> Diizinkan = new()
    {
        [".jpg"] = (new byte[] { 0xFF, 0xD8, 0xFF }, "image/jpeg"),
        [".jpeg"] = (new byte[] { 0xFF, 0xD8, 0xFF }, "image/jpeg"),
        [".png"] = (new byte[] { 0x89, 0x50, 0x4E, 0x47 }, "image/png"),
        [".pdf"] = (new byte[] { 0x25, 0x50, 0x44, 0x46 }, "application/pdf"),
    };

    private readonly string _folder;

    public BuktiUploadService(IWebHostEnvironment env)
    {
        // Disimpan di luar wwwroot supaya TIDAK bisa diakses langsung lewat URL.
        // Semua akses harus lewat PengaduanController.Bukti (cek hak akses).
        _folder = Path.Combine(env.ContentRootPath, "App_Data", "uploads", "bukti");
        Directory.CreateDirectory(_folder);
    }

    public async Task<List<string>> ValidasiAsync(IReadOnlyCollection<IFormFile> files)
    {
        var error = new List<string>();

        if (files.Count > MaksFile)
            error.Add($"Maksimal {MaksFile} file bukti.");

        foreach (var f in files)
        {
            var ext = Path.GetExtension(f.FileName).ToLowerInvariant();

            if (!Diizinkan.TryGetValue(ext, out var aturan))
            {
                error.Add($"\"{Path.GetFileName(f.FileName)}\": tipe file tidak diizinkan (hanya JPG, PNG, PDF).");
                continue;
            }
            if (f.Length > MaksUkuranByte)
            {
                error.Add($"\"{Path.GetFileName(f.FileName)}\": ukuran melebihi 5 MB.");
                continue;
            }

            // Cek isi file, bukan hanya namanya
            var buffer = new byte[aturan.Signature.Length];
            await using var stream = f.OpenReadStream();
            var terbaca = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length));
            if (terbaca < buffer.Length || !buffer.SequenceEqual(aturan.Signature))
                error.Add($"\"{Path.GetFileName(f.FileName)}\": isi file tidak sesuai dengan tipenya.");
        }
        return error;
    }

    public async Task<List<BuktiPengaduan>> SimpanAsync(IReadOnlyCollection<IFormFile> files)
    {
        var hasil = new List<BuktiPengaduan>();
        try
        {
            foreach (var f in files)
            {
                var ext = Path.GetExtension(f.FileName).ToLowerInvariant();
                var namaTersimpan = Guid.NewGuid().ToString("N") + ext;

                await using (var fs = new FileStream(Path.Combine(_folder, namaTersimpan), FileMode.CreateNew))
                    await f.CopyToAsync(fs);

                var namaAsli = Path.GetFileName(f.FileName);
                if (namaAsli.Length > 255) namaAsli = namaAsli[^255..];

                hasil.Add(new BuktiPengaduan { NamaFile = namaAsli, PathFile = namaTersimpan });
            }
            return hasil;
        }
        catch
        {
            Hapus(hasil.Select(b => b.PathFile)); // bersihkan jika gagal di tengah jalan
            throw;
        }
    }

    public string AmbilPathLengkap(string namaTersimpan)
        => Path.Combine(_folder, Path.GetFileName(namaTersimpan)); // GetFileName mencegah path traversal

    public string AmbilTipeKonten(string namaTersimpan)
    {
        var ext = Path.GetExtension(namaTersimpan).ToLowerInvariant();
        return Diizinkan.TryGetValue(ext, out var a) ? a.ContentType : "application/octet-stream";
    }

    public void Hapus(IEnumerable<string> namaTersimpan)
    {
        foreach (var nama in namaTersimpan)
        {
            var path = AmbilPathLengkap(nama);
            if (File.Exists(path)) File.Delete(path);
        }
    }
}