using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ComputerStore_WPF.Models;
using ComputerStore_WPF.Repositories;
using ComputerStore_WPF.Utilities;
using ComputerStore_WPF.ViewModels.Base;

namespace ComputerStore_WPF.ViewModels
{
    /// <summary>
    /// Model hiển thị dữ liệu cho biểu đồ tròn
    /// </summary>
    public class PieChartItem : ViewModelBase
    {
        public string TenMuc { get; set; }
        public decimal GiaTri { get; set; }
        public double PhanTram { get; set; }
        public string MauSac { get; set; }
        public string HienThi => $"{TenMuc}: {FormatHelper.FormatCurrency(GiaTri)} ({PhanTram:F1}%)";
    }

    public class ThongKeViewModel : ViewModelBase
    {
        private readonly HoaDonRepository _repo = new HoaDonRepository();

        public ObservableCollection<HoaDonBanModel> DSHoaDon { get; set; } = new ObservableCollection<HoaDonBanModel>();

        // Dữ liệu cho biểu đồ tròn
        public ObservableCollection<PieChartItem> DoanhThuTheoLoai { get; set; } = new ObservableCollection<PieChartItem>();
        public ObservableCollection<PieChartItem> DoanhThuTheoThang { get; set; } = new ObservableCollection<PieChartItem>();

        private DateTime _tuNgay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DateTime TuNgay { get => _tuNgay; set => SetProperty(ref _tuNgay, value); }

        private DateTime _denNgay = DateTime.Now;
        public DateTime DenNgay { get => _denNgay; set => SetProperty(ref _denNgay, value); }

        private decimal _tongDoanhThu;
        public decimal TongDoanhThu { get => _tongDoanhThu; set => SetProperty(ref _tongDoanhThu, value); }

        private int _soHoaDon;
        public int SoHoaDon { get => _soHoaDon; set => SetProperty(ref _soHoaDon, value); }

        private string _doanhThuText;
        public string DoanhThuText { get => _doanhThuText; set => SetProperty(ref _doanhThuText, value); }

        private int _namThongKe = DateTime.Now.Year;
        public int NamThongKe { get => _namThongKe; set => SetProperty(ref _namThongKe, value); }

        // Thông báo khi không có dữ liệu
        private string _thongBaoLoaiSP = "";
        public string ThongBaoLoaiSP { get => _thongBaoLoaiSP; set => SetProperty(ref _thongBaoLoaiSP, value); }

        private string _thongBaoTheoThang = "";
        public string ThongBaoTheoThang { get => _thongBaoTheoThang; set => SetProperty(ref _thongBaoTheoThang, value); }

        // Event để view vẽ lại biểu đồ
        public event Action BieuDoChanged;

        public ICommand ThongKeCommand { get; }
        public ICommand ExportCommand { get; }

        // Bảng màu cho biểu đồ tròn
        public static readonly string[] PieColors = new[]
        {
            "#2A9D8F", "#E07A5F", "#3A8970", "#D48166", "#4A7C6B",
            "#6B8E85", "#557A6F", "#8B5E3C", "#5B9BD5", "#A0C4B8",
            "#C77B55", "#718F87"
        };

        public ThongKeViewModel()
        {
            ThongKeCommand = new RelayCommand(_ => LoadThongKe());
            ExportCommand = new RelayCommand(_ => Export());
            LoadThongKe();
        }

        private void LoadThongKe()
        {
            try
            {
                // === Danh sách hóa đơn ===
                DSHoaDon.Clear();
                var list = _repo.GetHoaDonBanByDateRange(TuNgay, DenNgay.AddDays(1));
                foreach (var hd in list) DSHoaDon.Add(hd);
                TongDoanhThu = list.Sum(x => x.ThanhTienThucTe);
                SoHoaDon = list.Count;
                DoanhThuText = FormatHelper.FormatCurrency(TongDoanhThu);

                // === Doanh thu theo loại sản phẩm (Biểu đồ tròn 1) ===
                LoadDoanhThuTheoLoai();

                // === Doanh thu theo tháng (Biểu đồ tròn 2) ===
                LoadDoanhThuTheoThang();

                // Thông báo view vẽ lại biểu đồ
                BieuDoChanged?.Invoke();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void LoadDoanhThuTheoLoai()
        {
            try
            {
                DoanhThuTheoLoai.Clear();
                var data = _repo.GetDoanhThuTheoLoaiSP(TuNgay, DenNgay.AddDays(1));
                decimal tong = data.Sum(x => x.Item2);

                if (data.Count == 0 || tong == 0)
                {
                    ThongBaoLoaiSP = "Không có dữ liệu doanh thu theo loại sản phẩm trong khoảng thời gian này.";
                    return;
                }

                ThongBaoLoaiSP = "";
                for (int i = 0; i < data.Count; i++)
                {
                    DoanhThuTheoLoai.Add(new PieChartItem
                    {
                        TenMuc = data[i].Item1,
                        GiaTri = data[i].Item2,
                        PhanTram = tong > 0 ? (double)(data[i].Item2 / tong * 100) : 0,
                        MauSac = PieColors[i % PieColors.Length]
                    });
                }
            }
            catch { ThongBaoLoaiSP = "Lỗi tải dữ liệu doanh thu theo loại."; }
        }

        private void LoadDoanhThuTheoThang()
        {
            try
            {
                DoanhThuTheoThang.Clear();
                var data = _repo.GetDoanhThuTheoThang(NamThongKe);
                decimal tong = data.Sum(x => x.Item2);

                if (data.Count == 0 || tong == 0)
                {
                    ThongBaoTheoThang = $"Không có dữ liệu doanh thu trong năm {NamThongKe}.";
                    return;
                }

                ThongBaoTheoThang = "";
                for (int i = 0; i < data.Count; i++)
                {
                    DoanhThuTheoThang.Add(new PieChartItem
                    {
                        TenMuc = $"Tháng {data[i].Item1}",
                        GiaTri = data[i].Item2,
                        PhanTram = tong > 0 ? (double)(data[i].Item2 / tong * 100) : 0,
                        MauSac = PieColors[i % PieColors.Length]
                    });
                }
            }
            catch { ThongBaoTheoThang = $"Lỗi tải dữ liệu doanh thu theo tháng."; }
        }

        private void Export()
        {
            try
            {
                var dlg = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "HTML File|*.html|CSV File|*.csv",
                    FileName = $"BaoCaoDoanhThu_{TuNgay:yyyyMMdd}_{DenNgay:yyyyMMdd}"
                };
                if (dlg.ShowDialog() == true)
                {
                    var headers = new[] { "Mã HĐ", "Nhân viên", "Khách hàng", "Ngày bán", "Tổng tiền", "Giảm giá", "Thực tế" };
                    if (dlg.FileName.EndsWith(".html"))
                        ExportHelper.ExportToHtml(DSHoaDon.ToList(), dlg.FileName, $"BÁO CÁO DOANH THU ({TuNgay:dd/MM/yyyy} - {DenNgay:dd/MM/yyyy})", headers);
                    else
                        ExportHelper.ExportToCsv(DSHoaDon.ToList(), dlg.FileName, headers);
                    MessageBox.Show("Xuất báo cáo thành công!", "Thành công");
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = dlg.FileName,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }
    }
}
