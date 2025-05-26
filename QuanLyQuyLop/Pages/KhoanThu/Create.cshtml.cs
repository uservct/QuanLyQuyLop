using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuanLyQuyLop.Pages.ThanhVien;

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
                    // thêm khoản thu
                    string sql = "INSERT INTO KhoanThu" + "(TenKhoanThu, SoTien, NgayTao, HanNop, GhiChu) VALUES " + "(@tenkhoanthu, @sotien, @ngaytao, @hannop, @ghichu);"
                                    + " SELECT SCOPE_IDENTITY();"; // Lấy Id mới thêm"

                    int idKhoanThuMoi = 0;
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@tenkhoanthu", khoanThuInfo.TenKhoanThu);
                        command.Parameters.AddWithValue("@sotien", khoanThuInfo.SoTien);
                        command.Parameters.AddWithValue("@ngaytao", khoanThuInfo.NgayTao);
                        command.Parameters.AddWithValue("@hannop", khoanThuInfo.HanNop);
                        command.Parameters.AddWithValue("@ghichu", khoanThuInfo.GhiChu);

                        // command.ExecuteNonQuery();

                        object result = command.ExecuteScalar(); //executeScalar trả về giá trị đầu tiên của cột đầu tiên trong kết quả truy vấn
                        if (result != null)
                        {
                            idKhoanThuMoi = Convert.ToInt32(result);
                        }
                    }
                    // lấy danh sách tv
                    List<int> listThanhVien = new List<int>();
                    string sqlTV = "SELECT Id FROM ThanhVien";

                    using (SqlCommand command = new SqlCommand(sqlTV, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listThanhVien.Add(reader.GetInt32(0));
                        }
                    }
                    // tạo ChiTietThu cho mỗi thành viên
                    foreach (int thanhVienId in listThanhVien)
                    {
                        string sqlCTT = @"INSERT INTO ChiTietThu (KhoanThuId, ThanhVienId, DaNop)
                                        VALUES (@idKhoanThuMoi, @thanhVienId, 0)";

                        using (SqlCommand cmdInsert = new SqlCommand(sqlCTT, connection))
                        {
                            cmdInsert.Parameters.AddWithValue("@idKhoanThuMoi", idKhoanThuMoi);
                            cmdInsert.Parameters.AddWithValue("@thanhVienId", thanhVienId);
                            cmdInsert.ExecuteNonQuery();
                        }
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

