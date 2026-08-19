using System;
using System.Windows.Input;
using ComputerStore_WPF.Models;
using ComputerStore_WPF.ViewModels.Base;

namespace ComputerStore_WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private NhanVienModel _currentUser;
        public NhanVienModel CurrentUser
        {
            get => _currentUser;
            set
            {
                SetProperty(ref _currentUser, value);
                OnPropertyChanged(nameof(IsQuanLy));
                OnPropertyChanged(nameof(IsNhanVienBanHang));
                OnPropertyChanged(nameof(IsThuKho));
                OnPropertyChanged(nameof(WelcomeText));
            }
        }

        public string WelcomeText => CurrentUser != null ? $"Xin chào, {CurrentUser.HoTen} ({CurrentUser.TenVaiTro})" : "";

        // Phân quyền
        public bool IsQuanLy => CurrentUser?.MaVaiTro == "VT01";
        public bool IsNhanVienBanHang => CurrentUser?.MaVaiTro == "VT02";
        public bool IsThuKho => CurrentUser?.MaVaiTro == "VT03";

        private ViewModelBase _currentView;
        public ViewModelBase CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        private string _selectedMenu;
        public string SelectedMenu
        {
            get => _selectedMenu;
            set => SetProperty(ref _selectedMenu, value);
        }

        // Commands
        public ICommand NavigateCommand { get; }
        public ICommand LogoutCommand { get; }

        // Event đăng xuất
        public event Action LogoutRequested;

        // ViewModels con - khởi tạo lazy
        private SanPhamViewModel _sanPhamVM;
        private BanHangViewModel _banHangVM;
        private NhapHangViewModel _nhapHangVM;
        private ThongKeViewModel _thongKeVM;
        private LichSuViewModel _lichSuVM;
        private HoaDonViewModel _hoaDonVM;
        private NhanVienViewModel _nhanVienVM;

        public MainViewModel(NhanVienModel user)
        {
            CurrentUser = user;
            NavigateCommand = new RelayCommand(Navigate);
            LogoutCommand = new RelayCommand(_ => LogoutRequested?.Invoke());

            // Mặc định hiển thị Tổng quan
            Navigate("TongQuan");
        }

        private void Navigate(object parameter)
        {
            string page = parameter?.ToString();
            SelectedMenu = page;

            switch (page)
            {
                case "TongQuan":
                    CurrentView = null; 
                    break;
                case "SanPham":
                    if (_sanPhamVM == null) _sanPhamVM = new SanPhamViewModel(CurrentUser);
                    CurrentView = _sanPhamVM;
                    break;
                case "BanHang":
                    if (_banHangVM == null) _banHangVM = new BanHangViewModel(CurrentUser);
                    CurrentView = _banHangVM;
                    break;
                case "NhapHang":
                    if (_nhapHangVM == null) _nhapHangVM = new NhapHangViewModel(CurrentUser);
                    CurrentView = _nhapHangVM;
                    break;
                case "KhachHang":
                    CurrentView = null;
                    break;
                case "ThongKe":
                    if (_thongKeVM == null) _thongKeVM = new ThongKeViewModel();
                    CurrentView = _thongKeVM;
                    break;
                case "LichSu":
                    if (_lichSuVM == null) _lichSuVM = new LichSuViewModel();
                    CurrentView = _lichSuVM;
                    break;
                case "HoaDon":
                    if (_hoaDonVM == null) _hoaDonVM = new HoaDonViewModel();
                    CurrentView = _hoaDonVM;
                    break;
                case "NhanVien":
                    if (_nhanVienVM == null) _nhanVienVM = new NhanVienViewModel();
                    CurrentView = _nhanVienVM;
                    break;
            }
        }
    }
}
