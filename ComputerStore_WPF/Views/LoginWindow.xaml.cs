using System.Windows;
using System.Windows.Input;
using ComputerStore_WPF.ViewModels;

namespace ComputerStore_WPF.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _viewModel;

        public LoginWindow()
        {
            InitializeComponent();
            _viewModel = new LoginViewModel();
            DataContext = _viewModel;
            _viewModel.LoginSuccess += OnLoginSuccess;
            txtUsername.Focus();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.LoginCommand.Execute(txtPassword.Password);
        }

        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                _viewModel.LoginCommand.Execute(txtPassword.Password);
        }

        private void OnLoginSuccess(Models.NhanVienModel user)
        {
            var mainWindow = new MainWindow(user);
            mainWindow.Show();
            this.Close();
        }

        private void BtnForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            var forgotWindow = new ForgotPasswordWindow();
            forgotWindow.Show();
            this.Close();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }
    }
}
