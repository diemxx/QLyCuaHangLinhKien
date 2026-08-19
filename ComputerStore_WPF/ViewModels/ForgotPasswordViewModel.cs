using System;
using System.Text.RegularExpressions;
using System.Windows.Input;
using ComputerStore_WPF.Repositories;
using ComputerStore_WPF.Utilities;
using ComputerStore_WPF.ViewModels.Base;

namespace ComputerStore_WPF.ViewModels
{
    public class ForgotPasswordViewModel : ViewModelBase
    {
        private readonly HoaDonRepository _repo = new HoaDonRepository();

        // ===== Bước 1: Xác minh tài khoản =====
        private string _tenDangNhap;
        public string TenDangNhap
        {
            get => _tenDangNhap;
            set
            {
                SetProperty(ref _tenDangNhap, value);
                TenDangNhapError = string.Empty;
            }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                SetProperty(ref _email, value);
                EmailError = string.Empty;
            }
        }

        // ===== Bước 2: Đặt lại mật khẩu =====
        private string _matKhauMoi;
        public string MatKhauMoi
        {
            get => _matKhauMoi;
            set
            {
                SetProperty(ref _matKhauMoi, value);
                ValidateMatKhauMoi();
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
        private string _tenDangNhapError;
        public string TenDangNhapError { get => _tenDangNhapError; set => SetProperty(ref _tenDangNhapError, value); }

        private string _emailError;
        public string EmailError { get => _emailError; set => SetProperty(ref _emailError, value); }

        private string _matKhauMoiError;
        public string MatKhauMoiError { get => _matKhauMoiError; set => SetProperty(ref _matKhauMoiError, value); }

        private string _xacNhanMatKhauError;
        public string XacNhanMatKhauError { get => _xacNhanMatKhauError; set => SetProperty(ref _xacNhanMatKhauError, value); }

        private string _errorMessage;
        public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

        private string _successMessage;
        public string SuccessMessage { get => _successMessage; set => SetProperty(ref _successMessage, value); }

        private string _verifiedInfo;
        public string VerifiedInfo { get => _verifiedInfo; set => SetProperty(ref _verifiedInfo, value); }

        // ===== Step Control =====
        private bool _isStep1 = true;
        public bool IsStep1 { get => _isStep1; set => SetProperty(ref _isStep1, value); }

        private bool _isStep2 = false;
        public bool IsStep2 { get => _isStep2; set => SetProperty(ref _isStep2, value); }

        // ===== Internal State =====
        private string _maNVVerified; // Mã NV đã xác minh thành công

        // ===== Commands =====
        public ICommand VerifyCommand { get; }
        public ICommand ResetPasswordCommand { get; }

        // ===== Events =====
        public event Action ResetSuccess;

        public ForgotPasswordViewModel()
        {
            VerifyCommand = new RelayCommand(ExecuteVerify, _ => true);
            ResetPasswordCommand = new RelayCommand(ExecuteResetPassword, _ => true);
        }

        // ===== Validation =====
        private bool ValidateStep1()
        {
            bool valid = true;
            if (string.IsNullOrWhiteSpace(TenDangNhap))
            {
                TenDangNhapError = "Vui lòng nhập tên đăng nhập!";
                valid = false;
            }
            else
            {
                TenDangNhapError = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                EmailError = "Vui lòng nhập địa chỉ email!";
                valid = false;
            }
            else
            {
                var regex = new Regex(@"^[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}$");
                if (!regex.IsMatch(Email.Trim()))
                {
                    EmailError = "Địa chỉ email không hợp lệ!";
                    valid = false;
                }
                else
                {
                    EmailError = string.Empty;
                }
            }
            return valid;
        }

        private void ValidateMatKhauMoi()
        {
            if (string.IsNullOrEmpty(MatKhauMoi))
            {
                MatKhauMoiError = "Vui lòng nhập mật khẩu mới!";
                return;
            }
            if (MatKhauMoi.Length < 8)
            {
                MatKhauMoiError = "Mật khẩu phải có ít nhất 8 ký tự!";
                return;
            }
            if (!Regex.IsMatch(MatKhauMoi, @"[A-Z]"))
            {
                MatKhauMoiError = "Mật khẩu phải có ít nhất 1 chữ HOA!";
                return;
            }
            if (!Regex.IsMatch(MatKhauMoi, @"[a-z]"))
            {
                MatKhauMoiError = "Mật khẩu phải có ít nhất 1 chữ thường!";
                return;
            }
            if (!Regex.IsMatch(MatKhauMoi, @"[0-9]"))
            {
                MatKhauMoiError = "Mật khẩu phải có ít nhất 1 chữ số!";
                return;
            }
            MatKhauMoiError = string.Empty;
        }

        private void ValidateXacNhanMatKhau()
        {
            if (string.IsNullOrEmpty(XacNhanMatKhau))
                XacNhanMatKhauError = "Vui lòng xác nhận mật khẩu mới!";
            else if (XacNhanMatKhau != MatKhauMoi)
                XacNhanMatKhauError = "Mật khẩu xác nhận không khớp!";
            else
                XacNhanMatKhauError = string.Empty;
        }

        // ===== Execute =====
        private void ExecuteVerify(object parameter)
        {
            ErrorMessage = string.Empty;

            if (!ValidateStep1()) return;

            try
            {
                var nhanVien = _repo.GetNhanVienByTenDangNhapAndEmail(TenDangNhap.Trim(), Email.Trim());
                if (nhanVien == null)
                {
                    ErrorMessage = "Tên đăng nhập hoặc email không khớp!\nVui lòng kiểm tra lại.";
                    return;
                }
                if (nhanVien.TrangThai != "Hoạt động")
                {
                    ErrorMessage = "Tài khoản này đã bị khóa, không thể đặt lại mật khẩu!";
                    return;
                }

                _maNVVerified = nhanVien.MaNV;
                VerifiedInfo = $"Xác minh thành công: {nhanVien.HoTen}";

                // Chuyển sang bước 2
                IsStep1 = false;
                IsStep2 = true;
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Lỗi kết nối: " + ex.Message;
            }
        }

        private void ExecuteResetPassword(object parameter)
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            ValidateMatKhauMoi();
            ValidateXacNhanMatKhau();

            if (!string.IsNullOrEmpty(MatKhauMoiError) || !string.IsNullOrEmpty(XacNhanMatKhauError))
                return;

            try
            {
                string newHash = SecurityHelper.HashPassword(MatKhauMoi);
                _repo.UpdatePassword(_maNVVerified, newHash);
                ResetSuccess?.Invoke();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Lỗi khi đặt lại mật khẩu: " + ex.Message;
            }
        }
    }
}
