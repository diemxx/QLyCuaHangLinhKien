using System.Windows;
using ComputerStore_WPF.Models;
using ComputerStore_WPF.ViewModels;
using ComputerStore_WPF.Views.UserControls;

namespace ComputerStore_WPF.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private readonly NhanVienModel _currentUser;
        private bool _traGopSubscribed;

        public MainWindow(NhanVienModel user)
        {
            InitializeComponent();
            _currentUser = user;
            _viewModel = new MainViewModel(user);
            DataContext = _viewModel;

            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.CurrentView) || e.PropertyName == nameof(MainViewModel.SelectedMenu))
                    UpdateContent();
            };

            _viewModel.LogoutRequested += OnLogout;
            UpdateContent();
        }

        private void UpdateContent()
        {
            switch (_viewModel.SelectedMenu)
            {
                case "TongQuan":
                    MainContent.Content = new TongQuanUC();
                    break;
                case "SanPham":
                    MainContent.Content = new SanPhamUC { DataContext = _viewModel.CurrentView };
                    break;
                case "BanHang":
                    var banHangUC = new BanHangUC { DataContext = _viewModel.CurrentView };
                    if (!_traGopSubscribed && _viewModel.CurrentView is BanHangViewModel banHangVM)
                    {
                        banHangVM.TraGopRequested += (hdb) =>
                        {
                            var traGopWindow = new ThanhToanTraGopWindow(hdb.ThanhTienThucTe);
                            traGopWindow.Owner = this;
                            if (traGopWindow.ShowDialog() == true)
                            {
                                var vm = traGopWindow.ViewModel;
                                MessageBox.Show(
                                    $"Đã xác nhận trả góp thành công!\n\n" +
                                    $"Tổng tiền hàng: {vm.TongTienHang:#,##0} VNĐ\n" +
                                    $"Trả trước: {vm.TienTraTruoc:#,##0} VNĐ\n" +
                                    $"Số tháng: {vm.SoThang}\n" +
                                    $"Trả mỗi tháng: {vm.TienTraMoiThang:#,##0} VNĐ\n" +
                                    $"Tổng phải trả: {vm.TongTienPhaiTra:#,##0} VNĐ",
                                    "Trả góp thành công",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                            }
                        };
                        _traGopSubscribed = true;
                    }
                    MainContent.Content = banHangUC;
                    break;
                case "NhapHang":
                    MainContent.Content = new NhapHangUC { DataContext = _viewModel.CurrentView };
                    break;
                case "KhachHang":
                    MainContent.Content = new KhachHangUC();
                    break;
                case "ThongKe":
                    MainContent.Content = new ThongKeUC { DataContext = _viewModel.CurrentView };
                    break;
                case "LichSu":
                    MainContent.Content = new LichSuTimKiemUC { DataContext = _viewModel.CurrentView };
                    break;
                case "HoaDon":
                    MainContent.Content = new HoaDonUC { DataContext = _viewModel.CurrentView };
                    break;
                case "NhanVien":
                    MainContent.Content = new NhanVienUC { DataContext = _viewModel.CurrentView };
                    break;
            }
        }

        private void OnLogout()
        {
            if (MessageBox.Show("Bạn có chắc muốn đăng xuất?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }
    }
}
