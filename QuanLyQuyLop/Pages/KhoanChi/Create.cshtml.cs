using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace QuanLyQuyLop.Pages.KhoanChi
{
    public class CreateModel : PageModel
    {
        public KhoanChiInfo khoanChiInfo = new KhoanChiInfo();
        public string errorMessage = ""; //hiển thị thông báo lỗi khi người dùng ko nhập đủ thông tin
        public void OnGet()
        {
        }
        public void OnPost()
        {
            khoanChiInfo.TenKhoanChi = Request.Form["tenkhoanchi"];
            // khoanChiInfo.SoTien = Request.Form["sotien"];
            khoanChiInfo.NgayChi = Request.Form["ngaychi"];
            khoanChiInfo.GhiChu = Request.Form["ghichu"];
            string soTienStr = Request.Form["sotien"];

            if (string.IsNullOrEmpty(khoanChiInfo.TenKhoanChi) || string.IsNullOrEmpty(khoanChiInfo.NgayChi))
            {
                errorMessage = "Vui lòng điền đủ thông tin";
                return;
            }
            if (!int.TryParse(Request.Form["sotien"], out int soTienInt) || soTienInt <= 0)
            {
                //
                errorMessage = "Số tiền không hợp lệ";
                return;
            }

            khoanChiInfo.SoTien = soTienInt;

            //if ok, save new kt to database
            try
            {
                string connectionString = "Data Source=localhost\\sqlexpress;Initial Catalog=QuanLyQuyLop;" +
                    "Integrated Security=True;Pooling=False;TrustServerCertificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = @"INSERT INTO KhoanChi (TenKhoanChi, SoTien, NgayChi, GhiChu) VALUES (@tenkhoanchi, @sotien, @ngaychi, @ghichu);";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@tenkhoanchi", khoanChiInfo.TenKhoanChi);
                        command.Parameters.AddWithValue("@sotien", khoanChiInfo.SoTien);
                        command.Parameters.AddWithValue("@ngaychi", khoanChiInfo.NgayChi);
                        command.Parameters.AddWithValue("@ghichu", khoanChiInfo.GhiChu);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
            Response.Redirect("/KhoanChi");
        }
    }
}
