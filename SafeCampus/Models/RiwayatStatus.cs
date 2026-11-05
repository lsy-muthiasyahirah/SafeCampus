
using System;
using System.ComponentModel.DataAnnotations;

namespace SafeCampus.Models
{
    public class RiwayatStatus
    {
        [Key]
        public int IdRiwayat { get; set; }

        public int IdPengaduan { get; set; }

        public string StatusSebelumnya { get; set; } = "";

        public string StatusSesudahnya { get; set; } = "";

        public string DiubahOleh { get; set; } = "";

        public DateTime TanggalPerubahan { get; set; }
            = DateTime.Now;

        public string? Keterangan { get; set; }

        public Pengaduan? Pengaduan { get; set; }
    }
}
