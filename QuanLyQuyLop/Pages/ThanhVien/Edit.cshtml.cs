using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QuanLyQuyLop.Pages.ThanhVien
{
    public class EditModel : PageModel
    {
        public ThanhVienInfo thanhVienInfo = new ThanhVienInfo(); //model chứa thông tin 1 tv
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
                    string sql = "Select * from ThanhVien where id = @id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("id", id); //đưa id các tv đc truyền qua url
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                thanhVienInfo.Id = "" + reader.GetInt32(0);
                                thanhVienInfo.MSV = reader.GetString(1);
                                thanhVienInfo.HoTen = reader.GetString(2);
                                thanhVienInfo.GhiChu = reader.IsDBNull(3) ? "" : reader.GetString(3);
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
            thanhVienInfo.Id = Request.Form["id"];
            thanhVienInfo.MSV = Request.Form["msv"];
            thanhVienInfo.HoTen = Request.Form["hoten"];
            thanhVienInfo.GhiChu = Request.Form["ghichu"];
            //check all fields are filled
            if (string.IsNullOrEmpty(thanhVienInfo.MSV) || string.IsNullOrEmpty(thanhVienInfo.HoTen))
            {
                errorMessage = "Vui lòng điền đủ Mã Sinh Viên và Họ Tên";
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
                    string sql = "UPDATE ThanhVien " + " set MSV = @msv, HoTen = @hoten, GhiChu = @ghichu " + "WHERE id = @id;";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("msv", thanhVienInfo.MSV);
                        command.Parameters.AddWithValue("hoten", thanhVienInfo.HoTen);
                        command.Parameters.AddWithValue("ghichu", thanhVienInfo.GhiChu);
                        command.Parameters.AddWithValue("@id", thanhVienInfo.Id);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
            Response.Redirect("/ThanhVien");
        }
    }
}
