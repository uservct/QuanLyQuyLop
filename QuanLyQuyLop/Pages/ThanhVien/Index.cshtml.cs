using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QuanLyQuyLop.Pages.ThanhVien
{
    public class ThanhVienInfo
    {
        public string Id { get; set; }
        public string MSV { get; set; }
        public string HoTen { get; set; }
        public string GhiChu { get; set; }

    }
    public class IndexModel : PageModel
    {
        public List<ThanhVienInfo> listThanhVien = new List<ThanhVienInfo>();
        public void OnGet(string? searchTV)
        {
            try
            {
                string connectionString = "Data Source=localhost\\sqlexpress;Initial Catalog=QuanLyQuyLop;" +
                    "Integrated Security=True;Pooling=False;TrustServerCertificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "Select * from ThanhVien";
                    if (!string.IsNullOrEmpty(searchTV))
                    {
                        sql += " WHERE HoTen LIKE @search OR MSV LIKE @search";
                    }
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        if (!string.IsNullOrEmpty(searchTV))
                        {
                            command.Parameters.AddWithValue("@search", "%" + searchTV + "%");
                        }
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ThanhVienInfo thanhVienInfo = new ThanhVienInfo();
                                thanhVienInfo.Id = "" + reader.GetInt32(0);
                                thanhVienInfo.MSV = reader.GetString(1);
                                thanhVienInfo.HoTen = reader.GetString(2);
                                thanhVienInfo.GhiChu = reader.IsDBNull(3) ? "" : reader.GetString(3);
                                listThanhVien.Add(thanhVienInfo);
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

