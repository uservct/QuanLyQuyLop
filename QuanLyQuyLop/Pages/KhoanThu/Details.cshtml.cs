using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace QuanLyQuyLop.Pages.KhoanThu
{
    public class ChiTietThuInfo
    {
        public string Id { get; set; }
        public string MaSV { get; set; }
        public string HoTen { get; set; }
        public string? GhiChu { get; set; }
        public bool DaNop { get; set; }
    }
    public class DetailsModel : PageModel
    {
        public KhoanThuInfo khoanThuInfo = new KhoanThuInfo();
        public List<ChiTietThuInfo> listChiTietThu = new List<ChiTietThuInfo>();
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
                    // lấy danh sách tv + đã nộp
                    string sqlChiTietThu = @"SELECT ctt.Id, tv.MSV, tv.HoTen, ctt.GhiChu, ctt.DaNop
                                            FROM ChiTietThu ctt
                                            JOIN ThanhVien tv ON ctt.ThanhVienId = tv.Id
                                            WHERE ctt.KhoanThuId = @id;";
                    using (SqlCommand command = new SqlCommand(sqlChiTietThu, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ChiTietThuInfo chiTietThuInfo = new ChiTietThuInfo();
                                chiTietThuInfo.Id = "" + reader.GetInt32(0);
                                chiTietThuInfo.MaSV = reader.GetString(1);
                                chiTietThuInfo.HoTen = reader.GetString(2);
                                chiTietThuInfo.GhiChu = reader.IsDBNull(3) ? "" : reader.GetString(3);
                                chiTietThuInfo.DaNop = reader.GetBoolean(4);

                                listChiTietThu.Add(chiTietThuInfo);
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
            string id = Request.Form["id"];
            try
            {
                string connectionString = "Data Source=localhost\\sqlexpress;Initial Catalog=QuanLyQuyLop;" +
                    "Integrated Security=True;TrustServerCertificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sqlLoad = @"SELECT ctt.Id, tv.MSV, tv.HoTen, ctt.GhiChu, ctt.DaNop
                               FROM ChiTietThu ctt
                               JOIN ThanhVien tv ON ctt.ThanhVienId = tv.Id
                               WHERE ctt.KhoanThuId = @id;";
                    listChiTietThu.Clear(); // Xóa danh sách cũ để tránh trùng lặp
                    using (SqlCommand command = new SqlCommand(sqlLoad, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ChiTietThuInfo chiTietThuInfo = new ChiTietThuInfo();
                                chiTietThuInfo.Id = "" + reader.GetInt32(0);
                                chiTietThuInfo.MaSV = reader.GetString(1);
                                chiTietThuInfo.HoTen = reader.GetString(2);
                                chiTietThuInfo.GhiChu = reader.IsDBNull(3) ? "" : reader.GetString(3);
                                chiTietThuInfo.DaNop = reader.GetBoolean(4);

                                listChiTietThu.Add(chiTietThuInfo);
                            }
                        }
                    }
                    // lấy danh sách tv đã nộp từ form
                    foreach (var item in listChiTietThu)
                    {
                        string checkboxName = $"daNop[{item.Id}]";
                        bool daNop = Request.Form.ContainsKey(checkboxName);

                        string ghiChuKey = $"ghiChu[{item.Id}]";
                        string ghiChu = Request.Form[ghiChuKey];

                        string sqlUpdate = "UPDATE ChiTietThu SET DaNop = @daNop, GhiChu = @ghiChu WHERE Id = @id";
                        using (SqlCommand cmd = new SqlCommand(sqlUpdate, connection))
                        {
                            cmd.Parameters.AddWithValue("@daNop", daNop);
                            cmd.Parameters.AddWithValue("@ghiChu", ghiChu ?? "");
                            cmd.Parameters.AddWithValue("@id", item.Id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    // TempData["SuccessMessage"] = "Cập nhật thành công!";
                }
                // reload dữ liệu
                Response.Redirect("/KhoanThu/Details?id=" + id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
