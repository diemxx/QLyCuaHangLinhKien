using System;
using System.Windows;
using System.Windows.Input;
using ComputerStore_WPF.Models;
using ComputerStore_WPF.Repositories;
using ComputerStore_WPF.Utilities;
using ComputerStore_WPF.ViewModels.Base;

namespace ComputerStore_WPF.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly HoaDonRepository _repo = new HoaDonRepository();

        private string _tenDangNhap;
        public string TenDangNhap
        {
            get => _tenDangNhap;
            set => SetProperty(ref _tenDangNhap, value);
        }

        private string _matKhau;
        public string MatKhau
        {
            get => _matKhau;
            set => SetProperty(ref _matKhau, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand LoginCommand { get; }

        // Event khi đăng nhập thành công
        public event Action<NhanVienModel> LoginSuccess;

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(ExecuteLogin, CanLogin);
        }

        private bool CanLogin(object parameter) => !string.IsNullOrWhiteSpace(TenDangNhap) && !IsLoading;

        private void ExecuteLogin(object parameter)
        {
            ErrorMessage = string.Empty;

            // Lấy mật khẩu từ PasswordBox (truyền qua parameter)
            string password = parameter as string;
            if (string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "Vui lòng nhập mật khẩu!";
                return;
            }

            try
            {
                IsLoading = true;
                string hashedPassword = SecurityHelper.HashPassword(password);

                // Thử đăng nhập bằng mật khẩu đã hash trước
                var nhanVien = _repo.Login(TenDangNhap.Trim(), hashedPassword);

                // Nếu không thành công, thử đăng nhập bằng mật khẩu plaintext
                // (trường hợp DB chưa hash mật khẩu)
                if (nhanVien == null)
                {
                    nhanVien = _repo.LoginPlainText(TenDangNhap.Trim(), password);

                    // Nếu đăng nhập plaintext thành công, tự động cập nhật DB sang hash
                    if (nhanVien != null)
                    {
                        try
                        {
                            _repo.UpdatePassword(nhanVien.MaNV, hashedPassword);
                        }
                        catch { /* Bỏ qua lỗi cập nhật, đăng nhập vẫn thành công */ }
                    }
                }

                if (nhanVien != null)
                {
                    LoginSuccess?.Invoke(nhanVien);
                }
                else
                {
                    ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng!\nHoặc tài khoản đã bị khóa.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Lỗi kết nối: " + ex.Message;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
