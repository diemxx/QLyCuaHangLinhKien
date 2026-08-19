using System;
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
    public class HoaDonViewModel : ViewModelBase
    {
        private readonly HoaDonRepository _hdRepo = new HoaDonRepository();

        // Hóa đơn bán
        public ObservableCollection<HoaDonBanModel> DSHoaDonBan { get; set; } = new ObservableCollection<HoaDonBanModel>();
        public ObservableCollection<ChiTietHDBModel> ChiTietHDB { get; set; } = new ObservableCollection<ChiTietHDBModel>();

        // Hóa đơn nhập
        public ObservableCollection<HoaDonNhapModel> DSHoaDonNhap { get; set; } = new ObservableCollection<HoaDonNhapModel>();
        public ObservableCollection<ChiTietHDNModel> ChiTietHDN { get; set; } = new ObservableCollection<ChiTietHDNModel>();

        // Filter
        private DateTime _tuNgay = DateTime.Now.AddMonths(-1);
        public DateTime TuNgay { get => _tuNgay; set => SetProperty(ref _tuNgay, value); }

        private DateTime _denNgay = DateTime.Now;
        public DateTime DenNgay { get => _denNgay; set => SetProperty(ref _denNgay, value); }

        // Selected
        private HoaDonBanModel _selectedHDB;
        public HoaDonBanModel SelectedHDB
        {
            get => _selectedHDB;
            set { SetProperty(ref _selectedHDB, value); LoadChiTietHDB(); }
        }

        private HoaDonNhapModel _selectedHDN;
        public HoaDonNhapModel SelectedHDN
        {
            get => _selectedHDN;
            set { SetProperty(ref _selectedHDN, value); LoadChiTietHDN(); }
        }

        // Commands
        public ICommand LoadDataCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ExportHDBCommand { get; }
        public ICommand ExportHDNCommand { get; }

        public HoaDonViewModel()
        {
            LoadDataCommand = new RelayCommand(_ => LoadAll());
            RefreshCommand = new RelayCommand(_ => LoadAll());
            ExportHDBCommand = new RelayCommand(_ => ExportHDB(), _ => SelectedHDB != null);
            ExportHDNCommand = new RelayCommand(_ => ExportHDN(), _ => SelectedHDN != null);
            LoadAll();
        }

        private void LoadAll()
        {
            try
            {
                DSHoaDonBan.Clear();
                foreach (var h in _hdRepo.GetAllHoaDonBan()) DSHoaDonBan.Add(h);

                DSHoaDonNhap.Clear();
                foreach (var h in _hdRepo.GetAllHoaDonNhap()) DSHoaDonNhap.Add(h);

                ChiTietHDB.Clear();
                ChiTietHDN.Clear();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void LoadChiTietHDB()
        {
            ChiTietHDB.Clear();
            if (SelectedHDB == null) return;
            try
            {
                foreach (var ct in _hdRepo.GetChiTietHDB(SelectedHDB.MaHDB))
                    ChiTietHDB.Add(ct);
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void LoadChiTietHDN()
        {
            ChiTietHDN.Clear();
            if (SelectedHDN == null) return;
            try
            {
                foreach (var ct in _hdRepo.GetChiTietHDN(SelectedHDN.MaHDN))
                    ChiTietHDN.Add(ct);
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void ExportHDB()
        {
            if (SelectedHDB == null) return;
            var dlg = new SaveFileDialog
            {
                Filter = "HTML File|*.html",
                FileName = $"HoaDonBan_{SelectedHDB.MaHDB}.html",
                Title = "Xuất hóa đơn bán"
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var chiTiet = _hdRepo.GetChiTietHDB(SelectedHDB.MaHDB);
                    var sb = new StringBuilder();
                    sb.AppendLine("<!DOCTYPE html><html><head><meta charset='utf-8'/>");
                    sb.AppendLine($"<title>Hóa đơn bán {SelectedHDB.MaHDB}</title>");
                    sb.AppendLine("<style>");
                    sb.AppendLine("body { font-family: 'Segoe UI', sans-serif; max-width: 800px; margin: 0 auto; padding: 30px; }");
                    sb.AppendLine("h1 { color: #2A9D8F; text-align: center; } h2 { text-align: center; font-weight: normal; font-size: 14px; }");
                    sb.AppendLine("table { width: 100%; border-collapse: collapse; margin: 15px 0; }");
                    sb.AppendLine("th { background: #2A9D8F; color: white; padding: 10px; text-align: left; }");
                    sb.AppendLine("td { padding: 8px; border-bottom: 1px solid #ddd; } tr:nth-child(even) { background: #f8faf9; }");
                    sb.AppendLine(".total { text-align: right; } .total .final { font-size: 18px; color: #E07A5F; font-weight: bold; }");
                    sb.AppendLine(".footer { text-align: center; margin-top: 40px; color: #999; font-size: 11px; }");
                    sb.AppendLine("</style></head><body>");
                    sb.AppendLine("<h1>🖥️ COMPUTER STORE</h1><h2>HÓA ĐƠN BÁN HÀNG</h2><hr/>");
                    sb.AppendLine($"<p><strong>Mã HĐ:</strong> {SelectedHDB.MaHDB} | <strong>Ngày:</strong> {SelectedHDB.NgayBan:dd/MM/yyyy HH:mm}</p>");
                    sb.AppendLine($"<p><strong>NV:</strong> {SelectedHDB.TenNhanVien} | <strong>KH:</strong> {SelectedHDB.TenKhachHang}</p>");
                    sb.AppendLine("<table><tr><th>STT</th><th>Mã SP</th><th>Tên SP</th><th>SL</th><th>Đơn giá</th><th>Thành tiền</th></tr>");
                    int stt = 1;
                    foreach (var ct in chiTiet)
                        sb.AppendLine($"<tr><td>{stt++}</td><td>{ct.MaSP}</td><td>{ct.TenSP}</td><td>{ct.SoLuong}</td><td>{ct.DonGiaBan:#,##0} đ</td><td>{ct.ThanhTien:#,##0} đ</td></tr>");
                    sb.AppendLine("</table>");
                    sb.AppendLine($"<div class='total'><p>Tổng: <strong>{SelectedHDB.TongTien:#,##0} đ</strong></p>");
                    if (SelectedHDB.GiamGia > 0) sb.AppendLine($"<p>Giảm giá: <strong>-{SelectedHDB.GiamGia:#,##0} đ</strong></p>");
                    sb.AppendLine($"<p class='final'>THÀNH TIỀN: {SelectedHDB.ThanhTienThucTe:#,##0} đ</p></div>");
                    sb.AppendLine($"<div class='footer'>Xuất ngày {DateTime.Now:dd/MM/yyyy HH:mm}</div></body></html>");
                    File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("Xuất hóa đơn thành công!", "Thành công");
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = dlg.FileName, UseShellExecute = true });
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            }
        }

        private void ExportHDN()
        {
            if (SelectedHDN == null) return;
            var dlg = new SaveFileDialog
            {
                Filter = "HTML File|*.html",
                FileName = $"HoaDonNhap_{SelectedHDN.MaHDN}.html",
                Title = "Xuất hóa đơn nhập"
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var chiTiet = _hdRepo.GetChiTietHDN(SelectedHDN.MaHDN);
                    var sb = new StringBuilder();
                    sb.AppendLine("<!DOCTYPE html><html><head><meta charset='utf-8'/>");
                    sb.AppendLine($"<title>Phiếu nhập {SelectedHDN.MaHDN}</title>");
                    sb.AppendLine("<style>");
                    sb.AppendLine("body { font-family: 'Segoe UI', sans-serif; max-width: 800px; margin: 0 auto; padding: 30px; }");
                    sb.AppendLine("h1 { color: #4A7C6B; text-align: center; } h2 { text-align: center; font-weight: normal; font-size: 14px; }");
                    sb.AppendLine("table { width: 100%; border-collapse: collapse; margin: 15px 0; }");
                    sb.AppendLine("th { background: #4A7C6B; color: white; padding: 10px; text-align: left; }");
                    sb.AppendLine("td { padding: 8px; border-bottom: 1px solid #ddd; } tr:nth-child(even) { background: #f8faf9; }");
                    sb.AppendLine(".total { text-align: right; font-size: 16px; font-weight: bold; color: #2C3D38; }");
                    sb.AppendLine(".footer { text-align: center; margin-top: 40px; color: #999; font-size: 11px; }");
                    sb.AppendLine("</style></head><body>");
                    sb.AppendLine("<h1>🖥️ COMPUTER STORE</h1><h2>PHIẾU NHẬP HÀNG</h2><hr/>");
                    sb.AppendLine($"<p><strong>Mã phiếu:</strong> {SelectedHDN.MaHDN} | <strong>Ngày:</strong> {SelectedHDN.NgayNhap:dd/MM/yyyy}</p>");
                    sb.AppendLine($"<p><strong>NV:</strong> {SelectedHDN.TenNhanVien} | <strong>NCC:</strong> {SelectedHDN.TenNCC}</p>");
                    if (!string.IsNullOrEmpty(SelectedHDN.GhiChu)) sb.AppendLine($"<p><strong>Ghi chú:</strong> {SelectedHDN.GhiChu}</p>");
                    sb.AppendLine("<table><tr><th>STT</th><th>Mã SP</th><th>Tên SP</th><th>SL</th><th>Đơn giá nhập</th><th>Thành tiền</th></tr>");
                    int stt = 1;
                    foreach (var ct in chiTiet)
                        sb.AppendLine($"<tr><td>{stt++}</td><td>{ct.MaSP}</td><td>{ct.TenSP}</td><td>{ct.SoLuong}</td><td>{ct.DonGiaNhap:#,##0} đ</td><td>{ct.ThanhTien:#,##0} đ</td></tr>");
                    sb.AppendLine("</table>");
                    sb.AppendLine($"<p class='total'>TỔNG: {SelectedHDN.TongTien:#,##0} đ</p>");
                    sb.AppendLine($"<div class='footer'>Xuất ngày {DateTime.Now:dd/MM/yyyy HH:mm}</div></body></html>");
                    File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("Xuất phiếu nhập thành công!", "Thành công");
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = dlg.FileName, UseShellExecute = true });
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            }
        }
    }
}
