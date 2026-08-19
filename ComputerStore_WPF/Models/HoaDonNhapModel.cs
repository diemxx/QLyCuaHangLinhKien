using System;

namespace ComputerStore_WPF.Models
{
    public class HoaDonNhapModel
    {
        public string MaHDN { get; set; }
        public string MaNV { get; set; }
        public string MaNCC { get; set; }
        public DateTime NgayNhap { get; set; }
        public decimal TongTien { get; set; }
        public string GhiChu { get; set; }

        public string TenNhanVien { get; set; }
        public string TenNCC { get; set; }
    }
}
