using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using QuanLyQuyLop.Pages.KhoanThu;

namespace QuanLyQuyLop.Pages.KhoanThu
{
    public class KhoanThuInfo
    {
        public string Id { get; set; }
        public string TenKhoanThu { get; set; }
        public int SoTien { get; set; }
        public string NgayTao { get; set; }
        public string HanNop { get; set; }
        public string GhiChu { get; set; }
    }
    public class IndexModel : PageModel
    {
        public List<KhoanThuInfo> listKhoanThu = new List<KhoanThuInfo>();
        public void OnGet(string? searchKT)
        {
            try
            {
                string connectionString = "Data Source=localhost\\sqlexpress;Initial Catalog=QuanLyQuyLop;" +
                    "Integrated Security=True;Pooling=False;TrustServerCertificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "Select * from KhoanThu";
                    if (!string.IsNullOrEmpty(searchKT))
                    {
                        sql += " WHERE TenKhoanThu LIKE @search";
                    }
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        if (!string.IsNullOrEmpty(searchKT))
                        {
                            command.Parameters.AddWithValue("@search", "%" + searchKT + "%");
                        }
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                KhoanThuInfo khoanThuInfo = new KhoanThuInfo();
                                khoanThuInfo.Id = "" + reader.GetInt32(0);
                                khoanThuInfo.TenKhoanThu = reader.GetString(1);
                                khoanThuInfo.SoTien = reader.GetInt32(2);
                                khoanThuInfo.NgayTao = reader.GetDateTime(3).ToString("dd-MM-yyyy");
                                khoanThuInfo.HanNop = reader.GetDateTime(4).ToString("dd-MM-yyyy");
                                khoanThuInfo.GhiChu = reader.IsDBNull(5) ? "" : reader.GetString(5);
                                listKhoanThu.Add(khoanThuInfo);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Handle SQL exceptions
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}