using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace QuanLyQuyLop.Pages
{
    public class IndexModel : PageModel
    {
        public int TongThu { get; set; }
        public int TongChi { get; set; }
        public int SoDu => TongThu - TongChi;
        public void OnGet()
        {
            string connectionString = "Data Source=localhost\\sqlexpress;Initial Catalog=QuanLyQuyLop;" +
                "Integrated Security=True;Pooling=False;TrustServerCertificate=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Tổng thu
                string sqlThu = @"SELECT SUM(kt.SoTien)
                                    FROM ChiTietThu ctt
                                    JOIN KhoanThu kt ON ctt.KhoanThuId = kt.Id
                                    WHERE ctt.DaNop = 1";
                using (SqlCommand cmd = new SqlCommand(sqlThu, connection))
                {
                    TongThu = (int)cmd.ExecuteScalar();
                }

                // Tổng chi
                string sqlChi = "SELECT SUM(SoTien) FROM KhoanChi";
                using (SqlCommand cmd = new SqlCommand(sqlChi, connection))
                {
                    TongChi = (int)cmd.ExecuteScalar();
                }
            }
        }
    }
}
