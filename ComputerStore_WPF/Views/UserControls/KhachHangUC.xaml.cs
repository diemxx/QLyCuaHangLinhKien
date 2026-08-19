using System;
using System.Windows;
using System.Windows.Controls;
using ComputerStore_WPF.Models;
using ComputerStore_WPF.Repositories;

namespace ComputerStore_WPF.Views.UserControls
{
    public partial class KhachHangUC : UserControl
    {
        private readonly HoaDonRepository _repo = new HoaDonRepository();

        public KhachHangUC()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try { dgKhachHang.ItemsSource = _repo.GetAllKhachHang(); }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void BtnThemKH_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenKH.Text)) { MessageBox.Show("Nhập tên khách hàng!"); return; }
            try
            {
                string maKH = string.IsNullOrWhiteSpace(txtMaKH.Text) ? _repo.GenerateMaKH() : txtMaKH.Text;
                _repo.InsertKhachHang(new KhachHangModel
                {
                    MaKH = maKH,
                    TenKH = txtTenKH.Text,
                    SDT = txtSDT.Text,
                    Email = txtEmail.Text,
                    DiaChi = txtDiaChi.Text,
                    DiemTichLuy = 0
                });
                MessageBox.Show("Thêm khách hàng thành công!");
                txtMaKH.Text = txtTenKH.Text = txtSDT.Text = txtEmail.Text = txtDiaChi.Text = "";
                LoadData();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }
    }
}
