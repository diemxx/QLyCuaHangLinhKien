using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ComputerStore_WPF.ViewModels;

namespace ComputerStore_WPF.Views.UserControls
{
    public partial class ThongKeUC : UserControl
    {
        public ThongKeUC()
        {
            InitializeComponent();
            Loaded += ThongKeUC_Loaded;
        }

        private void ThongKeUC_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ThongKeViewModel vm)
            {
                // Vẽ biểu đồ lần đầu
                DrawPieChart(canvasLoaiSP, vm.DoanhThuTheoLoai);
                DrawPieChart(canvasTheoThang, vm.DoanhThuTheoThang);

                // Lắng nghe khi dữ liệu thay đổi
                vm.BieuDoChanged += () =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        DrawPieChart(canvasLoaiSP, vm.DoanhThuTheoLoai);
                        DrawPieChart(canvasTheoThang, vm.DoanhThuTheoThang);
                    });
                };
            }
        }

        /// <summary>
        /// Vẽ biểu đồ tròn (Pie Chart) thuần WPF bằng Path + ArcSegment
        /// </summary>
        private void DrawPieChart(Canvas canvas, ObservableCollection<PieChartItem> data)
        {
            canvas.Children.Clear();

            if (data == null || data.Count == 0)
            {
                // Hiển thị vòng tròn rỗng với text
                var emptyCircle = new Ellipse
                {
                    Width = 200,
                    Height = 200,
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E0EAE5")),
                    Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C2D6CE")),
                    StrokeThickness = 2
                };
                Canvas.SetLeft(emptyCircle, 40);
                Canvas.SetTop(emptyCircle, 40);
                canvas.Children.Add(emptyCircle);

                var emptyText = new TextBlock
                {
                    Text = "Chưa có\ndữ liệu",
                    FontSize = 14,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9CAEA8")),
                    FontWeight = FontWeights.SemiBold,
                    TextAlignment = TextAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Canvas.SetLeft(emptyText, 100);
                Canvas.SetTop(emptyText, 125);
                canvas.Children.Add(emptyText);
                return;
            }

            double centerX = 140;
            double centerY = 140;
            double radius = 120;
            double innerRadius = 50; // Donut chart cho đẹp hơn
            double currentAngle = -90; // Bắt đầu từ 12 giờ (đỉnh)

            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];
                double sweepAngle = item.PhanTram / 100.0 * 360.0;

                if (sweepAngle < 0.1) continue; // Bỏ qua phần quá nhỏ

                var path = CreateArcPath(centerX, centerY, radius, innerRadius, currentAngle, sweepAngle, item.MauSac);
                canvas.Children.Add(path);

                // Vẽ text phần trăm tại giữa arc (nếu phần đủ lớn)
                if (item.PhanTram >= 5)
                {
                    double midAngle = currentAngle + sweepAngle / 2;
                    double labelRadius = (radius + innerRadius) / 2;
                    double labelX = centerX + labelRadius * Math.Cos(midAngle * Math.PI / 180);
                    double labelY = centerY + labelRadius * Math.Sin(midAngle * Math.PI / 180);

                    var label = new TextBlock
                    {
                        Text = $"{item.PhanTram:F0}%",
                        FontSize = 11,
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.White
                    };
                    label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Canvas.SetLeft(label, labelX - label.DesiredSize.Width / 2);
                    Canvas.SetTop(label, labelY - label.DesiredSize.Height / 2);
                    canvas.Children.Add(label);
                }

                currentAngle += sweepAngle;
            }

            // Vẽ vòng tròn trung tâm (donut hole)
            var centerCircle = new Ellipse
            {
                Width = innerRadius * 2,
                Height = innerRadius * 2,
                Fill = Brushes.White
            };
            Canvas.SetLeft(centerCircle, centerX - innerRadius);
            Canvas.SetTop(centerCircle, centerY - innerRadius);
            canvas.Children.Add(centerCircle);

            // Text tổng ở trung tâm
            decimal total = 0;
            foreach (var item in data) total += item.GiaTri;

            var centerText = new TextBlock
            {
                Text = $"{data.Count}",
                FontSize = 22,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2C3D38")),
                HorizontalAlignment = HorizontalAlignment.Center,
                TextAlignment = TextAlignment.Center
            };
            centerText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Canvas.SetLeft(centerText, centerX - centerText.DesiredSize.Width / 2);
            Canvas.SetTop(centerText, centerY - centerText.DesiredSize.Height / 2 - 8);
            canvas.Children.Add(centerText);

            var subText = new TextBlock
            {
                Text = "mục",
                FontSize = 11,
                Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9CAEA8")),
                TextAlignment = TextAlignment.Center
            };
            subText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Canvas.SetLeft(subText, centerX - subText.DesiredSize.Width / 2);
            Canvas.SetTop(subText, centerY + 8);
            canvas.Children.Add(subText);
        }

        /// <summary>
        /// Tạo Path cho một phần (slice) của biểu đồ tròn donut
        /// </summary>
        private Path CreateArcPath(double cx, double cy, double outerR, double innerR, double startAngle, double sweepAngle, string colorStr)
        {
            // Giới hạn sweep angle tối đa 359.99 để tránh lỗi ArcSegment
            if (sweepAngle >= 360) sweepAngle = 359.99;

            double startRad = startAngle * Math.PI / 180;
            double endRad = (startAngle + sweepAngle) * Math.PI / 180;

            bool isLargeArc = sweepAngle > 180;

            // Điểm trên vòng ngoài
            Point outerStart = new Point(cx + outerR * Math.Cos(startRad), cy + outerR * Math.Sin(startRad));
            Point outerEnd = new Point(cx + outerR * Math.Cos(endRad), cy + outerR * Math.Sin(endRad));

            // Điểm trên vòng trong
            Point innerStart = new Point(cx + innerR * Math.Cos(endRad), cy + innerR * Math.Sin(endRad));
            Point innerEnd = new Point(cx + innerR * Math.Cos(startRad), cy + innerR * Math.Sin(startRad));

            var figure = new PathFigure { StartPoint = outerStart, IsClosed = true };

            // Arc vòng ngoài (theo chiều kim đồng hồ)
            figure.Segments.Add(new ArcSegment
            {
                Point = outerEnd,
                Size = new Size(outerR, outerR),
                IsLargeArc = isLargeArc,
                SweepDirection = SweepDirection.Clockwise
            });

            // Line từ vòng ngoài vào vòng trong
            figure.Segments.Add(new LineSegment { Point = innerStart });

            // Arc vòng trong (ngược chiều)
            figure.Segments.Add(new ArcSegment
            {
                Point = innerEnd,
                Size = new Size(innerR, innerR),
                IsLargeArc = isLargeArc,
                SweepDirection = SweepDirection.Counterclockwise
            });

            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            Color color;
            try { color = (Color)ColorConverter.ConvertFromString(colorStr); }
            catch { color = Colors.Gray; }

            var path = new Path
            {
                Data = geometry,
                Fill = new SolidColorBrush(color),
                Stroke = Brushes.White,
                StrokeThickness = 2,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            return path;
        }
    }
}
