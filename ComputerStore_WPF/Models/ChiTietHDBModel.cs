using System;

namespace ComputerStore_WPF.Models
{
    public class ChiTietHDBModel
    {
        public string MaHDB { get; set; }
        public string MaSP { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGiaBan { get; set; }
        public decimal ThanhTien { get; set; }
        public DateTime? ThoiHanBaoHanh { get; set; }

        
        public string TenSP { get; set; }
    }
}
