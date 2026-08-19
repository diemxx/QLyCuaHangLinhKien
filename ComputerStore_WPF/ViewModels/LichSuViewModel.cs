using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ComputerStore_WPF.Models;
using ComputerStore_WPF.Repositories;
using ComputerStore_WPF.ViewModels.Base;

namespace ComputerStore_WPF.ViewModels
{
    public class LichSuViewModel : ViewModelBase
    {
        private readonly LogRepository _logRepo = new LogRepository();
        public ObservableCollection<LichSuTimKiemModel> DanhSach { get; set; } = new ObservableCollection<LichSuTimKiemModel>();

        public ICommand RefreshCommand { get; }

        public LichSuViewModel()
        {
            RefreshCommand = new RelayCommand(_ => Load()); // load lại dữ liệu khi nhấn phím refesh
            Load(); //load data ban đầu
        }

        private void Load()
        {
            DanhSach.Clear();
            foreach (var item in _logRepo.GetAll()) DanhSach.Add(item);
        }
    }
}
