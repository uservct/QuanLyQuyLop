using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace QuanLyQuyLop.Pages.KhoanChi
{
    public class EditModel : PageModel
    {
        public KhoanChiInfo khoanChiInfo = new KhoanChiInfo();
        public string errorMessage = ""; //hiển thị thông báo lỗi khi người dùng ko nhập đủ thông tin
        public void OnGet()
        {
            String id = Request.Query["id"]; try
            {
                string connectionString = "Data Source=localhost\\sqlexpress;Initial Catalog=QuanLyQuyLop;" +
                    "Integrated Security=True;Pooling=False;TrustServerCertificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "Select * from KhoanChi where id = @id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("id", id); //đưa id các tv đc truyền qua url
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                khoanChiInfo.Id = "" + reader.GetInt32(0);
                                khoanChiInfo.TenKhoanChi = reader.GetString(1);
                                khoanChiInfo.SoTien = reader.GetInt32(2);
                                khoanChiInfo.NgayChi = reader.GetDateTime(3).ToString("yyyy-MM-dd");
                                khoanChiInfo.GhiChu = reader.IsDBNull(4) ? "" : reader.GetString(4);
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
        public void OnPost()
        {
            khoanChiInfo.Id = Request.Form["id"];
            khoanChiInfo.TenKhoanChi = Request.Form["tenkhoanchi"];
            khoanChiInfo.SoTien = int.TryParse(Request.Form["sotien"], out int soTien) ? soTien : 0;
            khoanChiInfo.NgayChi = Request.Form["ngaychi"];
            khoanChiInfo.GhiChu = Request.Form["ghichu"];
            //check all fields are filled
            if (string.IsNullOrEmpty(khoanChiInfo.TenKhoanChi) || string.IsNullOrEmpty(khoanChiInfo.NgayChi))
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
                    string sql = "UPDATE KhoanChi " + " set TenKhoanChi = @tenkhoanchi, SoTien = @sotien, NgayChi = @ngaychi, GhiChu = @ghichu " + "WHERE id = @id;";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@tenkhoanchi", khoanChiInfo.TenKhoanChi);
                        command.Parameters.AddWithValue("@sotien", khoanChiInfo.SoTien);
                        command.Parameters.AddWithValue("@ngaychi", khoanChiInfo.NgayChi);
                        command.Parameters.AddWithValue("@ghichu", khoanChiInfo.GhiChu);
                        command.Parameters.AddWithValue("@id", khoanChiInfo.Id);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
            Response.Redirect("/KhoanChi"); //redirect về trang danh sách khoan chi
        }
    }
}
