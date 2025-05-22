using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace QuanLyQuyLop.Pages.KhoanThu
{
    public class DetailsModel : PageModel
    {
        public KhoanThuInfo khoanThuInfo = new KhoanThuInfo();
        public void OnGet()
        {
            string id = Request.Query["id"];
             try
            {
                string connectionString = "Data Source=localhost\\sqlexpress;Initial Catalog=QuanLyQuyLop;" +
                    "Integrated Security=True;TrustServerCertificate=True";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM KhoanThu WHERE id = @id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                khoanThuInfo.Id = reader.GetInt32(0).ToString();
                                khoanThuInfo.TenKhoanThu = reader.GetString(1);
                                khoanThuInfo.SoTien = reader.GetInt32(2);
                                khoanThuInfo.NgayTao = reader.GetDateTime(3).ToString("yyyy-MM-dd");
                                khoanThuInfo.HanNop = reader.GetDateTime(4).ToString("yyyy-MM-dd");
                                khoanThuInfo.GhiChu = reader.IsDBNull(5) ? "" : reader.GetString(5);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
