using System.Windows;
using ComputerStore_WPF.ViewModels;

namespace ComputerStore_WPF.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly RegisterViewModel _viewModel;

        public RegisterWindow()
        {
            InitializeComponent();
            _viewModel = new RegisterViewModel();
            DataContext = _viewModel;
            _viewModel.RegisterSuccess += OnRegisterSuccess;
            txtHoTen.Focus();
        }

        private void TxtMatKhau_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.MatKhau = txtMatKhau.Password;
        }

        private void TxtXacNhanMatKhau_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.XacNhanMatKhau = txtXacNhanMatKhau.Password;
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.RegisterCommand.Execute(null);
        }

        private void BtnBackToLogin_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void OnRegisterSuccess()
        {
            System.Windows.MessageBox.Show(
                "Đăng ký tài khoản thành công!\nVui lòng đăng nhập để tiếp tục.",
                "Thành công",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);

            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
