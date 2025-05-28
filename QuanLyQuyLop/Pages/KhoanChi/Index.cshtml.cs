using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace QuanLyQuyLop.Pages.KhoanChi
{
    public class KhoanChiInfo
    {
        public string Id { get; set; }
        public string TenKhoanChi { get; set; }
        public int SoTien { get; set; }
        public string NgayChi { get; set; }
        public string GhiChu { get; set; }
    }
    public class IndexModel : PageModel
    {
        public List<KhoanChiInfo> listKhoanChi = new List<KhoanChiInfo>();
        public void OnGet(string? searchKC)
        {
            try
            {
                string connectionString = "Data Source=localhost\\sqlexpress;Initial Catalog=QuanLyQuyLop;" +
                    "Integrated Security=True;Pooling=False;TrustServerCertificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "Select * from KhoanChi";
                    if (!string.IsNullOrEmpty(searchKC))
                    {
                        sql += " WHERE TenKhoanChi LIKE @search";
                    }
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        if (!string.IsNullOrEmpty(searchKC))
                        {
                            command.Parameters.AddWithValue("@search", "%" + searchKC + "%");
                        }
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                KhoanChiInfo khoanChiInfo = new KhoanChiInfo();
                                khoanChiInfo.Id = "" + reader.GetInt32(0);
                                khoanChiInfo.TenKhoanChi = reader.GetString(1);
                                khoanChiInfo.SoTien = reader.GetInt32(2);
                                khoanChiInfo.NgayChi = reader.GetDateTime(3).ToString("dd-MM-yyyy");
                                khoanChiInfo.GhiChu = reader.IsDBNull(4) ? "" : reader.GetString(4);
                                listKhoanChi.Add(khoanChiInfo);
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
