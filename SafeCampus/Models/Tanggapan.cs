
using System;
using System.ComponentModel.DataAnnotations;

namespace SafeCampus.Models
{
    public class Tanggapan
    {
        [Key]
        public int IdTanggapan { get; set; }

        public int IdPengaduan { get; set; }

        public int IdUser { get; set; }

        public string Pengirim { get; set; } = "";

        public string IsiTanggapan { get; set; } = "";

        public DateTime TanggalTanggapan { get; set; }
            = DateTime.Now;

        public Pengaduan? Pengaduan { get; set; }
    }
}
