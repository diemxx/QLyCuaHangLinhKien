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
    public class NhanVienViewModel : ViewModelBase
    {
        private readonly HoaDonRepository _repo = new HoaDonRepository();

        // Danh sách nhân viên
        public ObservableCollection<NhanVienModel> DSNhanVien { get; set; } = new ObservableCollection<NhanVienModel>();

        // Danh sách vai trò
        public ObservableCollection<VaiTroModel> DSVaiTro { get; set; } = new ObservableCollection<VaiTroModel>();

        // Nhân viên đang chọn trong DataGrid
        private NhanVienModel _selectedNhanVien;
        public NhanVienModel SelectedNhanVien
        {
            get => _selectedNhanVien;
            set
            {
                if (SetProperty(ref _selectedNhanVien, value) && value != null)
                {
                    // Load thông tin vào form
                    MaNV = value.MaNV;
                    HoTen = value.HoTen;
                    NgaySinh = value.NgaySinh;
                    SDT = value.SDT;
                    Email = value.Email;
                    TenDangNhap = value.TenDangNhap;
                    SelectedVaiTro = DSVaiTro.FirstOrDefault(v => v.MaVaiTro == value.MaVaiTro);
                    TrangThai = value.TrangThai;
                    IsEditing = true;
                }
            }
        }

        // Form fields
        private string _maNV;
        public string MaNV { get => _maNV; set => SetProperty(ref _maNV, value); }

        private string _hoTen;
        public string HoTen { get => _hoTen; set => SetProperty(ref _hoTen, value); }

        private DateTime? _ngaySinh;
        public DateTime? NgaySinh { get => _ngaySinh; set => SetProperty(ref _ngaySinh, value); }

        private string _sdt;
        public string SDT { get => _sdt; set => SetProperty(ref _sdt, value); }

        private string _email;
        public string Email { get => _email; set => SetProperty(ref _email, value); }

        private string _tenDangNhap;
        public string TenDangNhap { get => _tenDangNhap; set => SetProperty(ref _tenDangNhap, value); }

        private VaiTroModel _selectedVaiTro;
        public VaiTroModel SelectedVaiTro { get => _selectedVaiTro; set => SetProperty(ref _selectedVaiTro, value); }

        private string _trangThai;
        public string TrangThai { get => _trangThai; set => SetProperty(ref _trangThai, value); }

        private bool _isEditing;
        public bool IsEditing { get => _isEditing; set => SetProperty(ref _isEditing, value); }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    TimKiem();
            }
        }

        // Danh sách trạng thái
        public ObservableCollection<string> DSTrangThai { get; set; } = new ObservableCollection<string>
        {
            "Hoạt động", "Đã khóa"
        };

        // Commands
        public ICommand ThemCommand { get; }
        public ICommand SuaCommand { get; }
        public ICommand XoaCommand { get; }
        public ICommand DatLaiMatKhauCommand { get; }
        public ICommand LamMoiCommand { get; }
        public ICommand TimKiemCommand { get; }

        public NhanVienViewModel()
        {
            ThemCommand = new RelayCommand(_ => ThemNhanVien());
            SuaCommand = new RelayCommand(_ => SuaNhanVien(), _ => IsEditing);
            XoaCommand = new RelayCommand(_ => XoaNhanVien(), _ => IsEditing);
            DatLaiMatKhauCommand = new RelayCommand(_ => DatLaiMatKhau(), _ => IsEditing);
            LamMoiCommand = new RelayCommand(_ => LamMoi());
            TimKiemCommand = new RelayCommand(_ => TimKiem());

            LoadVaiTro();
            LoadNhanVien();
        }

        private void LoadVaiTro()
        {
            try
            {
                DSVaiTro.Clear();
                var list = _repo.GetAllVaiTro();
                foreach (var vt in list) DSVaiTro.Add(vt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách vai trò: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadNhanVien()
        {
            try
            {
                DSNhanVien.Clear();
                var list = _repo.GetAllNhanVien();
                foreach (var nv in list) DSNhanVien.Add(nv);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách nhân viên: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TimKiem()
        {
            try
            {
                var all = _repo.GetAllNhanVien();
                DSNhanVien.Clear();
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    foreach (var nv in all) DSNhanVien.Add(nv);
                }
                else
                {
                    string kw = SearchText.ToLower();
                    foreach (var nv in all.Where(x =>
                        (x.MaNV?.ToLower().Contains(kw) == true) ||
                        (x.HoTen?.ToLower().Contains(kw) == true) ||
                        (x.TenDangNhap?.ToLower().Contains(kw) == true) ||
                        (x.SDT?.ToLower().Contains(kw) == true) ||
                        (x.TenVaiTro?.ToLower().Contains(kw) == true)))
                    {
                        DSNhanVien.Add(nv);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(HoTen))
            {
                MessageBox.Show("Vui lòng nhập họ tên nhân viên!", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(TenDangNhap))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập!", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (SelectedVaiTro == null)
            {
                MessageBox.Show("Vui lòng chọn vai trò cho nhân viên!", "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void ThemNhanVien()
        {
            if (!ValidateForm()) return;

            try
            {
                var nv = new NhanVienModel
                {
                    MaNV = _repo.GenerateMaNV(),
                    HoTen = HoTen.Trim(),
                    NgaySinh = NgaySinh,
                    SDT = SDT?.Trim(),
                    Email = Email?.Trim(),
                    TenDangNhap = TenDangNhap.Trim(),
                    MatKhau = SecurityHelper.HashPassword("123456"), // Mật khẩu mặc định
                    MaVaiTro = SelectedVaiTro.MaVaiTro,
                    TrangThai = TrangThai ?? "Hoạt động"
                };

                if (_repo.InsertNhanVien(nv))
                {
                    MessageBox.Show($"Thêm nhân viên thành công!\n\nMã NV: {nv.MaNV}\nMật khẩu mặc định: 123456\n\nVui lòng đổi mật khẩu sau khi đăng nhập.",
                        "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadNhanVien();
                    LamMoi();
                }
                else
                {
                    MessageBox.Show("Thêm nhân viên thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm nhân viên: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SuaNhanVien()
        {
            if (!ValidateForm() || SelectedNhanVien == null) return;

            if (MessageBox.Show($"Bạn có chắc muốn cập nhật thông tin nhân viên {MaNV}?",
                "Xác nhận cập nhật", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            try
            {
                var nv = new NhanVienModel
                {
                    MaNV = MaNV,
                    HoTen = HoTen.Trim(),
                    NgaySinh = NgaySinh,
                    SDT = SDT?.Trim(),
                    Email = Email?.Trim(),
                    TenDangNhap = TenDangNhap.Trim(),
                    MaVaiTro = SelectedVaiTro.MaVaiTro,
                    TrangThai = TrangThai ?? "Hoạt động"
                };

                if (_repo.UpdateNhanVien(nv))
                {
                    MessageBox.Show("Cập nhật thông tin nhân viên thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadNhanVien();
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void XoaNhanVien()
        {
            if (SelectedNhanVien == null) return;

            if (MessageBox.Show($"Bạn có chắc muốn KHÓA tài khoản nhân viên:\n{MaNV} - {HoTen}?\n\nTài khoản sẽ không thể đăng nhập sau khi bị khóa.",
                "Xác nhận khóa tài khoản", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            try
            {
                if (_repo.DeleteNhanVien(MaNV))
                {
                    MessageBox.Show("Đã khóa tài khoản nhân viên thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadNhanVien();
                    LamMoi();
                }
                else
                {
                    MessageBox.Show("Khóa tài khoản thất bại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DatLaiMatKhau()
        {
            if (SelectedNhanVien == null) return;

            if (MessageBox.Show($"Đặt lại mật khẩu cho nhân viên {MaNV} - {HoTen}?\n\nMật khẩu mới sẽ là: 123456",
                "Xác nhận đặt lại mật khẩu", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            try
            {
                string newHash = SecurityHelper.HashPassword("123456");
                _repo.UpdatePassword(MaNV, newHash);
                MessageBox.Show("Đặt lại mật khẩu thành công!\nMật khẩu mới: 123456", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi đặt lại mật khẩu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LamMoi()
        {
            MaNV = string.Empty;
            HoTen = string.Empty;
            NgaySinh = null;
            SDT = string.Empty;
            Email = string.Empty;
            TenDangNhap = string.Empty;
            SelectedVaiTro = null;
            TrangThai = "Hoạt động";
            SelectedNhanVien = null;
            IsEditing = false;
            SearchText = string.Empty;
        }
    }
}
