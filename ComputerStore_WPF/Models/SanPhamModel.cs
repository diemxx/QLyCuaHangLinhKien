using System;

namespace ComputerStore_WPF.Models
{
    public class SanPhamModel
    {
        public string MaSP { get; set; }
        public string TenSP { get; set; }
        public string MaLoai { get; set; }
        public string MaNCC { get; set; }
        public string HinhAnh { get; set; }
        public string ThongSoKyThuat { get; set; }
        public decimal GiaNhap { get; set; }
        public decimal GiaBan { get; set; }
        public int SoLuongTon { get; set; }
        public int BaoHanh { get; set; }
        public string TrangThai { get; set; }

        
        public string TenLoai { get; set; }
        public string TenNCC { get; set; }
    }
}
