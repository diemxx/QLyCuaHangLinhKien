using System.Windows;
using ComputerStore_WPF.ViewModels;

namespace ComputerStore_WPF.Views
{
    public partial class ThanhToanTraGopWindow : Window
    {
        public TraGopViewModel ViewModel => DataContext as TraGopViewModel;

        public ThanhToanTraGopWindow(decimal tongTien)
        {
            InitializeComponent();
            DataContext = new TraGopViewModel(5000000);
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            var vm = ViewModel;
            if (vm == null) return;

            if (vm.SoThang <= 0)
            {
                MessageBox.Show("Số tháng trả góp phải lớn hơn 0!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (vm.TienTraTruoc < 0)
            {
                MessageBox.Show("Tiền trả trước không được âm!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (vm.TienTraTruoc >= vm.TongTienHang)
            {
                MessageBox.Show("Tiền trả trước phải nhỏ hơn tổng tiền hàng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Xác nhận trả góp:\n" +
                $"  • Tổng tiền hàng: {vm.TongTienHang:#,##0} VNĐ\n" +
                $"  • Trả trước: {vm.TienTraTruoc:#,##0} VNĐ\n" +
                $"  • Số tháng: {vm.SoThang}\n" +
                $"  • Lãi suất: {vm.LaiSuat}%/tháng\n" +
                $"  • Trả mỗi tháng: {vm.TienTraMoiThang:#,##0} VNĐ\n" +
                $"  • Tổng phải trả: {vm.TongTienPhaiTra:#,##0} VNĐ\n\n" +
                "Bạn có chắc chắn muốn xác nhận?",
                "Xác nhận trả góp",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                DialogResult = true;
                Close();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
