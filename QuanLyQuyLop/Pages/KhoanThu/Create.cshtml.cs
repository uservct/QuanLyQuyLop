using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QuanLyQuyLop.Pages.KhoanThu
{
    public class CreateModel : PageModel
    {
        public KhoanThuInfo khoanThuInfo = new KhoanThuInfo();
        public string errorMessage = ""; //hiển thị thông báo lỗi khi người dùng ko nhập đủ thông tin
        public void OnGet()
        {
            khoanThuInfo.NgayTao = DateTime.Now.ToString("yyyy-MM-dd");
        }
        public void OnPost() //xử lý http method: POST xảy ra khi người dùng click Submit button
        {
            //property Request biểu diễn các thông tin do user gửi yêu cầu (request) đến Server
            khoanThuInfo.TenKhoanThu = Request.Form["tenkhoanthu"];
            khoanThuInfo.NgayTao = Request.Form["ngaytao"];
            khoanThuInfo.HanNop = Request.Form["hannop"];
            khoanThuInfo.GhiChu = Request.Form["ghichu"];
            string soTienStr = Request.Form["sotien"];

            //check all fields are filled
            if (string.IsNullOrEmpty(khoanThuInfo.TenKhoanThu) || string.IsNullOrEmpty(khoanThuInfo.HanNop))
            {
                errorMessage = "Vui lòng điền đủ thông tin";
                return;
            }
            // check soTienStr có phải int không, ép từ string sang int
            // tryparse kiểm tra xem có thể chuyển đổi từ string sang int không
            if (!int.TryParse(Request.Form["sotien"], out int soTienInt) || soTienInt <= 0)
            {
                //
                errorMessage = "Số tiền không hợp lệ";
                return;
            }

            khoanThuInfo.SoTien = soTienInt;
            //if ok, save new kt to database
            try
            {
                string connectionString = "Data Source=localhost\\sqlexpress;Initial Catalog=QuanLyQuyLop;" +
                    "Integrated Security=True;Pooling=False;TrustServerCertificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO KhoanThu" + "(TenKhoanThu, SoTien, NgayTao, HanNop, GhiChu) VALUES " + "(@tenkhoanthu, @sotien, @ngaytao, @hannop, @ghichu);";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@tenkhoanthu", khoanThuInfo.TenKhoanThu);
                        command.Parameters.AddWithValue("@sotien", khoanThuInfo.SoTien);
                        command.Parameters.AddWithValue("@ngaytao", khoanThuInfo.NgayTao);
                        command.Parameters.AddWithValue("@hannop", khoanThuInfo.HanNop);
                        command.Parameters.AddWithValue("@ghichu", khoanThuInfo.GhiChu);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
            //clear info for next input
            Response.Redirect("/KhoanThu");
        }
    }
}

