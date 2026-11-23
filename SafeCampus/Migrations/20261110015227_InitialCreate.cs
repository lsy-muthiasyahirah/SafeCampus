using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SafeCampus.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "jurusan",
                columns: table => new
                {
                    id_jurusan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nama_jurusan = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jurusan", x => x.id_jurusan);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "kategori",
                columns: table => new
                {
                    id_kategori = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nama_kategori = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deskripsi = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_kategori", x => x.id_kategori);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id_user = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nama = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    npm = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    role = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    jurusan_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id_user);
                    table.ForeignKey(
                        name: "FK_users_jurusan_jurusan_id",
                        column: x => x.jurusan_id,
                        principalTable: "jurusan",
                        principalColumn: "id_jurusan");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "pengaduan",
                columns: table => new
                {
                    id_pengaduan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_pelapor = table.Column<int>(type: "int", nullable: false),
                    id_kategori = table.Column<int>(type: "int", nullable: false),
                    nama_korban = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    npm_korban = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tanggal_kejadian = table.Column<DateTime>(type: "date", nullable: false),
                    tanggal_pengaduan = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    lokasi_kejadian = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    deskripsi = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<int>(type: "int", nullable: false),
                    batas_waktu_balasan = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pengaduan", x => x.id_pengaduan);
                    table.ForeignKey(
                        name: "FK_pengaduan_kategori_id_kategori",
                        column: x => x.id_kategori,
                        principalTable: "kategori",
                        principalColumn: "id_kategori",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pengaduan_users_id_pelapor",
                        column: x => x.id_pelapor,
                        principalTable: "users",
                        principalColumn: "id_user",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "bukti_pengaduan",
                columns: table => new
                {
                    id_bukti = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_pengaduan = table.Column<int>(type: "int", nullable: false),
                    nama_file = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    path_file = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bukti_pengaduan", x => x.id_bukti);
                    table.ForeignKey(
                        name: "FK_bukti_pengaduan_pengaduan_id_pengaduan",
                        column: x => x.id_pengaduan,
                        principalTable: "pengaduan",
                        principalColumn: "id_pengaduan",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "notifikasi",
                columns: table => new
                {
                    id_notifikasi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_user = table.Column<int>(type: "int", nullable: false),
                    id_pengaduan = table.Column<int>(type: "int", nullable: false),
                    pesan = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tanggal_notifikasi = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    sudah_dibaca = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifikasi", x => x.id_notifikasi);
                    table.ForeignKey(
                        name: "FK_notifikasi_pengaduan_id_pengaduan",
                        column: x => x.id_pengaduan,
                        principalTable: "pengaduan",
                        principalColumn: "id_pengaduan",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_notifikasi_users_id_user",
                        column: x => x.id_user,
                        principalTable: "users",
                        principalColumn: "id_user",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "riwayat_status",
                columns: table => new
                {
                    id_riwayat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_pengaduan = table.Column<int>(type: "int", nullable: false),
                    status_sebelumnya = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status_sesudahnya = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    diubah_oleh = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tanggal_perubahan = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    keterangan = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_riwayat_status", x => x.id_riwayat);
                    table.ForeignKey(
                        name: "FK_riwayat_status_pengaduan_id_pengaduan",
                        column: x => x.id_pengaduan,
                        principalTable: "pengaduan",
                        principalColumn: "id_pengaduan",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tanggapan",
                columns: table => new
                {
                    id_tanggapan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_pengaduan = table.Column<int>(type: "int", nullable: false),
                    id_user = table.Column<int>(type: "int", nullable: false),
                    pengirim = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    isi_tanggapan = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tanggal_tanggapan = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tanggapan", x => x.id_tanggapan);
                    table.ForeignKey(
                        name: "FK_tanggapan_pengaduan_id_pengaduan",
                        column: x => x.id_pengaduan,
                        principalTable: "pengaduan",
                        principalColumn: "id_pengaduan",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tanggapan_users_id_user",
                        column: x => x.id_user,
                        principalTable: "users",
                        principalColumn: "id_user",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "jurusan",
                columns: new[] { "id_jurusan", "nama_jurusan" },
                values: new object[,]
                {
                    { 1, "Teknik Informatika" },
                    { 2, "Sistem Informasi" },
                    { 3, "Teknik Elektro" },
                    { 4, "Manajemen" },
                    { 5, "Akuntansi" },
                    { 6, "Ilmu Hukum" },
                    { 7, "Psikologi" }
                });

            migrationBuilder.InsertData(
                table: "kategori",
                columns: new[] { "id_kategori", "deskripsi", "nama_kategori" },
                values: new object[,]
                {
                    { 1, "Tindakan mengintimidasi, mengejek, atau menyakiti secara berulang.", "Bullying / Perundungan" },
                    { 2, "Pelecehan atau kekerasan yang bersifat seksual.", "Kekerasan Seksual" },
                    { 3, "Perlakuan tidak adil berdasarkan suku, agama, gender, atau lainnya.", "Diskriminasi" },
                    { 4, "Keluhan terkait kerusakan atau ketidaksesuaian fasilitas kampus.", "Fasilitas Kampus" },
                    { 5, "Kecurangan, plagiarisme, atau pelanggaran aturan akademik lainnya.", "Pelanggaran Akademik" },
                    { 6, "Pengaduan yang tidak termasuk kategori di atas.", "Lainnya" }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id_user", "email", "jurusan_id", "nama", "npm", "password", "role" },
                values: new object[] { 1, "admin@safecampus.ac.id", null, "Administrator SafeCampus", "admin", "Admin@123", "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_bukti_pengaduan_id_pengaduan",
                table: "bukti_pengaduan",
                column: "id_pengaduan");

            migrationBuilder.CreateIndex(
                name: "IX_notifikasi_id_pengaduan",
                table: "notifikasi",
                column: "id_pengaduan");

            migrationBuilder.CreateIndex(
                name: "IX_notifikasi_id_user",
                table: "notifikasi",
                column: "id_user");

            migrationBuilder.CreateIndex(
                name: "IX_pengaduan_id_kategori",
                table: "pengaduan",
                column: "id_kategori");

            migrationBuilder.CreateIndex(
                name: "IX_pengaduan_id_pelapor",
                table: "pengaduan",
                column: "id_pelapor");

            migrationBuilder.CreateIndex(
                name: "IX_riwayat_status_id_pengaduan",
                table: "riwayat_status",
                column: "id_pengaduan");

            migrationBuilder.CreateIndex(
                name: "IX_tanggapan_id_pengaduan",
                table: "tanggapan",
                column: "id_pengaduan");

            migrationBuilder.CreateIndex(
                name: "IX_tanggapan_id_user",
                table: "tanggapan",
                column: "id_user");

            migrationBuilder.CreateIndex(
                name: "IX_users_jurusan_id",
                table: "users",
                column: "jurusan_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bukti_pengaduan");

            migrationBuilder.DropTable(
                name: "notifikasi");

            migrationBuilder.DropTable(
                name: "riwayat_status");

            migrationBuilder.DropTable(
                name: "tanggapan");

            migrationBuilder.DropTable(
                name: "pengaduan");

            migrationBuilder.DropTable(
                name: "kategori");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "jurusan");
        }
    }
}
