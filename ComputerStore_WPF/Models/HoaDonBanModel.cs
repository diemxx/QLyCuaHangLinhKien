using System;

namespace ComputerStore_WPF.Models
{
    public class HoaDonBanModel
    {
        public string MaHDB { get; set; }
        public string MaNV { get; set; }
        public string MaKH { get; set; }
        public DateTime NgayBan { get; set; }
        public decimal TongTien { get; set; }
        public decimal GiamGia { get; set; }
        public decimal ThanhTienThucTe { get; set; }

        
        public string TenNhanVien { get; set; }
        public string TenKhachHang { get; set; }
    }
}
