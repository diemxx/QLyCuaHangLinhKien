using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ComputerStore_WPF.Repositories;
using ComputerStore_WPF.Utilities;

namespace ComputerStore_WPF.Views.UserControls
{
    public partial class TongQuanUC : UserControl
    {
        // Bảng màu cho biểu đồ và card
        private readonly string[] _colors = new[]
        {
            "#2A9D8F", "#E07A5F", "#3A8970", "#D48166", "#4A7C6B",
            "#6B8E85", "#557A6F", "#8B5E3C", "#5B9BD5", "#A0C4B8",
            "#C77B55", "#718F87", "#B8D8D0", "#9A6A4E", "#3D6B5E"
        };

        public TongQuanUC()
        {
            InitializeComponent();
            LoadDashboard();
        }

        private void LoadDashboard()
        {
            try
            {
                var spRepo = new SanPhamRepository();
                var hdRepo = new HoaDonRepository();

                // === 1. Thẻ thống kê nhanh (giữ nguyên logic cũ) ===
                var allSP = spRepo.GetAll();
                var activeSP = allSP.Where(x => x.TrangThai == "Đang kinh doanh").ToList();

                txtTongSP.Text = activeSP.Count.ToString();

                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);
                int soHD = hdRepo.CountHoaDonBan(today, tomorrow);
                txtHoaDon.Text = soHD.ToString();

                decimal doanhThu = hdRepo.GetDoanhThu(today, tomorrow);
                txtDoanhThu.Text = FormatHelper.FormatCurrency(doanhThu);

                var sapHet = activeSP.Where(x => x.SoLuongTon < 5).ToList();
                txtSapHet.Text = sapHet.Count.ToString();
                dgSapHet.ItemsSource = sapHet;

                // === 2. Sơ đồ phân loại linh kiện ===
                LoadSoDoLoaiSanPham(hdRepo);

                // === 3. Biểu đồ tồn kho theo loại ===
                LoadBieuDoTonKho(hdRepo);

                // === 4. Top sản phẩm bán chạy ===
                LoadTopBanChay(hdRepo);
            }
            catch (Exception ex)
            {
                txtTongSP.Text = "Lỗi";
                txtDoanhThu.Text = ex.Message;
            }
        }

        /// <summary>
        /// Tạo sơ đồ phân loại linh kiện dạng cây từ trung tâm
        /// </summary>
        private void LoadSoDoLoaiSanPham(HoaDonRepository repo)
        {
            try
            {
                var data = repo.GetSoLuongTheoLoai();
                icLoaiSanPham.Items.Clear();

                for (int i = 0; i < data.Count; i++)
                {
                    var item = data[i];
                    string color = _colors[i % _colors.Length];

                    var card = new Border
                    {
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color)),
                        CornerRadius = new CornerRadius(8),
                        Padding = new Thickness(15, 10, 15, 10),
                        Margin = new Thickness(5, 8, 5, 5),
                        MinWidth = 160,
                        Effect = new System.Windows.Media.Effects.DropShadowEffect
                        {
                            Color = Colors.Gray,
                            BlurRadius = 6,
                            ShadowDepth = 2,
                            Opacity = 0.15
                        }
                    };

                    var sp = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };

                    // Đường nối từ thanh ngang
                    var connector = new Border
                    {
                        Width = 2,
                        Height = 12,
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A7C6B")),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 0, 0, 0)
                    };

                    sp.Children.Add(new TextBlock
                    {
                        Text = item.Item1, // Tên loại
                        FontWeight = FontWeights.Bold,
                        FontSize = 13,
                        Foreground = Brushes.White,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        TextWrapping = TextWrapping.Wrap,
                        TextAlignment = TextAlignment.Center
                    });
                    sp.Children.Add(new TextBlock
                    {
                        Text = $"{item.Item2} sản phẩm",
                        FontSize = 11,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0EAE5")),
                        HorizontalAlignment = HorizontalAlignment.Center
                    });
                    sp.Children.Add(new TextBlock
                    {
                        Text = $"Tồn kho: {item.Item3}",
                        FontSize = 11,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0EAE5")),
                        HorizontalAlignment = HorizontalAlignment.Center
                    });

                    card.Child = sp;

                    // Wrap trong StackPanel có connector
                    var wrapper = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center };
                    wrapper.Children.Add(connector);
                    wrapper.Children.Add(card);

                    icLoaiSanPham.Items.Add(wrapper);
                }
            }
            catch { /* Bỏ qua nếu không load được */ }
        }

        /// <summary>
        /// Tạo biểu đồ thanh ngang hiển thị tồn kho theo loại
        /// </summary>
        private void LoadBieuDoTonKho(HoaDonRepository repo)
        {
            try
            {
                var data = repo.GetSoLuongTheoLoai();
                spBieuDoTonKho.Children.Clear();

                if (data.Count == 0)
                {
                    spBieuDoTonKho.Children.Add(new TextBlock
                    {
                        Text = "Chưa có dữ liệu loại sản phẩm",
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9CAEA8")),
                        FontSize = 13,
                        FontStyle = FontStyles.Italic,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0, 15, 0, 15)
                    });
                    return;
                }

                int maxTon = data.Max(x => x.Item3);
                if (maxTon == 0) maxTon = 1; // Tránh chia cho 0

                for (int i = 0; i < data.Count; i++)
                {
                    var item = data[i];
                    string color = _colors[i % _colors.Length];
                    double ratio = (double)item.Item3 / maxTon;

                    var row = new Grid { Margin = new Thickness(0, 4, 0, 4) };
                    row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });
                    row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });

                    // Tên loại
                    var txtName = new TextBlock
                    {
                        Text = item.Item1,
                        FontSize = 12,
                        FontWeight = FontWeights.SemiBold,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2C3D38")),
                        VerticalAlignment = VerticalAlignment.Center,
                        TextTrimming = TextTrimming.CharacterEllipsis
                    };
                    Grid.SetColumn(txtName, 0);

                    // Thanh biểu đồ
                    var barContainer = new Grid { Margin = new Thickness(8, 0, 8, 0) };
                    var barBg = new Border
                    {
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0EAE5")),
                        CornerRadius = new CornerRadius(4),
                        Height = 20
                    };
                    var barFill = new Border
                    {
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color)),
                        CornerRadius = new CornerRadius(4),
                        Height = 20,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Width = 0 // Sẽ được set sau khi layout
                    };


                    barContainer.Children.Add(barBg);
                    barContainer.Children.Add(barFill);
                    Grid.SetColumn(barContainer, 1);

                    // Bind width sau khi container có kích thước
                    barContainer.SizeChanged += (s, e) =>
                    {
                        barFill.Width = e.NewSize.Width * ratio;
                    };

                    // Số lượng
                    var txtCount = new TextBlock
                    {
                        Text = $"{item.Item2} SP / {item.Item3} tồn",
                        FontSize = 11,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#557A6F")),
                        VerticalAlignment = VerticalAlignment.Center,
                        FontWeight = FontWeights.SemiBold
                    };
                    Grid.SetColumn(txtCount, 2);

                    row.Children.Add(txtName);
                    row.Children.Add(barContainer);
                    row.Children.Add(txtCount);

                    spBieuDoTonKho.Children.Add(row);
                }
            }
            catch { /* Bỏ qua */ }
        }

        /// <summary>
        /// Load top sản phẩm bán chạy
        /// </summary>
        private void LoadTopBanChay(HoaDonRepository repo)
        {
            try
            {
                var top = repo.GetTopSanPhamBanChay(5);
                var displayList = new List<object>();

                for (int i = 0; i < top.Count; i++)
                {
                    displayList.Add(new
                    {
                        Hang = $"#{i + 1}",
                        MaSP = top[i].Item1,
                        TenSP = top[i].Item2,
                        TongBan = top[i].Item3.ToString("#,##0"),
                        TongTien = FormatHelper.FormatCurrency(top[i].Item4)
                    });
                }

                dgTopBanChay.ItemsSource = displayList;
            }
            catch { /* Bỏ qua */ }
        }
    }
}
