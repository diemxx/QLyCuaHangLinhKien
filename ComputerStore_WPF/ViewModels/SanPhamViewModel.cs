using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ComputerStore_WPF.Models;
using ComputerStore_WPF.Repositories;
using ComputerStore_WPF.ViewModels.Base;
using Microsoft.Win32;

namespace ComputerStore_WPF.ViewModels
{
    public class SanPhamViewModel : ValidatableViewModelBase
    {
        private readonly SanPhamRepository _spRepo = new SanPhamRepository();
        private readonly LogRepository _logRepo = new LogRepository();
        private readonly NhanVienModel _currentUser;

        // Collections
        public ObservableCollection<SanPhamModel> DanhSachSanPham { get; set; } = new ObservableCollection<SanPhamModel>();
        public ObservableCollection<LoaiSanPhamModel> DanhSachLoai { get; set; } = new ObservableCollection<LoaiSanPhamModel>();
        public ObservableCollection<NhaCungCapModel> DanhSachNCC { get; set; } = new ObservableCollection<NhaCungCapModel>();

        // Selected
        private SanPhamModel _selectedSanPham;
        public SanPhamModel SelectedSanPham
        {
            get => _selectedSanPham;
            set { SetProperty(ref _selectedSanPham, value); LoadSelectedToForm(); }
        }

        // Form fields
        private string _maSP, _tenSP, _maLoai, _maNCC, _hinhAnh, _thongSo, _trangThai;
        private decimal _giaNhap, _giaBan;
        private int _soLuongTon, _baoHanh;

        public string MaSP { get => _maSP; set => SetProperty(ref _maSP, value); }
        public string TenSP { get => _tenSP; set { SetProperty(ref _tenSP, value); ValidateTenSP(); } }
        public string MaLoai { get => _maLoai; set => SetProperty(ref _maLoai, value); }
        public string MaNCC { get => _maNCC; set => SetProperty(ref _maNCC, value); }
        public string HinhAnh { get => _hinhAnh; set => SetProperty(ref _hinhAnh, value); }
        public string ThongSo { get => _thongSo; set => SetProperty(ref _thongSo, value); }
        public decimal GiaNhap { get => _giaNhap; set => SetProperty(ref _giaNhap, value); }
        public decimal GiaBan { get => _giaBan; set { SetProperty(ref _giaBan, value); ValidateGiaBan(); } }
        public int SoLuongTon { get => _soLuongTon; set => SetProperty(ref _soLuongTon, value); }
        public int BaoHanh { get => _baoHanh; set => SetProperty(ref _baoHanh, value); }
        public string TrangThai { get => _trangThai; set => SetProperty(ref _trangThai, value); }

        // Search
        private string _searchKeyword;
        public string SearchKeyword { get => _searchKeyword; set => SetProperty(ref _searchKeyword, value); }

        private string _filterLoai;
        public string FilterLoai { get => _filterLoai; set => SetProperty(ref _filterLoai, value); }

        // Phân quyền
        public bool CanDelete => _currentUser?.MaVaiTro == "VT01";
        public bool CanEdit => _currentUser?.MaVaiTro == "VT01" || _currentUser?.MaVaiTro == "VT03";

        // Loại SP form
        private string _newMaLoai, _newTenLoai, _newMoTaLoai;
        public string NewMaLoai { get => _newMaLoai; set => SetProperty(ref _newMaLoai, value); }
        public string NewTenLoai { get => _newTenLoai; set => SetProperty(ref _newTenLoai, value); }
        public string NewMoTaLoai { get => _newMoTaLoai; set => SetProperty(ref _newMoTaLoai, value); }

        // NCC form
        private string _newMaNCC, _newTenNCC, _newSDTNCC, _newDiaChiNCC, _newEmailNCC;
        public string NewMaNCC { get => _newMaNCC; set => SetProperty(ref _newMaNCC, value); }
        public string NewTenNCC { get => _newTenNCC; set => SetProperty(ref _newTenNCC, value); }
        public string NewSDTNCC { get => _newSDTNCC; set => SetProperty(ref _newSDTNCC, value); }
        public string NewDiaChiNCC { get => _newDiaChiNCC; set => SetProperty(ref _newDiaChiNCC, value); }
        public string NewEmailNCC { get => _newEmailNCC; set => SetProperty(ref _newEmailNCC, value); }

        // Commands
        public ICommand SearchCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand SelectImageCommand { get; }
        public ICommand AddLoaiCommand { get; }
        public ICommand AddNCCCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand GenerateMaSPCommand { get; }

        public SanPhamViewModel(NhanVienModel currentUser)
        {
            _currentUser = currentUser;
            SearchCommand = new RelayCommand(_ => Search());
            AddCommand = new RelayCommand(_ => AddSanPham(), _ => CanEdit);
            UpdateCommand = new RelayCommand(_ => UpdateSanPham(), _ => CanEdit);
            DeleteCommand = new RelayCommand(_ => DeleteSanPham(), _ => CanDelete);
            ClearCommand = new RelayCommand(_ => ClearForm());
            SelectImageCommand = new RelayCommand(_ => SelectImage());
            AddLoaiCommand = new RelayCommand(_ => AddLoai());
            AddNCCCommand = new RelayCommand(_ => AddNCC());
            RefreshCommand = new RelayCommand(_ => LoadData());
            GenerateMaSPCommand = new RelayCommand(_ => GenerateMaSP());
            BaoHanh = 12;
            TrangThai = "Đang kinh doanh";
            LoadData();
        }

        private void GenerateMaSP()
        {
            try
            {
                MaSP = _spRepo.GenerateMaSP();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tạo mã SP: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadData()
        {
            try
            {
                DanhSachSanPham.Clear();
                foreach (var sp in _spRepo.GetAll()) DanhSachSanPham.Add(sp);

                DanhSachLoai.Clear();
                foreach (var l in _spRepo.GetAllLoaiSanPham()) DanhSachLoai.Add(l);

                DanhSachNCC.Clear();
                foreach (var n in _spRepo.GetAllNhaCungCap()) DanhSachNCC.Add(n);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Search()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchKeyword) && string.IsNullOrWhiteSpace(FilterLoai))
                {
                    LoadData();
                    return;
                }

                var results = _spRepo.Search(SearchKeyword, FilterLoai);
                DanhSachSanPham.Clear();
                foreach (var sp in results) DanhSachSanPham.Add(sp);

                if (!string.IsNullOrWhiteSpace(SearchKeyword))
                    _logRepo.InsertLog(SearchKeyword, _currentUser.MaNV);

                if (DanhSachSanPham.Count == 0)
                    MessageBox.Show("Không tìm thấy sản phẩm nào!", "Kết quả", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddSanPham()
        {
            // Auto-generate MaSP nếu trống
            if (string.IsNullOrWhiteSpace(MaSP))
            {
                try { MaSP = _spRepo.GenerateMaSP(); }
                catch (Exception ex) { MessageBox.Show("Lỗi tạo mã SP: " + ex.Message); return; }
            }

            if (!ValidateForm()) return;
            try
            {
                // Kiểm tra trùng mã SP
                var existing = _spRepo.GetById(MaSP);
                if (existing != null)
                {
                    MessageBox.Show($"Mã SP '{MaSP}' đã tồn tại! Vui lòng đổi mã khác.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var sp = new SanPhamModel
                {
                    MaSP = MaSP,
                    TenSP = TenSP,
                    MaLoai = MaLoai,
                    MaNCC = MaNCC,
                    HinhAnh = HinhAnh,
                    ThongSoKyThuat = ThongSo,
                    GiaNhap = GiaNhap,
                    GiaBan = GiaBan,
                    SoLuongTon = SoLuongTon,
                    BaoHanh = BaoHanh,
                    TrangThai = TrangThai ?? "Đang kinh doanh"
                };
                if (_spRepo.Insert(sp))
                {
                    MessageBox.Show("Thêm sản phẩm thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                    ClearForm();
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void UpdateSanPham()
        {
            if (SelectedSanPham == null) { MessageBox.Show("Vui lòng chọn sản phẩm cần sửa!"); return; }
            if (!ValidateForm()) return;
            try
            {
                var sp = new SanPhamModel
                {
                    MaSP = MaSP,
                    TenSP = TenSP,
                    MaLoai = MaLoai,
                    MaNCC = MaNCC,
                    HinhAnh = HinhAnh,
                    ThongSoKyThuat = ThongSo,
                    GiaNhap = GiaNhap,
                    GiaBan = GiaBan,
                    SoLuongTon = SoLuongTon,
                    BaoHanh = BaoHanh,
                    TrangThai = TrangThai
                };
                if (_spRepo.Update(sp))
                {
                    MessageBox.Show("Cập nhật sản phẩm thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void DeleteSanPham()
        {
            if (SelectedSanPham == null) { MessageBox.Show("Vui lòng chọn sản phẩm cần xóa!"); return; }
            if (MessageBox.Show($"Bạn có chắc muốn ngừng kinh doanh sản phẩm '{SelectedSanPham.TenSP}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    if (_spRepo.Delete(SelectedSanPham.MaSP))
                    {
                        MessageBox.Show("Đã ngừng kinh doanh sản phẩm!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData(); ClearForm();
                    }
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error); }
            }
        }

        private void SelectImage()
        {
            var dlg = new OpenFileDialog { Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp;*.gif", Title = "Chọn hình ảnh sản phẩm" };
            if (dlg.ShowDialog() == true) HinhAnh = dlg.FileName;
        }

        private void LoadSelectedToForm()
        {
            if (SelectedSanPham == null) return;
            MaSP = SelectedSanPham.MaSP; TenSP = SelectedSanPham.TenSP;
            MaLoai = SelectedSanPham.MaLoai; MaNCC = SelectedSanPham.MaNCC;
            HinhAnh = SelectedSanPham.HinhAnh; ThongSo = SelectedSanPham.ThongSoKyThuat;
            GiaNhap = SelectedSanPham.GiaNhap; GiaBan = SelectedSanPham.GiaBan;
            SoLuongTon = SelectedSanPham.SoLuongTon; BaoHanh = SelectedSanPham.BaoHanh;
            TrangThai = SelectedSanPham.TrangThai;
        }

        private void ClearForm()
        {
            MaSP = TenSP = MaLoai = MaNCC = HinhAnh = ThongSo = "";
            GiaNhap = GiaBan = 0; SoLuongTon = 0; BaoHanh = 12;
            TrangThai = "Đang kinh doanh"; SelectedSanPham = null;
            ClearAllErrors();
        }

        private bool ValidateForm()
        {
            ClearAllErrors();
            bool valid = true;
            if (string.IsNullOrWhiteSpace(TenSP)) { AddError(nameof(TenSP), "Tên SP không được trống!"); valid = false; }
            if (string.IsNullOrWhiteSpace(MaLoai)) { AddError(nameof(MaLoai), "Chưa chọn loại!"); valid = false; }
            if (string.IsNullOrWhiteSpace(MaNCC)) { AddError(nameof(MaNCC), "Chưa chọn NCC!"); valid = false; }
            if (GiaBan <= 0) { AddError(nameof(GiaBan), "Giá bán phải > 0!"); valid = false; }
            if (GiaNhap <= 0) { AddError(nameof(GiaNhap), "Giá nhập phải > 0!"); valid = false; }
            if (GiaBan < GiaNhap) { AddError(nameof(GiaBan), "Giá bán phải >= giá nhập!"); valid = false; }
            if (!valid) MessageBox.Show("Vui lòng kiểm tra lại thông tin nhập!", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return valid;
        }

        private void ValidateTenSP() { ClearErrors(nameof(TenSP)); if (string.IsNullOrWhiteSpace(TenSP)) AddError(nameof(TenSP), "Tên SP không được trống!"); }
        private void ValidateGiaBan() { ClearErrors(nameof(GiaBan)); if (GiaBan <= 0) AddError(nameof(GiaBan), "Giá bán phải > 0!"); }

        private void AddLoai()
        {
            if (string.IsNullOrWhiteSpace(NewMaLoai) || string.IsNullOrWhiteSpace(NewTenLoai))
            { MessageBox.Show("Vui lòng nhập mã và tên loại sản phẩm!", "Validation"); return; }
            try
            {
                _spRepo.InsertLoaiSanPham(new LoaiSanPhamModel { MaLoai = NewMaLoai, TenLoai = NewTenLoai, MoTa = NewMoTaLoai });
                MessageBox.Show("Thêm loại sản phẩm thành công!");
                DanhSachLoai.Clear();
                foreach (var l in _spRepo.GetAllLoaiSanPham()) DanhSachLoai.Add(l);
                NewMaLoai = NewTenLoai = NewMoTaLoai = "";
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void AddNCC()
        {
            if (string.IsNullOrWhiteSpace(NewMaNCC) || string.IsNullOrWhiteSpace(NewTenNCC))
            { MessageBox.Show("Vui lòng nhập mã và tên NCC!", "Validation"); return; }
            try
            {
                _spRepo.InsertNhaCungCap(new NhaCungCapModel { MaNCC = NewMaNCC, TenNCC = NewTenNCC, SDT = NewSDTNCC, DiaChi = NewDiaChiNCC, Email = NewEmailNCC });
                MessageBox.Show("Thêm nhà cung cấp thành công!");
                DanhSachNCC.Clear();
                foreach (var n in _spRepo.GetAllNhaCungCap()) DanhSachNCC.Add(n);
                NewMaNCC = NewTenNCC = NewSDTNCC = NewDiaChiNCC = NewEmailNCC = "";
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }
    }
}
