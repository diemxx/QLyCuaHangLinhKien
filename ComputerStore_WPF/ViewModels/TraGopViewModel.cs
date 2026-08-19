using System;
using ComputerStore_WPF.ViewModels.Base;

namespace ComputerStore_WPF.ViewModels
{
    public class TraGopViewModel : ViewModelBase
    {
        private decimal _tongTienHang;
        public decimal TongTienHang { get => _tongTienHang; set { SetProperty(ref _tongTienHang, value); TinhToan(); } }

        private decimal _tienTraTruoc;
        public decimal TienTraTruoc { get => _tienTraTruoc; set { SetProperty(ref _tienTraTruoc, value); TinhToan(); } }

        private int _soThang = 6;
        public int SoThang { get => _soThang; set { SetProperty(ref _soThang, value); TinhToan(); } }

        private decimal _laiSuat = 1.5m;
        public decimal LaiSuat { get => _laiSuat; set { SetProperty(ref _laiSuat, value); TinhToan(); } }

        private decimal _tienTraMoiThang;
        public decimal TienTraMoiThang { get => _tienTraMoiThang; set => SetProperty(ref _tienTraMoiThang, value); }

        private decimal _tongTienPhaiTra;
        public decimal TongTienPhaiTra { get => _tongTienPhaiTra; set => SetProperty(ref _tongTienPhaiTra, value); }

        private decimal _tongLai;
        public decimal TongLai { get => _tongLai; set => SetProperty(ref _tongLai, value); }

        public TraGopViewModel(decimal tongTien)
        {
            TongTienHang = tongTien;
            TinhToan();
        }

        private void TinhToan()
        {
            if (SoThang <= 0) return;
            decimal conLai = TongTienHang - TienTraTruoc;
            if (conLai < 0) conLai = 0;
            decimal laiSuatThang = LaiSuat / 100;
            TongTienPhaiTra = conLai * (1 + laiSuatThang * SoThang) + TienTraTruoc;
            TienTraMoiThang = SoThang > 0 ? (TongTienPhaiTra - TienTraTruoc) / SoThang : 0;
            TongLai = TongTienPhaiTra - TongTienHang;
        }
    }
}
