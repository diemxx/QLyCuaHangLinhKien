using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ComputerStore_WPF.Models;
using ComputerStore_WPF.Repositories;
using ComputerStore_WPF.ViewModels.Base;

namespace ComputerStore_WPF.ViewModels
{
    public class NhapHangViewModel : ViewModelBase
    {
        private readonly HoaDonRepository _hdRepo = new HoaDonRepository();
        private readonly SanPhamRepository _spRepo = new SanPhamRepository();
        private readonly NhanVienModel _currentUser;

        public ObservableCollection<SanPhamModel> DSSanPham { get; set; } = new ObservableCollection<SanPhamModel>();
        public ObservableCollection<NhaCungCapModel> DSNCC { get; set; } = new ObservableCollection<NhaCungCapModel>();
        public ObservableCollection<ChiTietHDNModel> ChiTietNhap { get; set; } = new ObservableCollection<ChiTietHDNModel>();
        public ObservableCollection<HoaDonNhapModel> DSHoaDonNhap { get; set; } = new ObservableCollection<HoaDonNhapModel>();

        private string _maNCC;
        public string MaNCC { get => _maNCC; set => SetProperty(ref _maNCC, value); }

        private string _ghiChu;
        public string GhiChu { get => _ghiChu; set => SetProperty(ref _ghiChu, value); }

        private SanPhamModel _selectedSP;
        public SanPhamModel SelectedSP { get => _selectedSP; set => SetProperty(ref _selectedSP, value); }

        private int _soLuongNhap = 1;
        public int SoLuongNhap { get => _soLuongNhap; set => SetProperty(ref _soLuongNhap, value); }

        private decimal _donGiaNhap;
        public decimal DonGiaNhap { get => _donGiaNhap; set => SetProperty(ref _donGiaNhap, value); }

        private decimal _tongTien;
        public decimal TongTien { get => _tongTien; set => SetProperty(ref _tongTien, value); }

        public ICommand AddItemCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand CreateOrderCommand { get; }
        public ICommand ClearCommand { get; }

        public NhapHangViewModel(NhanVienModel user)
        {
            _currentUser = user;
            AddItemCommand = new RelayCommand(_ => AddItem());
            RemoveItemCommand = new RelayCommand(RemoveItem);
            CreateOrderCommand = new RelayCommand(_ => CreateOrder());
            ClearCommand = new RelayCommand(_ => ClearAll());
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                DSSanPham.Clear();
                foreach (var sp in _spRepo.GetAll()) DSSanPham.Add(sp);
                DSNCC.Clear();
                foreach (var n in _spRepo.GetAllNhaCungCap().Where(x => x.TrangThai == "Hoạt động")) DSNCC.Add(n);
                DSHoaDonNhap.Clear();
                foreach (var h in _hdRepo.GetAllHoaDonNhap()) DSHoaDonNhap.Add(h);
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void AddItem()
        {
            if (SelectedSP == null) { MessageBox.Show("Chọn sản phẩm!"); return; }
            if (SoLuongNhap <= 0) { MessageBox.Show("Số lượng phải > 0!"); return; }
            if (DonGiaNhap <= 0) { MessageBox.Show("Đơn giá phải > 0!"); return; }

            var existing = ChiTietNhap.FirstOrDefault(x => x.MaSP == SelectedSP.MaSP);
            if (existing != null) { MessageBox.Show("SP đã có trong danh sách!"); return; }

            ChiTietNhap.Add(new ChiTietHDNModel
            {
                MaSP = SelectedSP.MaSP,
                TenSP = SelectedSP.TenSP,
                SoLuong = SoLuongNhap,
                DonGiaNhap = DonGiaNhap,
                ThanhTien = SoLuongNhap * DonGiaNhap
            });
            TongTien = ChiTietNhap.Sum(x => x.ThanhTien);
        }

        private void RemoveItem(object param)
        {
            if (param is ChiTietHDNModel item) ChiTietNhap.Remove(item);
            TongTien = ChiTietNhap.Sum(x => x.ThanhTien);
        }

        private void CreateOrder()
        {
            if (string.IsNullOrWhiteSpace(MaNCC)) { MessageBox.Show("Chọn NCC!"); return; }
            if (ChiTietNhap.Count == 0) { MessageBox.Show("Chưa có sản phẩm nhập!"); return; }
            if (MessageBox.Show($"Xác nhận tạo phiếu nhập?\nTổng tiền: {TongTien:#,##0} VNĐ", "Xác nhận", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

            try
            {
                string maHDN = _hdRepo.GenerateMaHDN();
                var hdn = new HoaDonNhapModel
                {
                    MaHDN = maHDN,
                    MaNV = _currentUser.MaNV,
                    MaNCC = MaNCC,
                    NgayNhap = DateTime.Now,
                    TongTien = TongTien,
                    GhiChu = GhiChu
                };
                var chiTiet = ChiTietNhap.Select(x => new ChiTietHDNModel
                {
                    MaHDN = maHDN,
                    MaSP = x.MaSP,
                    SoLuong = x.SoLuong,
                    DonGiaNhap = x.DonGiaNhap,
                    ThanhTien = x.ThanhTien
                }).ToList();

                _hdRepo.CreateHoaDonNhap(hdn, chiTiet);
                MessageBox.Show($"Tạo phiếu nhập {maHDN} thành công!\nTồn kho đã được cập nhật.", "Thành công");
                ClearAll(); LoadData();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void ClearAll()
        {
            ChiTietNhap.Clear(); MaNCC = GhiChu = ""; TongTien = 0; SoLuongNhap = 1; DonGiaNhap = 0;
        }
    }
}
