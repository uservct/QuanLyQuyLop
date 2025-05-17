using System.Data.SqlClient;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QuanLyQuyLop.Pages.ThanhVien
{
    public class CreateModel : PageModel
    {
        public ThanhVienInfo thanhVienInfo = new ThanhVienInfo();
        public string errorMessage = ""; //hiển thị thông báo lỗi khi người dùng ko nhập đủ thông tin
        public void OnGet()
        {
        }
        public void OnPost() //xử lý http method: POST xảy ra khi người dùng click Submit button
        {
            //property Request biểu diễn các thông tin do user gửi yêu cầu (request) đến Server
            thanhVienInfo.MSV = Request.Form["msv"];
            thanhVienInfo.HoTen = Request.Form["hoten"];
            thanhVienInfo.GhiChu = Request.Form["ghichu"];
            //check all fields are filled
            if (string.IsNullOrEmpty(thanhVienInfo.MSV) || string.IsNullOrEmpty(thanhVienInfo.HoTen))
            {
                errorMessage = "Vui lòng điền đủ Mã Sinh Viên và Họ Tên";
                return;
            }
            //if ok, save new tv to database
            try
            {
                string connectionString = "Data Source=localhost\\sqlexpress;Initial Catalog=QuanLyQuyLop;" +
                    "Integrated Security=True;Pooling=False;TrustServerCertificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO ThanhVien" + "(MSV, HoTen, GhiChu) VALUES " + "(@msv, @hoten, @ghichu);";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@msv", thanhVienInfo.MSV);
                        command.Parameters.AddWithValue("@hoten", thanhVienInfo.HoTen);
                        command.Parameters.AddWithValue("@ghichu", thanhVienInfo.GhiChu);

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
            thanhVienInfo.MSV = "";
            thanhVienInfo.HoTen = "";
            thanhVienInfo.GhiChu = "";
            Response.Redirect("/ThanhVien");
        }
    }
}
