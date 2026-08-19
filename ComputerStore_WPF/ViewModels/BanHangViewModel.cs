using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using ComputerStore_WPF.Models;
using ComputerStore_WPF.Repositories;
using ComputerStore_WPF.ViewModels.Base;
using Microsoft.Win32;

namespace ComputerStore_WPF.ViewModels
{
    public class BanHangViewModel : ViewModelBase
    {
        private readonly HoaDonRepository _hdRepo = new HoaDonRepository();
        private readonly SanPhamRepository _spRepo = new SanPhamRepository();
        private readonly NhanVienModel _currentUser;

        public ObservableCollection<SanPhamModel> DanhSachSP { get; set; } = new ObservableCollection<SanPhamModel>();
        public ObservableCollection<ChiTietHDBModel> GioHang { get; set; } = new ObservableCollection<ChiTietHDBModel>();
        public ObservableCollection<KhachHangModel> DanhSachKH { get; set; } = new ObservableCollection<KhachHangModel>();

        private SanPhamModel _selectedSP;
        public SanPhamModel SelectedSP { get => _selectedSP; set => SetProperty(ref _selectedSP, value); }

        private KhachHangModel _selectedKH;
        public KhachHangModel SelectedKH
        {
            get => _selectedKH;
            set { SetProperty(ref _selectedKH, value); OnPropertyChanged(nameof(DiemKH)); OnPropertyChanged(nameof(GiamGiaToiDa)); }
        }

        private int _soLuongMua = 1;
        public int SoLuongMua { get => _soLuongMua; set => SetProperty(ref _soLuongMua, value); }

        private decimal _tongTien;
        public decimal TongTien { get => _tongTien; set => SetProperty(ref _tongTien, value); }

        private decimal _giamGia;
        public decimal GiamGia { get => _giamGia; set { SetProperty(ref _giamGia, value); TinhThanhTien(); } }

        private decimal _thanhTienThucTe;
        public decimal ThanhTienThucTe { get => _thanhTienThucTe; set => SetProperty(ref _thanhTienThucTe, value); }

        // Lưu lại hóa đơn vừa tạo
        private string _lastMaHDB;
        public string LastMaHDB { get => _lastMaHDB; set { SetProperty(ref _lastMaHDB, value); OnPropertyChanged(nameof(CanExport)); } }
        public bool CanExport => !string.IsNullOrEmpty(LastMaHDB);

        public int DiemKH => SelectedKH?.DiemTichLuy ?? 0;
        public decimal GiamGiaToiDa => DiemKH * 1000m;

        private string _searchSP;
        public string SearchSP { get => _searchSP; set => SetProperty(ref _searchSP, value); }

        // Commands
        public ICommand SearchSPCommand { get; }
        public ICommand AddToCartCommand { get; }
        public ICommand RemoveFromCartCommand { get; }
        public ICommand ApplyDiscountCommand { get; }
        public ICommand CreateInvoiceCommand { get; }
        public ICommand ClearCartCommand { get; }
        public ICommand TraGopCommand { get; }
        public ICommand ExportInvoiceCommand { get; }

        public event Action<HoaDonBanModel> TraGopRequested;

        public BanHangViewModel(NhanVienModel user)
        {
            _currentUser = user;
            SearchSPCommand = new RelayCommand(_ => SearchSanPham());
            AddToCartCommand = new RelayCommand(_ => AddToCart());
            RemoveFromCartCommand = new RelayCommand(RemoveFromCart);
            ApplyDiscountCommand = new RelayCommand(_ => ApplyDiscount());
            CreateInvoiceCommand = new RelayCommand(_ => CreateInvoice());
            ClearCartCommand = new RelayCommand(_ => ClearCart());
            TraGopCommand = new RelayCommand(_ => OpenTraGop());
            ExportInvoiceCommand = new RelayCommand(_ => ExportInvoice(), _ => CanExport);
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                DanhSachSP.Clear();
                foreach (var sp in _spRepo.GetAll().Where(s => s.TrangThai == "Đang kinh doanh" && s.SoLuongTon > 0))
                    DanhSachSP.Add(sp);

                DanhSachKH.Clear();
                foreach (var kh in _hdRepo.GetAllKhachHang()) DanhSachKH.Add(kh);
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void SearchSanPham()
        {
            DanhSachSP.Clear();
            var results = string.IsNullOrWhiteSpace(SearchSP) ?
                _spRepo.GetAll().Where(s => s.TrangThai == "Đang kinh doanh" && s.SoLuongTon > 0) :
                _spRepo.Search(SearchSP).Where(s => s.TrangThai == "Đang kinh doanh" && s.SoLuongTon > 0);
            foreach (var sp in results) DanhSachSP.Add(sp);
        }

        private void AddToCart()
        {
            if (SelectedSP == null) { MessageBox.Show("Chọn sản phẩm!"); return; }
            if (SoLuongMua <= 0) { MessageBox.Show("Số lượng phải > 0!"); return; }
            if (SoLuongMua > SelectedSP.SoLuongTon) { MessageBox.Show($"Tồn kho chỉ còn {SelectedSP.SoLuongTon}!"); return; }

            var existing = GioHang.FirstOrDefault(x => x.MaSP == SelectedSP.MaSP);
            if (existing != null)
            {
                if (existing.SoLuong + SoLuongMua > SelectedSP.SoLuongTon) { MessageBox.Show("Vượt tồn kho!"); return; }
                existing.SoLuong += SoLuongMua;
                existing.ThanhTien = existing.SoLuong * existing.DonGiaBan;
                var idx = GioHang.IndexOf(existing);
                GioHang.RemoveAt(idx);
                GioHang.Insert(idx, existing);
            }
            else
            {
                int baoHanhMonths = SelectedSP.BaoHanh;
                GioHang.Add(new ChiTietHDBModel
                {
                    MaSP = SelectedSP.MaSP,
                    TenSP = SelectedSP.TenSP,
                    SoLuong = SoLuongMua,
                    DonGiaBan = SelectedSP.GiaBan,
                    ThanhTien = SoLuongMua * SelectedSP.GiaBan,
                    ThoiHanBaoHanh = DateTime.Now.AddMonths(baoHanhMonths)
                });
            }
            TinhTongTien();
        }

        private void RemoveFromCart(object param)
        {
            if (param is ChiTietHDBModel item) GioHang.Remove(item);
            TinhTongTien();
        }

        private void TinhTongTien()
        {
            TongTien = GioHang.Sum(x => x.ThanhTien);
            TinhThanhTien();
        }

        private void TinhThanhTien()
        {
            ThanhTienThucTe = TongTien - GiamGia;
            if (ThanhTienThucTe < 0) ThanhTienThucTe = 0;
        }

        private void ApplyDiscount()
        {
            if (SelectedKH == null) { MessageBox.Show("Chọn khách hàng để áp dụng giảm giá!"); return; }
            if (GiamGia > GiamGiaToiDa) { MessageBox.Show($"Giảm giá tối đa: {GiamGiaToiDa:#,##0} VNĐ ({DiemKH} điểm)"); GiamGia = GiamGiaToiDa; }
            TinhThanhTien();
        }

        private void CreateInvoice()
        {
            if (GioHang.Count == 0) { MessageBox.Show("Giỏ hàng trống!"); return; }
            if (MessageBox.Show($"Xác nhận tạo hóa đơn?\nTổng tiền: {ThanhTienThucTe:#,##0} VNĐ", "Xác nhận", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

            try
            {
                string maHDB = _hdRepo.GenerateMaHDB();
                var hdb = new HoaDonBanModel
                {
                    MaHDB = maHDB,
                    MaNV = _currentUser.MaNV,
                    MaKH = SelectedKH?.MaKH,
                    NgayBan = DateTime.Now,
                    TongTien = TongTien,
                    GiamGia = GiamGia,
                    ThanhTienThucTe = ThanhTienThucTe
                };
                var chiTiet = GioHang.Select(x => new ChiTietHDBModel
                {
                    MaHDB = maHDB,
                    MaSP = x.MaSP,
                    SoLuong = x.SoLuong,
                    DonGiaBan = x.DonGiaBan,
                    ThanhTien = x.ThanhTien,
                    ThoiHanBaoHanh = x.ThoiHanBaoHanh
                }).ToList();

                // Lưu lại chi tiết trước khi clear
                _lastInvoiceItems = chiTiet.Select(x => new ChiTietHDBModel
                {
                    MaHDB = x.MaHDB,
                    MaSP = x.MaSP,
                    TenSP = GioHang.First(g => g.MaSP == x.MaSP).TenSP,
                    SoLuong = x.SoLuong,
                    DonGiaBan = x.DonGiaBan,
                    ThanhTien = x.ThanhTien,
                    ThoiHanBaoHanh = x.ThoiHanBaoHanh
                }).ToList();
                _lastInvoice = hdb;
                _lastInvoice.TenNhanVien = _currentUser.HoTen;
                _lastInvoice.TenKhachHang = SelectedKH?.TenKH ?? "Khách vãng lai";

                _hdRepo.CreateHoaDonBan(hdb, chiTiet);
                LastMaHDB = maHDB;
                MessageBox.Show($"Tạo hóa đơn {maHDB} thành công!\nBấm 'XUẤT HÓA ĐƠN' để in.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearCart();
                LoadData();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private HoaDonBanModel _lastInvoice;
        private List<ChiTietHDBModel> _lastInvoiceItems;

        private void ExportInvoice()
        {
            if (_lastInvoice == null || _lastInvoiceItems == null) { MessageBox.Show("Chưa có hóa đơn để xuất!"); return; }

            var dlg = new SaveFileDialog
            {
                Filter = "HTML File|*.html",
                FileName = $"HoaDon_{_lastInvoice.MaHDB}_{DateTime.Now:yyyyMMdd_HHmmss}.html",
                Title = "Xuất hóa đơn"
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("<!DOCTYPE html>");
                    sb.AppendLine("<html><head><meta charset='utf-8'/>");
                    sb.AppendLine($"<title>Hóa đơn {_lastInvoice.MaHDB}</title>");
                    sb.AppendLine("<style>");
                    sb.AppendLine("body { font-family: 'Segoe UI', sans-serif; max-width: 800px; margin: 0 auto; padding: 30px; }");
                    sb.AppendLine("h1 { color: #2A9D8F; text-align: center; font-size: 24px; }");
                    sb.AppendLine("h2 { color: #2C3D38; text-align: center; font-size: 14px; font-weight: normal; }");
                    sb.AppendLine(".info { margin: 20px 0; }");
                    sb.AppendLine(".info p { margin: 4px 0; font-size: 13px; }");
                    sb.AppendLine("table { width: 100%; border-collapse: collapse; margin: 15px 0; }");
                    sb.AppendLine("th { background: #2A9D8F; color: white; padding: 10px 8px; text-align: left; font-size: 12px; }");
                    sb.AppendLine("td { padding: 8px; border-bottom: 1px solid #ddd; font-size: 12px; }");
                    sb.AppendLine("tr:nth-child(even) { background: #f8faf9; }");
                    sb.AppendLine(".total { text-align: right; margin-top: 15px; }");
                    sb.AppendLine(".total p { margin: 5px 0; font-size: 14px; }");
                    sb.AppendLine(".total .final { font-size: 18px; color: #E07A5F; font-weight: bold; }");
                    sb.AppendLine(".footer { text-align: center; margin-top: 40px; color: #999; font-size: 11px; }");
                    sb.AppendLine("@media print { body { padding: 10px; } }");
                    sb.AppendLine("</style></head><body>");
                    sb.AppendLine("<h1>COMPUTER STORE</h1>");
                    sb.AppendLine("<h2>Cửa hàng linh kiện máy tính & laptop</h2>");
                    sb.AppendLine("<hr/>");
                    sb.AppendLine($"<div class='info'>");
                    sb.AppendLine($"<p><strong>Mã hóa đơn:</strong> {_lastInvoice.MaHDB}</p>");
                    sb.AppendLine($"<p><strong>Ngày bán:</strong> {_lastInvoice.NgayBan:dd/MM/yyyy HH:mm}</p>");
                    sb.AppendLine($"<p><strong>Nhân viên:</strong> {_lastInvoice.TenNhanVien}</p>");
                    sb.AppendLine($"<p><strong>Khách hàng:</strong> {_lastInvoice.TenKhachHang}</p>");
                    sb.AppendLine("</div>");
                    sb.AppendLine("<table><thead><tr>");
                    sb.AppendLine("<th>STT</th><th>Mã SP</th><th>Tên sản phẩm</th><th>SL</th><th>Đơn giá</th><th>Thành tiền</th><th>Bảo hành đến</th>");
                    sb.AppendLine("</tr></thead><tbody>");

                    int stt = 1;
                    foreach (var ct in _lastInvoiceItems)
                    {
                        sb.AppendLine($"<tr><td>{stt++}</td><td>{ct.MaSP}</td><td>{ct.TenSP}</td><td>{ct.SoLuong}</td>");
                        sb.AppendLine($"<td>{ct.DonGiaBan:#,##0} đ</td><td>{ct.ThanhTien:#,##0} đ</td>");
                        sb.AppendLine($"<td>{ct.ThoiHanBaoHanh:dd/MM/yyyy}</td></tr>");
                    }
                    sb.AppendLine("</tbody></table>");
                    sb.AppendLine("<div class='total'>");
                    sb.AppendLine($"<p>Tổng tiền: <strong>{_lastInvoice.TongTien:#,##0} đ</strong></p>");
                    if (_lastInvoice.GiamGia > 0)
                        sb.AppendLine($"<p>Giảm giá: <strong>-{_lastInvoice.GiamGia:#,##0} đ</strong></p>");
                    sb.AppendLine($"<p class='final'>THÀNH TIỀN: {_lastInvoice.ThanhTienThucTe:#,##0} đ</p>");
                    sb.AppendLine("</div>");
                    sb.AppendLine($"<div class='footer'>Ngày xuất: {DateTime.Now:dd/MM/yyyy HH:mm} | Cảm ơn quý khách!</div>");
                    sb.AppendLine("</body></html>");

                    File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show($"Đã xuất hóa đơn ra:\n{dlg.FileName}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Mở file trong trình duyệt
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = dlg.FileName,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xuất hóa đơn: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClearCart()
        {
            GioHang.Clear();
            TongTien = GiamGia = ThanhTienThucTe = 0;
            SelectedKH = null; SoLuongMua = 1;
        }

        private void OpenTraGop()
        {
            if (GioHang.Count == 0) { MessageBox.Show("Giỏ hàng trống!"); return; }
            TraGopRequested?.Invoke(new HoaDonBanModel { TongTien = TongTien, ThanhTienThucTe = ThanhTienThucTe });
        }
    }
}
