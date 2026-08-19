using System;
using System.Text.RegularExpressions;
using System.Windows.Input;
using ComputerStore_WPF.Models;
using ComputerStore_WPF.Repositories;
using ComputerStore_WPF.Utilities;
using ComputerStore_WPF.ViewModels.Base;

namespace ComputerStore_WPF.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {
        private readonly HoaDonRepository _repo = new HoaDonRepository();

        // ===== Properties =====
        private string _hoTen;
        public string HoTen
        {
            get => _hoTen;
            set
            {
                SetProperty(ref _hoTen, value);
                ValidateHoTen();
            }
        }

        private string _sdt;
        public string SDT
        {
            get => _sdt;
            set
            {
                SetProperty(ref _sdt, value);
                ValidateSDT();
            }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                SetProperty(ref _email, value);
                ValidateEmail();
            }
        }

        private string _tenDangNhap;
        public string TenDangNhap
        {
            get => _tenDangNhap;
            set
            {
                SetProperty(ref _tenDangNhap, value);
                ValidateTenDangNhap();
            }
        }

        private string _matKhau;
        public string MatKhau
        {
            get => _matKhau;
            set
            {
                SetProperty(ref _matKhau, value);
                ValidateMatKhau();
                // Validate lại xác nhận nếu đã nhập
                if (!string.IsNullOrEmpty(_xacNhanMatKhau))
                    ValidateXacNhanMatKhau();
            }
        }

        private string _xacNhanMatKhau;
        public string XacNhanMatKhau
        {
            get => _xacNhanMatKhau;
            set
            {
                SetProperty(ref _xacNhanMatKhau, value);
                ValidateXacNhanMatKhau();
            }
        }

        // ===== Error Messages =====
        private string _hoTenError;
        public string HoTenError { get => _hoTenError; set => SetProperty(ref _hoTenError, value); }

        private string _sdtError;
        public string SDTError { get => _sdtError; set => SetProperty(ref _sdtError, value); }

        private string _emailError;
        public string EmailError { get => _emailError; set => SetProperty(ref _emailError, value); }

        private string _tenDangNhapError;
        public string TenDangNhapError { get => _tenDangNhapError; set => SetProperty(ref _tenDangNhapError, value); }

        private string _matKhauError;
        public string MatKhauError { get => _matKhauError; set => SetProperty(ref _matKhauError, value); }

        private string _xacNhanMatKhauError;
        public string XacNhanMatKhauError { get => _xacNhanMatKhauError; set => SetProperty(ref _xacNhanMatKhauError, value); }

        private string _errorMessage;
        public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

        private string _successMessage;
        public string SuccessMessage { get => _successMessage; set => SetProperty(ref _successMessage, value); }

        // ===== Commands =====
        public ICommand RegisterCommand { get; }

        // ===== Events =====
        public event Action RegisterSuccess;

        public RegisterViewModel()
        {
            RegisterCommand = new RelayCommand(ExecuteRegister, CanRegister);
        }

        // ===== Validation =====
        private void ValidateHoTen()
        {
            if (string.IsNullOrWhiteSpace(HoTen))
                HoTenError = "Vui lòng nhập họ và tên!";
            else if (HoTen.Trim().Length < 2)
                HoTenError = "Họ và tên phải có ít nhất 2 ký tự!";
            else if (HoTen.Trim().Length > 100)
                HoTenError = "Họ và tên không được vượt quá 100 ký tự!";
            else
                HoTenError = string.Empty;
        }

        private void ValidateSDT()
        {
            if (string.IsNullOrWhiteSpace(SDT))
            {
                SDTError = "Vui lòng nhập số điện thoại!";
                return;
            }
            // Số điện thoại Việt Nam: bắt đầu bằng 0, 10 chữ số, hoặc +84 9/10 chữ số
            var regex = new Regex(@"^(0[3|5|7|8|9])[0-9]{8}$|^(84[3|5|7|8|9])[0-9]{8}$|^\+84[3|5|7|8|9][0-9]{8}$");
            if (!regex.IsMatch(SDT.Trim()))
                SDTError = "Số điện thoại không hợp lệ! (VD: 0912345678)";
            else
                SDTError = string.Empty;
        }

        private void ValidateEmail()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                EmailError = "Vui lòng nhập địa chỉ email!";
                return;
            }
            var regex = new Regex(@"^[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}$");
            if (!regex.IsMatch(Email.Trim()))
                EmailError = "Địa chỉ email không hợp lệ! (VD: example@gmail.com)";
            else
                EmailError = string.Empty;
        }

        private void ValidateTenDangNhap()
        {
            if (string.IsNullOrWhiteSpace(TenDangNhap))
            {
                TenDangNhapError = "Vui lòng nhập tên đăng nhập!";
                return;
            }
            if (TenDangNhap.Trim().Length < 4)
            {
                TenDangNhapError = "Tên đăng nhập phải có ít nhất 4 ký tự!";
                return;
            }
            if (TenDangNhap.Trim().Length > 50)
            {
                TenDangNhapError = "Tên đăng nhập không được vượt quá 50 ký tự!";
                return;
            }
            // Chỉ chứa chữ cái, số, dấu gạch dưới
            var regex = new Regex(@"^[a-zA-Z0-9_]+$");
            if (!regex.IsMatch(TenDangNhap.Trim()))
                TenDangNhapError = "Tên đăng nhập chỉ được chứa chữ cái, số và dấu _!";
            else
                TenDangNhapError = string.Empty;
        }

        private void ValidateMatKhau()
        {
            if (string.IsNullOrEmpty(MatKhau))
            {
                MatKhauError = "Vui lòng nhập mật khẩu!";
                return;
            }
            if (MatKhau.Length < 8)
            {
                MatKhauError = "Mật khẩu phải có ít nhất 8 ký tự!";
                return;
            }
            // Phải có chữ hoa, chữ thường và số
            if (!Regex.IsMatch(MatKhau, @"[A-Z]"))
            {
                MatKhauError = "Mật khẩu phải có ít nhất 1 chữ HOA!";
                return;
            }
            if (!Regex.IsMatch(MatKhau, @"[a-z]"))
            {
                MatKhauError = "Mật khẩu phải có ít nhất 1 chữ thường!";
                return;
            }
            if (!Regex.IsMatch(MatKhau, @"[0-9]"))
            {
                MatKhauError = "Mật khẩu phải có ít nhất 1 chữ số!";
                return;
            }
            MatKhauError = string.Empty;
        }

        private void ValidateXacNhanMatKhau()
        {
            if (string.IsNullOrEmpty(XacNhanMatKhau))
                XacNhanMatKhauError = "Vui lòng xác nhận mật khẩu!";
            else if (XacNhanMatKhau != MatKhau)
                XacNhanMatKhauError = "Mật khẩu xác nhận không khớp!";
            else
                XacNhanMatKhauError = string.Empty;
        }

        private bool IsFormValid()
        {
            // Trigger tất cả validate
            ValidateHoTen();
            ValidateSDT();
            ValidateEmail();
            ValidateTenDangNhap();
            ValidateMatKhau();
            ValidateXacNhanMatKhau();

            return string.IsNullOrEmpty(HoTenError)
                && string.IsNullOrEmpty(SDTError)
                && string.IsNullOrEmpty(EmailError)
                && string.IsNullOrEmpty(TenDangNhapError)
                && string.IsNullOrEmpty(MatKhauError)
                && string.IsNullOrEmpty(XacNhanMatKhauError);
        }

        private bool CanRegister(object parameter) => true;

        private void ExecuteRegister(object parameter)
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (!IsFormValid()) return;

            try
            {
                // Kiểm tra tên đăng nhập đã tồn tại
                if (_repo.IsTenDangNhapExists(TenDangNhap.Trim()))
                {
                    TenDangNhapError = "Tên đăng nhập này đã được sử dụng!";
                    return;
                }

                // Kiểm tra email đã tồn tại
                if (_repo.IsEmailNhanVienExists(Email.Trim()))
                {
                    EmailError = "Email này đã được đăng ký!";
                    return;
                }

                string maNV = _repo.GenerateMaNV();
                string hashedPassword = SecurityHelper.HashPassword(MatKhau);

                var nhanVien = new NhanVienModel
                {
                    MaNV = maNV,
                    HoTen = HoTen.Trim(),
                    SDT = SDT.Trim(),
                    Email = Email.Trim(),
                    TenDangNhap = TenDangNhap.Trim(),
                    MatKhau = hashedPassword,
                    MaVaiTro = "VT02", // Mặc định là Nhân viên
                    TrangThai = "Hoạt động"
                };

                bool success = _repo.InsertNhanVien(nhanVien);
                if (success)
                {
                    RegisterSuccess?.Invoke();
                }
                else
                {
                    ErrorMessage = "Đăng ký thất bại! Vui lòng thử lại.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Lỗi: " + ex.Message;
            }
        }
    }
}
