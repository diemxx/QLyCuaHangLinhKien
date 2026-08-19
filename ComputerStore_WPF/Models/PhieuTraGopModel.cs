using System;

namespace ComputerStore_WPF.Models
{
    public class PhieuTraGopModel
    {
        public string MaPhieu { get; set; }
        public string MaHDB { get; set; }
        public int SoThangTraGop { get; set; }
        public decimal LaiSuat { get; set; }
        public decimal TienTraMoiThang { get; set; }
        public decimal TienTraTruoc { get; set; }
        public decimal TongTienPhaiTra { get; set; }
        public DateTime NgayBatDau { get; set; }
        public string TrangThai { get; set; }
    }
}
