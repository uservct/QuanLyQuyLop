using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QuanLyQuyLop.Pages.KhoanThu
{
    public class EditModel : PageModel
    {
        public KhoanThuInfo khoanThuInfo = new KhoanThuInfo(); //model chứa thông tin 1 khoan thu
        public string errorMessage = ""; //báo lỗi nhập thiếu thông tin
        public void OnGet()
        {
            String id = Request.Query["id"]; //lấy id trên url của request
            try
            {
                string connectionString = "Data Source=localhost\\sqlexpress;Initial Catalog=QuanLyQuyLop;" +
                    "Integrated Security=True;Pooling=False;TrustServerCertificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "Select * from KhoanThu where id = @id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("id", id); //đưa id các tv đc truyền qua url
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                khoanThuInfo.Id = "" + reader.GetInt32(0);
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
        public void OnPost() //xử lý http method: POST khi người dùng bấm submit
        {
            khoanThuInfo.Id = Request.Form["id"];
            khoanThuInfo.TenKhoanThu = Request.Form["tenkhoanthu"];
            khoanThuInfo.SoTien = int.TryParse(Request.Form["sotien"], out int soTien) ? soTien : 0;
            // khoanThuInfo.NgayTao = Request.Form["ngaytao"];
            khoanThuInfo.HanNop = Request.Form["hannop"];
            khoanThuInfo.GhiChu = Request.Form["ghichu"];
            //check all fields are filled
            if (string.IsNullOrEmpty(khoanThuInfo.TenKhoanThu) || string.IsNullOrEmpty(khoanThuInfo.HanNop))
            {
                errorMessage = "Vui lòng điền đủ thông tin";
                return;
            }
            //if ok,update tv to database
            try
            {
                string connectionString = "Data Source=localhost\\sqlexpress;Initial Catalog=QuanLyQuyLop;" +
                   "Integrated Security=True;Pooling=False;TrustServerCertificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE KhoanThu " + " set TenKhoanThu = @tenkhoanthu, SoTien = @sotien, HanNop = @hannop, GhiChu = @ghichu " + "WHERE id = @id;";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@tenkhoanthu", khoanThuInfo.TenKhoanThu);
                        command.Parameters.AddWithValue("@sotien", khoanThuInfo.SoTien);
                        command.Parameters.AddWithValue("@hannop", khoanThuInfo.HanNop);
                        command.Parameters.AddWithValue("@ghichu", khoanThuInfo.GhiChu);
                        command.Parameters.AddWithValue("@id", khoanThuInfo.Id);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
            Response.Redirect("/KhoanThu/Details?id=" + khoanThuInfo.Id); //trở về trang danh sách khoan thu
        }
    }
}

