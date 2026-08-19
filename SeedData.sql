-- =====================================================
-- Script tạo dữ liệu mẫu cho ComputerStoreDB
-- Chạy script này trên SQL Server Management Studio
-- =====================================================
USE ComputerStoreDB;
GO

-- ============ VaiTro ============
IF NOT EXISTS (SELECT 1 FROM VaiTro WHERE MaVaiTro = 'VT01')
    INSERT INTO VaiTro (MaVaiTro, TenVaiTro, MoTa) VALUES ('VT01', N'Quản lý', N'Quản lý toàn bộ hệ thống');
IF NOT EXISTS (SELECT 1 FROM VaiTro WHERE MaVaiTro = 'VT02')
    INSERT INTO VaiTro (MaVaiTro, TenVaiTro, MoTa) VALUES ('VT02', N'Nhân viên bán hàng', N'Bán hàng, tạo hóa đơn');
IF NOT EXISTS (SELECT 1 FROM VaiTro WHERE MaVaiTro = 'VT03')
    INSERT INTO VaiTro (MaVaiTro, TenVaiTro, MoTa) VALUES ('VT03', N'Thủ kho', N'Quản lý kho, nhập hàng');
GO

-- ============ Nhân viên (mật khẩu plaintext - app sẽ tự hash khi đăng nhập) ============
-- Xóa dữ liệu cũ nếu cần cập nhật
IF NOT EXISTS (SELECT 1 FROM NhanVien WHERE TenDangNhap = 'admin')
    INSERT INTO NhanVien (MaNV, HoTen, NgaySinh, SDT, Email, TenDangNhap, MatKhau, MaVaiTro, TrangThai)
    VALUES ('NV001', N'Nguyễn Văn Admin', '1990-05-15', '0901111111', 'admin@store.com', 'admin', 'admin123', 'VT01', N'Hoạt động');
ELSE
    UPDATE NhanVien SET MatKhau = 'admin123' WHERE TenDangNhap = 'admin';

IF NOT EXISTS (SELECT 1 FROM NhanVien WHERE TenDangNhap = 'nvbh01')
    INSERT INTO NhanVien (MaNV, HoTen, NgaySinh, SDT, Email, TenDangNhap, MatKhau, MaVaiTro, TrangThai)
    VALUES ('NV002', N'Trần Thị Bán Hàng', '1995-08-20', '0902222222', 'banhang@store.com', 'nvbh01', 'nv123', 'VT02', N'Hoạt động');

IF NOT EXISTS (SELECT 1 FROM NhanVien WHERE TenDangNhap = 'thukho01')
    INSERT INTO NhanVien (MaNV, HoTen, NgaySinh, SDT, Email, TenDangNhap, MatKhau, MaVaiTro, TrangThai)
    VALUES ('NV003', N'Lê Văn Thủ Kho', '1992-03-10', '0903333333', 'thukho@store.com', 'thukho01', 'tk123', 'VT03', N'Hoạt động');

IF NOT EXISTS (SELECT 1 FROM NhanVien WHERE TenDangNhap = 'nvbh02')
    INSERT INTO NhanVien (MaNV, HoTen, NgaySinh, SDT, Email, TenDangNhap, MatKhau, MaVaiTro, TrangThai)
    VALUES ('NV004', N'Phạm Minh Tuấn', '1998-11-25', '0904444444', 'tuan@store.com', 'nvbh02', 'nv123', 'VT02', N'Hoạt động');
GO

-- ============ Loại sản phẩm ============
IF NOT EXISTS (SELECT 1 FROM LoaiSanPham WHERE MaLoai = 'LSP01')
BEGIN
    INSERT INTO LoaiSanPham (MaLoai, TenLoai, MoTa) VALUES ('LSP01', N'CPU - Bộ vi xử lý', N'Các loại CPU Intel, AMD');
    INSERT INTO LoaiSanPham (MaLoai, TenLoai, MoTa) VALUES ('LSP02', N'Mainboard', N'Bo mạch chủ các loại');
    INSERT INTO LoaiSanPham (MaLoai, TenLoai, MoTa) VALUES ('LSP03', N'RAM', N'Bộ nhớ RAM DDR4, DDR5');
    INSERT INTO LoaiSanPham (MaLoai, TenLoai, MoTa) VALUES ('LSP04', N'Ổ cứng SSD/HDD', N'Ổ lưu trữ SSD, HDD, NVMe');
    INSERT INTO LoaiSanPham (MaLoai, TenLoai, MoTa) VALUES ('LSP05', N'VGA - Card đồ họa', N'Card màn hình NVIDIA, AMD');
    INSERT INTO LoaiSanPham (MaLoai, TenLoai, MoTa) VALUES ('LSP06', N'Nguồn PSU', N'Bộ nguồn máy tính');
    INSERT INTO LoaiSanPham (MaLoai, TenLoai, MoTa) VALUES ('LSP07', N'Case - Vỏ máy tính', N'Thùng máy tính các loại');
    INSERT INTO LoaiSanPham (MaLoai, TenLoai, MoTa) VALUES ('LSP08', N'Tản nhiệt', N'Tản nhiệt CPU, tản nước');
    INSERT INTO LoaiSanPham (MaLoai, TenLoai, MoTa) VALUES ('LSP09', N'Màn hình', N'Màn hình máy tính các loại');
    INSERT INTO LoaiSanPham (MaLoai, TenLoai, MoTa) VALUES ('LSP10', N'Bàn phím', N'Bàn phím cơ, membrane');
    INSERT INTO LoaiSanPham (MaLoai, TenLoai, MoTa) VALUES ('LSP11', N'Chuột', N'Chuột gaming, văn phòng');
    INSERT INTO LoaiSanPham (MaLoai, TenLoai, MoTa) VALUES ('LSP12', N'Tai nghe', N'Tai nghe gaming, studio');
    INSERT INTO LoaiSanPham (MaLoai, TenLoai, MoTa) VALUES ('LSP13', N'Laptop', N'Laptop các hãng');
END
GO

-- ============ Nhà cung cấp ============
IF NOT EXISTS (SELECT 1 FROM NhaCungCap WHERE MaNCC = 'NCC01')
BEGIN
    INSERT INTO NhaCungCap (MaNCC, TenNCC, SDT, DiaChi, Email, TrangThai) VALUES ('NCC01', N'Phong Vũ Computer', '02871089999', N'Số 1 Lê Duẩn, Q.1, TP.HCM', 'contact@phongvu.vn', N'Hoạt động');
    INSERT INTO NhaCungCap (MaNCC, TenNCC, SDT, DiaChi, Email, TrangThai) VALUES ('NCC02', N'An Phát Computer', '02462554455', N'Số 65 Thái Hà, Đống Đa, Hà Nội', 'info@anphat.com', N'Hoạt động');
    INSERT INTO NhaCungCap (MaNCC, TenNCC, SDT, DiaChi, Email, TrangThai) VALUES ('NCC03', N'Nguyễn Công PC', '02873099999', N'65 Trần Hưng Đạo, Q.1, TP.HCM', 'info@nguyencong.com', N'Hoạt động');
    INSERT INTO NhaCungCap (MaNCC, TenNCC, SDT, DiaChi, Email, TrangThai) VALUES ('NCC04', N'Intel Vietnam', '02838271234', N'KCN Saigon Hi-Tech, Q.9, TP.HCM', 'vn@intel.com', N'Hoạt động');
    INSERT INTO NhaCungCap (MaNCC, TenNCC, SDT, DiaChi, Email, TrangThai) VALUES ('NCC05', N'Synnex FPT', '02873006789', N'Tòa FPT, Đại lộ Thăng Long, Hà Nội', 'sales@synnex.fpt.vn', N'Hoạt động');
END
GO

-- ============ Sản phẩm linh kiện (30+ sản phẩm) ============
IF NOT EXISTS (SELECT 1 FROM SanPham WHERE MaSP = 'SP001')
BEGIN
    -- CPU
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP001', N'CPU Intel Core i5-13400F', 'LSP01', 'NCC04', N'10C/16T, 2.5GHz - 4.6GHz, Socket LGA 1700, 65W', 4200000, 4890000, 25, 36, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP002', N'CPU Intel Core i7-13700K', 'LSP01', 'NCC04', N'16C/24T, 3.4GHz - 5.4GHz, Socket LGA 1700, 125W', 8500000, 9790000, 15, 36, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP003', N'CPU Intel Core i9-13900K', 'LSP01', 'NCC04', N'24C/32T, 3.0GHz - 5.8GHz, Socket LGA 1700, 125W', 12500000, 13990000, 8, 36, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP004', N'CPU AMD Ryzen 5 7600X', 'LSP01', 'NCC01', N'6C/12T, 4.7GHz - 5.3GHz, Socket AM5, 105W', 5200000, 5990000, 20, 36, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP005', N'CPU AMD Ryzen 7 7700X', 'LSP01', 'NCC01', N'8C/16T, 4.5GHz - 5.4GHz, Socket AM5, 105W', 7200000, 8290000, 12, 36, N'Đang kinh doanh');

    -- Mainboard
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP006', N'Mainboard ASUS ROG STRIX B660-A', 'LSP02', 'NCC01', N'LGA 1700, DDR5, PCIe 5.0, WiFi 6, ATX', 4200000, 4890000, 18, 36, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP007', N'Mainboard MSI MAG B760 TOMAHAWK', 'LSP02', 'NCC02', N'LGA 1700, DDR5, PCIe 4.0, 2.5G LAN, ATX', 3800000, 4490000, 22, 36, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP008', N'Mainboard Gigabyte B650 AORUS ELITE AX', 'LSP02', 'NCC03', N'AM5, DDR5, PCIe 5.0, WiFi 6E, ATX', 4500000, 5190000, 14, 36, N'Đang kinh doanh');

    -- RAM
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP009', N'RAM Kingston Fury Beast 16GB DDR5 5200MHz', 'LSP03', 'NCC01', N'16GB (1x16GB), DDR5, 5200MHz, CL40, 1.25V', 1100000, 1390000, 40, 60, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP010', N'RAM Corsair Vengeance 32GB (2x16GB) DDR5 5600MHz', 'LSP03', 'NCC02', N'32GB Kit (2x16GB), DDR5, 5600MHz, CL36, RGB', 2400000, 2890000, 30, 60, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP011', N'RAM G.Skill Trident Z5 RGB 32GB DDR5 6000MHz', 'LSP03', 'NCC01', N'32GB Kit (2x16GB), DDR5, 6000MHz, CL30, RGB', 3200000, 3790000, 15, 60, N'Đang kinh doanh');

    -- SSD/HDD
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP012', N'SSD Samsung 980 PRO 1TB NVMe M.2', 'LSP04', 'NCC01', N'1TB, NVMe M.2, Read 7000MB/s, Write 5000MB/s, TLC', 2200000, 2690000, 35, 60, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP013', N'SSD WD Black SN850X 2TB NVMe', 'LSP04', 'NCC02', N'2TB, NVMe M.2, Read 7300MB/s, Write 6600MB/s, TLC', 3800000, 4490000, 18, 60, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP014', N'SSD Kingston NV2 500GB NVMe M.2', 'LSP04', 'NCC03', N'500GB, NVMe M.2, Read 3500MB/s, Write 2100MB/s', 700000, 890000, 50, 36, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP015', N'HDD Seagate Barracuda 2TB 7200rpm', 'LSP04', 'NCC02', N'2TB, 7200rpm, SATA III, 256MB Cache', 1100000, 1390000, 25, 24, N'Đang kinh doanh');

    -- VGA
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP016', N'VGA NVIDIA GeForce RTX 4060 8GB', 'LSP05', 'NCC01', N'8GB GDDR6, Boost 2460MHz, 128-bit, 115W, DLSS 3', 7200000, 8290000, 15, 36, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP017', N'VGA NVIDIA GeForce RTX 4070 12GB', 'LSP05', 'NCC02', N'12GB GDDR6X, Boost 2475MHz, 192-bit, 200W, DLSS 3', 12500000, 13990000, 10, 36, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP018', N'VGA AMD Radeon RX 7600 8GB', 'LSP05', 'NCC01', N'8GB GDDR6, Boost 2655MHz, 128-bit, 165W', 6200000, 7190000, 18, 36, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP019', N'VGA NVIDIA GeForce RTX 4090 24GB', 'LSP05', 'NCC03', N'24GB GDDR6X, Boost 2520MHz, 384-bit, 450W', 38000000, 42990000, 5, 36, N'Đang kinh doanh');

    -- PSU
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP020', N'Nguồn Corsair RM750e 750W 80+ Gold', 'LSP06', 'NCC01', N'750W, 80+ Gold, Full Modular, ATX 3.0', 1800000, 2190000, 20, 84, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP021', N'Nguồn Seasonic Focus GX-850 850W 80+ Gold', 'LSP06', 'NCC02', N'850W, 80+ Gold, Full Modular, Hybrid Fan', 2500000, 2990000, 15, 120, N'Đang kinh doanh');

    -- Case
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP022', N'Case NZXT H5 Flow', 'LSP07', 'NCC01', N'Mid Tower, ATX, Kính cường lực, 2 fan 120mm', 1800000, 2290000, 12, 24, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP023', N'Case Lian Li O11 Dynamic EVO', 'LSP07', 'NCC03', N'Mid Tower, E-ATX, Kính cường lực, Dual Chamber', 2800000, 3390000, 8, 24, N'Đang kinh doanh');

    -- Tản nhiệt
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP024', N'Tản nhiệt Noctua NH-D15', 'LSP08', 'NCC02', N'2 quạt 150mm, 6 ống đồng, TDP 250W, Socket đa năng', 2000000, 2490000, 10, 72, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP025', N'Tản nước AIO NZXT Kraken X63 280mm', 'LSP08', 'NCC01', N'280mm Radiator, 2 fan 140mm, LCD Display, RGB', 3500000, 4190000, 8, 60, N'Đang kinh doanh');

    -- Màn hình
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP026', N'Màn hình LG 27GP850-B 27" 2K 165Hz', 'LSP09', 'NCC01', N'27", 2560x1440, IPS, 165Hz, 1ms, HDR400, G-Sync', 7500000, 8690000, 12, 36, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP027', N'Màn hình Samsung Odyssey G5 32" 2K 165Hz', 'LSP09', 'NCC02', N'32", 2560x1440, VA, 165Hz, 1ms, HDR10, FreeSync', 6200000, 7190000, 10, 36, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP028', N'Màn hình Dell S2722DGM 27" 2K 165Hz', 'LSP09', 'NCC03', N'27", 2560x1440, VA Curved, 165Hz, 1ms, FreeSync', 5500000, 6390000, 8, 36, N'Đang kinh doanh');

    -- Bàn phím
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP029', N'Bàn phím cơ Logitech G Pro X', 'LSP10', 'NCC01', N'Cơ GX Blue, RGB, TKL 87 phím, Có dây USB', 1600000, 1990000, 25, 24, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP030', N'Bàn phím cơ Razer BlackWidow V4', 'LSP10', 'NCC02', N'Razer Green Switch, RGB Chroma, Full size, Có dây', 2800000, 3390000, 15, 24, N'Đang kinh doanh');

    -- Chuột
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP031', N'Chuột Logitech G Pro X Superlight 2', 'LSP11', 'NCC01', N'Wireless, 25600 DPI, 60g, 5 nút, Pin 95h', 2400000, 2890000, 20, 24, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP032', N'Chuột Razer DeathAdder V3 Pro', 'LSP11', 'NCC02', N'Wireless, 30000 DPI, 63g, 5 nút, Pin 90h', 2600000, 3090000, 15, 24, N'Đang kinh doanh');

    -- Tai nghe
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP033', N'Tai nghe SteelSeries Arctis Nova Pro', 'LSP12', 'NCC01', N'Over-ear, ANC, Hi-Res Audio, Wireless, 44h', 4800000, 5590000, 10, 24, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP034', N'Tai nghe HyperX Cloud III Wireless', 'LSP12', 'NCC02', N'Over-ear, 53mm Driver, DTS, Wireless 2.4GHz, 120h', 2500000, 2990000, 18, 24, N'Đang kinh doanh');

    -- Laptop
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP035', N'Laptop ASUS TUF Gaming F15 FX507Z', 'LSP13', 'NCC01', N'i7-12700H, RTX 4060, 16GB DDR5, 512GB SSD, 15.6" FHD 144Hz', 20500000, 23990000, 8, 24, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP036', N'Laptop Lenovo Legion 5 Pro', 'LSP13', 'NCC02', N'R7-7745HX, RTX 4070, 16GB DDR5, 1TB SSD, 16" WQXGA 165Hz', 28000000, 31990000, 5, 24, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP037', N'Laptop Dell Inspiron 15 3530', 'LSP13', 'NCC03', N'i5-1335U, Intel Iris Xe, 8GB DDR4, 512GB SSD, 15.6" FHD', 12500000, 14290000, 12, 12, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP038', N'Laptop MacBook Air M2 2022', 'LSP13', 'NCC05', N'Apple M2 8C CPU/8C GPU, 8GB, 256GB SSD, 13.6" Liquid Retina', 23000000, 25990000, 6, 12, N'Đang kinh doanh');

    -- Thêm sản phẩm phụ kiện
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP039', N'CPU Intel Core i3-13100F', 'LSP01', 'NCC04', N'4C/8T, 3.4GHz - 4.5GHz, Socket LGA 1700, 58W', 2200000, 2690000, 30, 36, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP040', N'RAM Kingston Fury Beast 8GB DDR4 3200MHz', 'LSP03', 'NCC01', N'8GB (1x8GB), DDR4, 3200MHz, CL16, 1.35V', 500000, 650000, 60, 60, N'Đang kinh doanh');

    -- ============ BỔ SUNG SẢN PHẨM MỚI (SP041 - SP080) ============

    -- CPU bổ sung
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP041', N'CPU AMD Ryzen 9 7900X', 'LSP01', 'NCC01', N'12C/24T, 4.7GHz - 5.6GHz, Socket AM5, 170W, 64MB L3 Cache', 9800000, 11290000, 7, 36, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP042', N'CPU Intel Core i5-14400F', 'LSP01', 'NCC04', N'10C/16T, 2.5GHz - 4.7GHz, Socket LGA 1700, 65W, 20MB L3', 4500000, 5290000, 22, 36, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP043', N'CPU Intel Core i7-14700K', 'LSP01', 'NCC04', N'20C/28T, 3.4GHz - 5.6GHz, Socket LGA 1700, 125W, 33MB L3', 9200000, 10590000, 10, 36, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP044', N'CPU AMD Ryzen 5 5600', 'LSP01', 'NCC01', N'6C/12T, 3.5GHz - 4.4GHz, Socket AM4, 65W, 32MB L3', 2800000, 3290000, 35, 36, N'Đang kinh doanh');

    -- Mainboard bổ sung
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP045', N'Mainboard ASUS PRIME B760M-A D4', 'LSP02', 'NCC01', N'LGA 1700, DDR4, PCIe 4.0, M.2 x2, Micro-ATX', 2500000, 2990000, 20, 36, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP046', N'Mainboard MSI PRO B650M-P', 'LSP02', 'NCC02', N'AM5, DDR5, PCIe 4.0, M.2 x2, Micro-ATX', 2800000, 3390000, 16, 36, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP047', N'Mainboard Gigabyte B450M DS3H V2', 'LSP02', 'NCC03', N'AM4, DDR4, PCIe 3.0, M.2 x1, Micro-ATX', 1300000, 1590000, 28, 36, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP048', N'Mainboard ASUS ROG STRIX Z790-E Gaming WiFi', 'LSP02', 'NCC01', N'LGA 1700, DDR5, PCIe 5.0, WiFi 6E, ATX, Thunderbolt 4', 8500000, 9890000, 6, 36, N'Đang kinh doanh');

    -- RAM bổ sung
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP049', N'RAM Corsair Vengeance LPX 16GB (2x8GB) DDR4 3200MHz', 'LSP03', 'NCC02', N'16GB Kit (2x8GB), DDR4, 3200MHz, CL16, 1.35V, Heatsink', 780000, 990000, 45, 60, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP050', N'RAM Kingston Fury Renegade 16GB DDR5 6400MHz', 'LSP03', 'NCC01', N'16GB (1x16GB), DDR5, 6400MHz, CL32, 1.4V, RGB', 1500000, 1890000, 20, 60, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP051', N'RAM TeamGroup T-Force Delta RGB 32GB (2x16GB) DDR4 3600MHz', 'LSP03', 'NCC03', N'32GB Kit (2x16GB), DDR4, 3600MHz, CL18, RGB Addressable', 1600000, 1990000, 18, 60, N'Đang kinh doanh');

    -- SSD/HDD bổ sung
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP052', N'SSD Samsung 990 PRO 2TB NVMe M.2', 'LSP04', 'NCC01', N'2TB, NVMe PCIe 4.0, Read 7450MB/s, Write 6900MB/s, V-NAND TLC', 4200000, 4990000, 12, 60, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP053', N'SSD Crucial P3 Plus 1TB NVMe M.2', 'LSP04', 'NCC02', N'1TB, NVMe PCIe 4.0, Read 5000MB/s, Write 4200MB/s, QLC', 1350000, 1690000, 30, 60, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP054', N'SSD Samsung 870 EVO 1TB SATA III', 'LSP04', 'NCC01', N'1TB, SATA III 2.5 inch, Read 560MB/s, Write 530MB/s, V-NAND', 1700000, 2090000, 22, 60, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP055', N'HDD WD Blue 1TB 7200rpm', 'LSP04', 'NCC02', N'1TB, 7200rpm, SATA III, 64MB Cache, 3.5 inch', 750000, 950000, 30, 24, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP056', N'HDD WD Purple 4TB (CMR Giám sát)', 'LSP04', 'NCC02', N'4TB, 5400rpm, SATA III, 256MB Cache, 3.5 inch, 24/7', 2200000, 2790000, 15, 36, N'Đang kinh doanh');

    -- VGA bổ sung
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP057', N'VGA NVIDIA GeForce RTX 4060 Ti 8GB', 'LSP05', 'NCC01', N'8GB GDDR6, Boost 2535MHz, 128-bit, 160W, DLSS 3, Ada Lovelace', 9200000, 10690000, 12, 36, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP058', N'VGA NVIDIA GeForce RTX 4070 Ti SUPER 16GB', 'LSP05', 'NCC02', N'16GB GDDR6X, Boost 2610MHz, 256-bit, 285W, DLSS 3', 18500000, 20990000, 6, 36, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP059', N'VGA AMD Radeon RX 7800 XT 16GB', 'LSP05', 'NCC01', N'16GB GDDR6, Boost 2430MHz, 256-bit, 263W, FSR 3', 10500000, 11990000, 9, 36, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP060', N'VGA NVIDIA GeForce GTX 1650 4GB', 'LSP05', 'NCC03', N'4GB GDDR6, Boost 1590MHz, 128-bit, 75W, PCIe 3.0', 3200000, 3790000, 20, 36, N'Đang kinh doanh');

    -- PSU bổ sung
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP061', N'Nguồn Corsair CV550 550W 80+ Bronze', 'LSP06', 'NCC01', N'550W, 80+ Bronze, Non-Modular, Quạt 120mm', 850000, 1090000, 25, 36, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP062', N'Nguồn EVGA SuperNOVA 1000 G7 1000W 80+ Gold', 'LSP06', 'NCC02', N'1000W, 80+ Gold, Full Modular, ATX 3.0, 135mm Fan', 3200000, 3890000, 8, 120, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP063', N'Nguồn Cooler Master MWE Gold V2 650W', 'LSP06', 'NCC03', N'650W, 80+ Gold, Full Modular, Quạt 120mm HDB', 1400000, 1790000, 18, 60, N'Đang kinh doanh');

    -- Case bổ sung
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP064', N'Case Corsair 4000D Airflow', 'LSP07', 'NCC01', N'Mid Tower, ATX, Kính cường lực, 2 fan 120mm, Mesh Front', 1900000, 2390000, 14, 24, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP065', N'Case Cooler Master MasterBox Q300L', 'LSP07', 'NCC03', N'Mini Tower, Micro-ATX, Tấm lọc bụi, 1 fan 120mm', 750000, 990000, 20, 24, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP066', N'Case Phanteks Eclipse G360A', 'LSP07', 'NCC02', N'Mid Tower, ATX, Kính cường lực, 3 fan D-RGB 120mm, Mesh', 2200000, 2690000, 10, 24, N'Đang kinh doanh');

    -- Tản nhiệt bổ sung
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP067', N'Tản nhiệt ID-COOLING SE-214-XT', 'LSP08', 'NCC03', N'1 quạt 120mm, 4 ống đồng, TDP 180W, Socket Intel/AMD', 350000, 490000, 30, 24, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP068', N'Tản nhiệt DeepCool AK620', 'LSP08', 'NCC02', N'2 quạt 120mm, 6 ống đồng, TDP 260W, Socket LGA1700/AM5', 1100000, 1490000, 15, 36, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP069', N'Tản nước AIO Corsair iCUE H150i Elite 360mm', 'LSP08', 'NCC01', N'360mm Radiator, 3 fan ML120 RGB, Copper Cold Plate', 4200000, 4990000, 6, 60, N'Đang kinh doanh');

    -- Màn hình bổ sung
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP070', N'Màn hình ASUS VG249Q1A 24" FHD 165Hz', 'LSP09', 'NCC01', N'23.8", 1920x1080, IPS, 165Hz, 1ms MPRT, FreeSync, HDMI+DP', 3500000, 4190000, 16, 36, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP071', N'Màn hình LG 27UL500-W 27" 4K IPS', 'LSP09', 'NCC01', N'27", 3840x2160, IPS, 60Hz, 5ms, HDR10, sRGB 98%', 5800000, 6790000, 10, 36, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP072', N'Màn hình Samsung LS24C330 24" FHD IPS', 'LSP09', 'NCC02', N'24", 1920x1080, IPS, 100Hz, 5ms, HDMI+VGA, Viền mỏng', 2200000, 2790000, 22, 36, N'Đang kinh doanh');

    -- Bàn phím bổ sung
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP073', N'Bàn phím cơ Akko 3098B Multi-modes', 'LSP10', 'NCC03', N'Akko CS Jelly Pink, RGB, 98 phím, Bluetooth/2.4G/USB-C', 1200000, 1490000, 20, 12, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP074', N'Bàn phím Logitech K380 Multi-Device', 'LSP10', 'NCC01', N'Membrane, Bluetooth, 3 thiết bị, Pin AAA 2 năm', 650000, 850000, 30, 12, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP075', N'Bàn phím cơ Corsair K70 RGB PRO', 'LSP10', 'NCC02', N'Cherry MX Red, RGB, Full size, USB Passthrough, Có dây', 2500000, 3090000, 12, 24, N'Đang kinh doanh');

    -- Chuột bổ sung
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP076', N'Chuột Logitech G502 X PLUS Wireless', 'LSP11', 'NCC01', N'Wireless LIGHTSPEED, 25600 DPI, 106g, 13 nút, RGB, Pin 130h', 2800000, 3390000, 14, 24, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP077', N'Chuột Logitech MX Master 3S', 'LSP11', 'NCC01', N'Wireless BT/USB, 8000 DPI, 141g, MagSpeed Scroll, USB-C', 1900000, 2390000, 18, 24, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP078', N'Chuột Razer Viper V3 HyperSpeed', 'LSP11', 'NCC02', N'Wireless, 35000 DPI, 82g, 6 nút, Pin 280h, Kết nối kép', 2200000, 2690000, 12, 24, N'Đang kinh doanh');

    -- Tai nghe bổ sung
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP079', N'Tai nghe Razer Kraken V3 HyperSense', 'LSP12', 'NCC02', N'Over-ear, 50mm TriForce Driver, THX Spatial, USB, Haptic', 1800000, 2290000, 14, 24, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP080', N'Tai nghe Logitech G735 Wireless', 'LSP12', 'NCC01', N'Over-ear, 40mm Pro-G Driver, BT/LIGHTSPEED, RGB, 56h, Mic Blue VO!CE', 3200000, 3890000, 10, 24, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP081', N'Tai nghe Sony WH-1000XM5', 'LSP12', 'NCC05', N'Over-ear, 30mm Driver, ANC, LDAC, Bluetooth 5.2, 30h, 250g', 6500000, 7590000, 8, 12, N'Đang kinh doanh');

    -- Laptop bổ sung
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP082', N'Laptop ASUS ROG Strix G16 G614JV', 'LSP13', 'NCC01', N'i7-13650HX, RTX 4060 8GB, 16GB DDR5, 512GB SSD, 16" WQXGA 165Hz', 26500000, 29990000, 6, 24, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP083', N'Laptop HP Victus 15-fa1093TX', 'LSP13', 'NCC02', N'i5-12450H, RTX 4050 6GB, 16GB DDR4, 512GB SSD, 15.6" FHD 144Hz', 17000000, 19490000, 10, 24, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP084', N'Laptop Acer Nitro V ANV15-51-55CA', 'LSP13', 'NCC03', N'i5-13420H, RTX 4050 6GB, 16GB DDR5, 512GB SSD, 15.6" FHD 144Hz', 18500000, 20990000, 7, 24, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP085', N'Laptop MacBook Pro 14 M3 Pro', 'LSP13', 'NCC05', N'Apple M3 Pro 11C CPU/14C GPU, 18GB, 512GB SSD, 14.2" Liquid Retina XDR', 42000000, 47990000, 4, 12, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP086', N'Laptop Lenovo IdeaPad Slim 5 14IAH8', 'LSP13', 'NCC02', N'i5-12500H, Intel Iris Xe, 16GB DDR5, 512GB SSD, 14" 2.8K OLED', 15000000, 17290000, 9, 12, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP087', N'Laptop HP Pavilion 15-eg3098TU', 'LSP13', 'NCC02', N'i5-1340P, Intel Iris Xe, 16GB DDR4, 512GB SSD, 15.6" FHD IPS', 13200000, 15290000, 11, 12, N'Đang kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP088', N'Laptop MSI GF63 Thin 12VE', 'LSP13', 'NCC02', N'i5-12450H, RTX 4050 6GB, 8GB DDR4, 512GB SSD, 15.6" FHD 144Hz', 15500000, 17990000, 8, 24, N'Đang kinh doanh');

    -- Sản phẩm ngừng kinh doanh (cho test filter)
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP089', N'CPU Intel Core i5-12400F (cũ)', 'LSP01', 'NCC04', N'6C/12T, 2.5GHz - 4.4GHz, Socket LGA 1700, 65W', 3100000, 3590000, 0, 36, N'Ngừng kinh doanh');
    INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai)
    VALUES ('SP090', N'VGA NVIDIA GeForce RTX 3060 12GB (cũ)', 'LSP05', 'NCC01', N'12GB GDDR6, Boost 1780MHz, 192-bit, 170W, DLSS 2', 5500000, 6490000, 0, 36, N'Ngừng kinh doanh');
END
GO

-- ============ Khách hàng ============
IF NOT EXISTS (SELECT 1 FROM KhachHang WHERE MaKH = 'KH01')
BEGIN
    INSERT INTO KhachHang (MaKH, TenKH, SDT, Email, DiaChi, DiemTichLuy) VALUES ('KH01', N'Nguyễn Hoàng Nam', '0911223344', 'nam@gmail.com', N'Q.1, TP.HCM', 150);
    INSERT INTO KhachHang (MaKH, TenKH, SDT, Email, DiaChi, DiemTichLuy) VALUES ('KH02', N'Trần Thị Hoa', '0922334455', 'hoa@gmail.com', N'Q.7, TP.HCM', 80);
    INSERT INTO KhachHang (MaKH, TenKH, SDT, Email, DiaChi, DiemTichLuy) VALUES ('KH03', N'Lê Minh Đức', '0933445566', 'duc@gmail.com', N'Bình Thạnh, TP.HCM', 200);
    INSERT INTO KhachHang (MaKH, TenKH, SDT, Email, DiaChi, DiemTichLuy) VALUES ('KH04', N'Phạm Thanh Tùng', '0944556677', 'tung@gmail.com', N'Thủ Đức, TP.HCM', 50);
    INSERT INTO KhachHang (MaKH, TenKH, SDT, Email, DiaChi, DiemTichLuy) VALUES ('KH05', N'Võ Thị Mai', '0955667788', 'mai@gmail.com', N'Q.3, TP.HCM', 120);
END
GO

PRINT N'✅ Đã thêm dữ liệu mẫu thành công!';
PRINT N'📋 Tài khoản đăng nhập:';
PRINT N'   admin / admin123 (Quản lý)';
PRINT N'   nvbh01 / nv123 (NV bán hàng)';  
PRINT N'   nvbh02 / nv123 (NV bán hàng)';
PRINT N'   thukho01 / tk123 (Thủ kho)';
GO
