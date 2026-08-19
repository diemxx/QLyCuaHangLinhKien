using System.Windows;
using ComputerStore_WPF.ViewModels;

namespace ComputerStore_WPF.Views
{
    public partial class ForgotPasswordWindow : Window
    {
        private readonly ForgotPasswordViewModel _viewModel;

        public ForgotPasswordWindow()
        {
            InitializeComponent();
            _viewModel = new ForgotPasswordViewModel();
            DataContext = _viewModel;
            _viewModel.ResetSuccess += OnResetSuccess;
            txtTenDangNhap.Focus();
        }

        private void BtnVerify_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.VerifyCommand.Execute(null);
        }

        private void TxtMatKhauMoi_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.MatKhauMoi = txtMatKhauMoi.Password;
        }

        private void TxtXacNhanMatKhau_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.XacNhanMatKhau = txtXacNhanMatKhau.Password;
        }

        private void BtnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ResetPasswordCommand.Execute(null);
        }

        private void BtnBackToLogin_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void OnResetSuccess()
        {
            System.Windows.MessageBox.Show(
                "Đặt lại mật khẩu thành công!\nVui lòng đăng nhập với mật khẩu mới.",
                "Thành công",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);

            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
