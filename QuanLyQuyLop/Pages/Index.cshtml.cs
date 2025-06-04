using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace QuanLyQuyLop.Pages
{
    public class ChiTietThuReport
    {
        public string TenKhoanThu { get; set; }
        public int SoTien { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime HanNop { get; set; }
        public int SoNguoiDaDong { get; set; }
        public int ThanhTien => SoTien * SoNguoiDaDong;
    }
    public class KhoanChiItem
    {
        public string TenKhoanChi { get; set; }
        public int SoTien { get; set; }
        public DateTime NgayChi { get; set; }
        public string GhiChu { get; set; }
    }

    public class KhoanChuaNop
    {
        public string TenKhoanThu { get; set; }
        public int SoTien { get; set; }
    }

    public class ThanhVienChuaNop
    {
        public int Id { get; set; }
        public string MSV { get; set; }
        public string HoTen { get; set; }
        public List<string> khoanChuaNop { get; set; } = new List<string>();
        public int TongSoTien { get; set; }
    }
    public class IndexModel : PageModel
    {
        
        [BindProperty(SupportsGet = true)]  //	Dùng để bind từ query string khi GET
        public DateTime? FromDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? ToDate { get; set; }
        public int TongThu { get; set; }
        public int TongChi { get; set; }
        public int SoDu => TongThu - TongChi;

        public List<ChiTietThuReport> ChiTietThuReport = new List<ChiTietThuReport>();
        public List<KhoanChiItem> khoanChiItem = new List<KhoanChiItem>();
        public List<ThanhVienChuaNop> thanhVienChuaNop = new List<ThanhVienChuaNop>();
        public void OnGet()
        {
            string connectionString = "Data Source=localhost\\sqlexpress;Initial Catalog=QuanLyQuyLop;" +
                "Integrated Security=True;Pooling=False;TrustServerCertificate=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Tổng thu
                string sqlThu = @"SELECT SUM(kt.SoTien)
                                    FROM ChiTietThu ctt
                                    JOIN KhoanThu kt ON ctt.KhoanThuId = kt.Id
                                    WHERE ctt.DaNop = 1";
                if (FromDate.HasValue && ToDate.HasValue)
                {
                    sqlThu += " AND kt.NgayTao BETWEEN @fromDate AND @toDate";
                }
                using (SqlCommand cmd = new SqlCommand(sqlThu, connection))
                {
                    if (FromDate.HasValue && ToDate.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@fromDate", FromDate.Value);
                        cmd.Parameters.AddWithValue("@toDate", ToDate.Value);
                    }
                    object resultThu = cmd.ExecuteScalar();
                    TongThu = (resultThu != DBNull.Value && resultThu != null) ? Convert.ToInt32(resultThu) : 0;
                }

                // Tổng chi
                string sqlChi = "SELECT SUM(SoTien) FROM KhoanChi";
                if (FromDate.HasValue && ToDate.HasValue)
                {
                    sqlChi += " WHERE NgayChi BETWEEN @fromDate AND @toDate";
                }
                using (SqlCommand cmd = new SqlCommand(sqlChi, connection))
                {
                    if (FromDate.HasValue && ToDate.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@fromDate", FromDate.Value);
                        cmd.Parameters.AddWithValue("@toDate", ToDate.Value);
                    }
                    object resultChi = cmd.ExecuteScalar();
                    TongChi = (resultChi != DBNull.Value && resultChi != null) ? Convert.ToInt32(resultChi) : 0;
                }

                // Lấy chi tiết thu
                string sql = @"SELECT kt.Id, kt.TenKhoanThu, kt.SoTien, kt.NgayTao, kt.HanNop,
                                COUNT(CASE WHEN ctt.DaNop = 1 THEN 1 END) AS SoNguoiDaDong
                                FROM KhoanThu kt
                                JOIN ChiTietThu ctt ON kt.Id = ctt.KhoanThuId
                                ";
                if (FromDate.HasValue && ToDate.HasValue)
                {
                    sql += " WHERE kt.NgayTao BETWEEN @fromDate AND @toDate";
                }

                sql += " GROUP BY kt.Id, kt.TenKhoanThu, kt.SoTien, kt.NgayTao, kt.HanNop";

                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    if (FromDate.HasValue && ToDate.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@fromDate", FromDate.Value);
                        cmd.Parameters.AddWithValue("@toDate", ToDate.Value);
                    }
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ChiTietThuReport report = new ChiTietThuReport();
                            report.TenKhoanThu = reader.GetString(1);
                            report.SoTien = reader.GetInt32(2);
                            report.NgayTao = reader.GetDateTime(3);
                            report.HanNop = reader.GetDateTime(4);
                            report.SoNguoiDaDong = reader.GetInt32(5);
                            ChiTietThuReport.Add(report);
                        }
                    }
                }
                // Lấy danh sách khoản chi
                string sqlKhoanChi = "SELECT TenKhoanChi, SoTien, NgayChi, GhiChu FROM KhoanChi";
                if (FromDate.HasValue && ToDate.HasValue)
                {
                    sqlKhoanChi += " WHERE NgayChi BETWEEN @fromDate AND @toDate";
                }

                using (SqlCommand cmd = new SqlCommand(sqlKhoanChi, connection))
                {
                    if (FromDate.HasValue && ToDate.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@fromDate", FromDate.Value);
                        cmd.Parameters.AddWithValue("@toDate", ToDate.Value);
                    }
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            KhoanChiItem khoanChi = new KhoanChiItem();
                            khoanChi.TenKhoanChi = reader.GetString(0);
                            khoanChi.SoTien = reader.GetInt32(1);
                            khoanChi.NgayChi = reader.GetDateTime(2);
                            khoanChi.GhiChu = reader.IsDBNull(3) ? "" : reader.GetString(3);
                            khoanChiItem.Add(khoanChi);
                        }
                    }
                }

                // Lấy danh sách thành viên chưa nộp
                string sqlChuaNop = @"SELECT tv.Id,tv.MSV, tv.HoTen, kt.TenKhoanThu, kt.SoTien
                            FROM ThanhVien tv
                            JOIN ChiTietThu ctt ON tv.Id = ctt.ThanhVienId
                            JOIN KhoanThu kt ON ctt.KhoanThuId = kt.Id
                            WHERE ctt.DaNop = 0
                            ";

                if (FromDate.HasValue && ToDate.HasValue)
                {
                    sqlChuaNop += " AND kt.NgayTao BETWEEN @fromDate AND @toDate";
                }

                sqlChuaNop += " ORDER BY tv.HoTen";

                using (SqlCommand cmd = new SqlCommand(sqlChuaNop, connection))
                {
                    if (FromDate.HasValue && ToDate.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@fromDate", FromDate.Value);
                        cmd.Parameters.AddWithValue("@toDate", ToDate.Value);
                    }
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        var thanhVienDict = new Dictionary<int, ThanhVienChuaNop>();

                        while (reader.Read())
                        {
                            int Id = reader.GetInt32(0);
                            string MSV = reader.GetString(1);
                            string HoTen = reader.GetString(2);
                            string TenKhoanThu = reader.GetString(3);
                            int SoTien = reader.GetInt32(4);

                            if (!thanhVienDict.ContainsKey(Id))
                            {
                                thanhVienDict[Id] = new ThanhVienChuaNop
                                {
                                    Id = Id,
                                    MSV = MSV,
                                    HoTen = HoTen
                                };
                            }

                            var tv = thanhVienDict[Id];
                            tv.khoanChuaNop.Add($"{TenKhoanThu} - {SoTien:N0} VNĐ");
                            tv.TongSoTien += SoTien;
                        }
                        // Sau vòng lặp, gán danh sách
                        thanhVienChuaNop = thanhVienDict.Values.ToList();
                    }
                }
            }
        }
    }
}