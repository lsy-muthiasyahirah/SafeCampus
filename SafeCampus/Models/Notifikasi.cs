
using System;
using System.ComponentModel.DataAnnotations;

namespace SafeCampus.Models
{
    public class Notifikasi
    {
        [Key]
        public int IdNotifikasi { get; set; }

        public int IdUser { get; set; }

        public int IdPengaduan { get; set; }

        public string Pesan { get; set; } = "";

        public DateTime TanggalNotifikasi { get; set; }
            = DateTime.Now;

        public bool SudahDibaca { get; set; } = false;

        public Pengaduan? Pengaduan { get; set; }
    }
}
