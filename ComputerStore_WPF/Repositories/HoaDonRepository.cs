using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ComputerStore_WPF.DataAccess;
using ComputerStore_WPF.Models;

namespace ComputerStore_WPF.Repositories
{
    public class HoaDonRepository
    {
        // ===== HÓA ĐƠN BÁN =====
        public List<HoaDonBanModel> GetAllHoaDonBan()
        {
            var list = new List<HoaDonBanModel>();
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT hdb.*, nv.HoTen AS TenNhanVien, kh.TenKH AS TenKhachHang
                               FROM HoaDonBan hdb
                               LEFT JOIN NhanVien nv ON hdb.MaNV = nv.MaNV
                               LEFT JOIN KhachHang kh ON hdb.MaKH = kh.MaKH
                               ORDER BY hdb.NgayBan DESC";
                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) list.Add(MapHDB(reader));
                }
            }
            return list;
        }

        public List<ChiTietHDBModel> GetChiTietHDB(string maHDB)
        {
            var list = new List<ChiTietHDBModel>();
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT ct.*, sp.TenSP FROM ChiTietHDB ct LEFT JOIN SanPham sp ON ct.MaSP = sp.MaSP WHERE ct.MaHDB = @MaHDB";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaHDB", maHDB);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ChiTietHDBModel
                            {
                                MaHDB = reader["MaHDB"].ToString(),
                                MaSP = reader["MaSP"].ToString(),
                                SoLuong = Convert.ToInt32(reader["SoLuong"]),
                                DonGiaBan = Convert.ToDecimal(reader["DonGiaBan"]),
                                ThanhTien = Convert.ToDecimal(reader["ThanhTien"]),
                                ThoiHanBaoHanh = reader["ThoiHanBaoHanh"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["ThoiHanBaoHanh"]),
                                TenSP = reader["TenSP"]?.ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        public bool CreateHoaDonBan(HoaDonBanModel hdb, List<ChiTietHDBModel> chiTietList)
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        string sqlHDB = @"INSERT INTO HoaDonBan (MaHDB,MaNV,MaKH,NgayBan,TongTien,GiamGia,ThanhTienThucTe) VALUES (@MaHDB,@MaNV,@MaKH,@NgayBan,@TongTien,@GiamGia,@ThanhTienThucTe)";
                        using (var cmd = new SqlCommand(sqlHDB, conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@MaHDB", hdb.MaHDB);
                            cmd.Parameters.AddWithValue("@MaNV", hdb.MaNV);
                            cmd.Parameters.AddWithValue("@MaKH", (object)hdb.MaKH ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@NgayBan", hdb.NgayBan);
                            cmd.Parameters.AddWithValue("@TongTien", hdb.TongTien);
                            cmd.Parameters.AddWithValue("@GiamGia", hdb.GiamGia);
                            cmd.Parameters.AddWithValue("@ThanhTienThucTe", hdb.ThanhTienThucTe);
                            cmd.ExecuteNonQuery();
                        }

                        foreach (var ct in chiTietList)
                        {
                            using (var cmdCT = new SqlCommand(@"INSERT INTO ChiTietHDB (MaHDB,MaSP,SoLuong,DonGiaBan,ThanhTien,ThoiHanBaoHanh) VALUES (@MaHDB,@MaSP,@SoLuong,@DonGiaBan,@ThanhTien,@ThoiHanBaoHanh)", conn, tran))
                            {
                                cmdCT.Parameters.AddWithValue("@MaHDB", ct.MaHDB);
                                cmdCT.Parameters.AddWithValue("@MaSP", ct.MaSP);
                                cmdCT.Parameters.AddWithValue("@SoLuong", ct.SoLuong);
                                cmdCT.Parameters.AddWithValue("@DonGiaBan", ct.DonGiaBan);
                                cmdCT.Parameters.AddWithValue("@ThanhTien", ct.ThanhTien);
                                cmdCT.Parameters.AddWithValue("@ThoiHanBaoHanh", (object)ct.ThoiHanBaoHanh ?? DBNull.Value);
                                cmdCT.ExecuteNonQuery();
                            }
                            using (var cmdTon = new SqlCommand("UPDATE SanPham SET SoLuongTon=SoLuongTon-@SL WHERE MaSP=@MaSP AND SoLuongTon>=@SL", conn, tran))
                            {
                                cmdTon.Parameters.AddWithValue("@SL", ct.SoLuong);
                                cmdTon.Parameters.AddWithValue("@MaSP", ct.MaSP);
                                if (cmdTon.ExecuteNonQuery() == 0) throw new Exception($"SP {ct.MaSP} không đủ tồn kho!");
                            }
                        }

                        if (!string.IsNullOrEmpty(hdb.MaKH))
                        {
                            int diem = (int)(hdb.ThanhTienThucTe / 100000);
                            using (var cmdD = new SqlCommand("UPDATE KhachHang SET DiemTichLuy=DiemTichLuy+@D WHERE MaKH=@MaKH", conn, tran))
                            {
                                cmdD.Parameters.AddWithValue("@D", diem);
                                cmdD.Parameters.AddWithValue("@MaKH", hdb.MaKH);
                                cmdD.ExecuteNonQuery();
                            }
                        }
                        tran.Commit();
                        return true;
                    }
                    catch { tran.Rollback(); throw; }
                }
            }
        }

        public string GenerateMaHDB()
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT ISNULL(MAX(CAST(SUBSTRING(MaHDB,4,LEN(MaHDB)-3) AS INT)),0)+1 FROM HoaDonBan", conn))
                    return "HDB" + Convert.ToInt32(cmd.ExecuteScalar()).ToString("D3");
            }
        }

        // ===== HÓA ĐƠN NHẬP =====
        public List<HoaDonNhapModel> GetAllHoaDonNhap()
        {
            var list = new List<HoaDonNhapModel>();
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT hdn.*, nv.HoTen AS TenNhanVien, ncc.TenNCC FROM HoaDonNhap hdn LEFT JOIN NhanVien nv ON hdn.MaNV=nv.MaNV LEFT JOIN NhaCungCap ncc ON hdn.MaNCC=ncc.MaNCC ORDER BY hdn.NgayNhap DESC";
                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new HoaDonNhapModel
                        {
                            MaHDN = reader["MaHDN"].ToString(),
                            MaNV = reader["MaNV"].ToString(),
                            MaNCC = reader["MaNCC"].ToString(),
                            NgayNhap = Convert.ToDateTime(reader["NgayNhap"]),
                            TongTien = Convert.ToDecimal(reader["TongTien"]),
                            GhiChu = reader["GhiChu"]?.ToString(),
                            TenNhanVien = reader["TenNhanVien"]?.ToString(),
                            TenNCC = reader["TenNCC"]?.ToString()
                        });
                    }
                }
            }
            return list;
        }

        public List<ChiTietHDNModel> GetChiTietHDN(string maHDN)
        {
            var list = new List<ChiTietHDNModel>();
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"SELECT ct.*, sp.TenSP FROM ChiTietHDN ct LEFT JOIN SanPham sp ON ct.MaSP=sp.MaSP WHERE ct.MaHDN=@MaHDN", conn))
                {
                    cmd.Parameters.AddWithValue("@MaHDN", maHDN);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ChiTietHDNModel
                            {
                                MaHDN = reader["MaHDN"].ToString(),
                                MaSP = reader["MaSP"].ToString(),
                                SoLuong = Convert.ToInt32(reader["SoLuong"]),
                                DonGiaNhap = Convert.ToDecimal(reader["DonGiaNhap"]),
                                ThanhTien = Convert.ToDecimal(reader["ThanhTien"]),
                                TenSP = reader["TenSP"]?.ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        public bool CreateHoaDonNhap(HoaDonNhapModel hdn, List<ChiTietHDNModel> chiTietList)
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new SqlCommand(@"INSERT INTO HoaDonNhap (MaHDN,MaNV,MaNCC,NgayNhap,TongTien,GhiChu) VALUES (@MaHDN,@MaNV,@MaNCC,@NgayNhap,@TongTien,@GhiChu)", conn, tran))
                        {
                            cmd.Parameters.AddWithValue("@MaHDN", hdn.MaHDN);
                            cmd.Parameters.AddWithValue("@MaNV", hdn.MaNV);
                            cmd.Parameters.AddWithValue("@MaNCC", hdn.MaNCC);
                            cmd.Parameters.AddWithValue("@NgayNhap", hdn.NgayNhap);
                            cmd.Parameters.AddWithValue("@TongTien", hdn.TongTien);
                            cmd.Parameters.AddWithValue("@GhiChu", (object)hdn.GhiChu ?? DBNull.Value);
                            cmd.ExecuteNonQuery();
                        }
                        foreach (var ct in chiTietList)
                        {
                            using (var cmdCT = new SqlCommand(@"INSERT INTO ChiTietHDN (MaHDN,MaSP,SoLuong,DonGiaNhap,ThanhTien) VALUES (@MaHDN,@MaSP,@SoLuong,@DonGiaNhap,@ThanhTien)", conn, tran))
                            {
                                cmdCT.Parameters.AddWithValue("@MaHDN", ct.MaHDN);
                                cmdCT.Parameters.AddWithValue("@MaSP", ct.MaSP);
                                cmdCT.Parameters.AddWithValue("@SoLuong", ct.SoLuong);
                                cmdCT.Parameters.AddWithValue("@DonGiaNhap", ct.DonGiaNhap);
                                cmdCT.Parameters.AddWithValue("@ThanhTien", ct.ThanhTien);
                                cmdCT.ExecuteNonQuery();
                            }
                            using (var cmdTon = new SqlCommand("UPDATE SanPham SET SoLuongTon=SoLuongTon+@SL WHERE MaSP=@MaSP", conn, tran))
                            {
                                cmdTon.Parameters.AddWithValue("@SL", ct.SoLuong);
                                cmdTon.Parameters.AddWithValue("@MaSP", ct.MaSP);
                                cmdTon.ExecuteNonQuery();
                            }
                        }
                        tran.Commit(); return true;
                    }
                    catch { tran.Rollback(); throw; }
                }
            }
        }

        public string GenerateMaHDN()
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT ISNULL(MAX(CAST(SUBSTRING(MaHDN,4,LEN(MaHDN)-3) AS INT)),0)+1 FROM HoaDonNhap", conn))
                    return "HDN" + Convert.ToInt32(cmd.ExecuteScalar()).ToString("D3");
            }
        }

        // ===== KHÁCH HÀNG =====
        public List<KhachHangModel> GetAllKhachHang()
        {
            var list = new List<KhachHangModel>();
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT * FROM KhachHang ORDER BY MaKH", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new KhachHangModel
                        {
                            MaKH = reader["MaKH"].ToString(),
                            TenKH = reader["TenKH"].ToString(),
                            SDT = reader["SDT"]?.ToString(),
                            Email = reader["Email"]?.ToString(),
                            DiaChi = reader["DiaChi"]?.ToString(),
                            DiemTichLuy = Convert.ToInt32(reader["DiemTichLuy"])
                        });
                    }
                }
            }
            return list;
        }

        public bool InsertKhachHang(KhachHangModel kh)
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("INSERT INTO KhachHang (MaKH,TenKH,SDT,Email,DiaChi,DiemTichLuy) VALUES (@MaKH,@TenKH,@SDT,@Email,@DiaChi,@Diem)", conn))
                {
                    cmd.Parameters.AddWithValue("@MaKH", kh.MaKH); cmd.Parameters.AddWithValue("@TenKH", kh.TenKH);
                    cmd.Parameters.AddWithValue("@SDT", (object)kh.SDT ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object)kh.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DiaChi", (object)kh.DiaChi ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Diem", kh.DiemTichLuy);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateKhachHang(KhachHangModel kh)
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("UPDATE KhachHang SET TenKH=@TenKH,SDT=@SDT,Email=@Email,DiaChi=@DiaChi,DiemTichLuy=@Diem WHERE MaKH=@MaKH", conn))
                {
                    cmd.Parameters.AddWithValue("@MaKH", kh.MaKH); cmd.Parameters.AddWithValue("@TenKH", kh.TenKH);
                    cmd.Parameters.AddWithValue("@SDT", (object)kh.SDT ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object)kh.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DiaChi", (object)kh.DiaChi ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Diem", kh.DiemTichLuy);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public string GenerateMaKH()
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT ISNULL(MAX(CAST(SUBSTRING(MaKH,3,LEN(MaKH)-2) AS INT)),0)+1 FROM KhachHang", conn))
                    return "KH" + Convert.ToInt32(cmd.ExecuteScalar()).ToString("D2");
            }
        }

        // ===== THỐNG KÊ =====
        public decimal GetDoanhThu(DateTime tuNgay, DateTime denNgay)
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT ISNULL(SUM(ThanhTienThucTe),0) FROM HoaDonBan WHERE NgayBan>=@T AND NgayBan<=@D", conn))
                {
                    cmd.Parameters.AddWithValue("@T", tuNgay); cmd.Parameters.AddWithValue("@D", denNgay);
                    return Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }
        }

        public List<HoaDonBanModel> GetHoaDonBanByDateRange(DateTime tuNgay, DateTime denNgay)
        {
            var list = new List<HoaDonBanModel>();
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"SELECT hdb.*, nv.HoTen AS TenNhanVien, kh.TenKH AS TenKhachHang FROM HoaDonBan hdb LEFT JOIN NhanVien nv ON hdb.MaNV=nv.MaNV LEFT JOIN KhachHang kh ON hdb.MaKH=kh.MaKH WHERE hdb.NgayBan>=@T AND hdb.NgayBan<=@D ORDER BY hdb.NgayBan DESC", conn))
                {
                    cmd.Parameters.AddWithValue("@T", tuNgay); cmd.Parameters.AddWithValue("@D", denNgay);
                    using (var reader = cmd.ExecuteReader()) { while (reader.Read()) list.Add(MapHDB(reader)); }
                }
            }
            return list;
        }

        public List<Tuple<int, decimal>> GetDoanhThuTheoThang(int nam)
        {
            var list = new List<Tuple<int, decimal>>();
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"SELECT MONTH(NgayBan) AS Thang, SUM(ThanhTienThucTe) AS DT FROM HoaDonBan WHERE YEAR(NgayBan)=@Nam GROUP BY MONTH(NgayBan) ORDER BY Thang", conn))
                {
                    cmd.Parameters.AddWithValue("@Nam", nam);
                    using (var reader = cmd.ExecuteReader())
                    { while (reader.Read()) list.Add(Tuple.Create(Convert.ToInt32(reader["Thang"]), Convert.ToDecimal(reader["DT"]))); }
                }
            }
            return list;
        }

        public int CountHoaDonBan(DateTime tuNgay, DateTime denNgay)
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM HoaDonBan WHERE NgayBan>=@T AND NgayBan<=@D", conn))
                {
                    cmd.Parameters.AddWithValue("@T", tuNgay); cmd.Parameters.AddWithValue("@D", denNgay);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        // ===== ĐĂNG NHẬP & NHÂN VIÊN =====
        public NhanVienModel Login(string tenDangNhap, string matKhauHash)
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"SELECT nv.*, vt.TenVaiTro FROM NhanVien nv INNER JOIN VaiTro vt ON nv.MaVaiTro=vt.MaVaiTro WHERE nv.TenDangNhap=@U AND nv.MatKhau=@P AND nv.TrangThai=N'Hoạt động'", conn))
                {
                    cmd.Parameters.AddWithValue("@U", tenDangNhap); cmd.Parameters.AddWithValue("@P", matKhauHash);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) return MapNhanVien(reader);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Đăng nhập bằng mật khẩu plaintext (fallback khi DB chưa hash)
        /// </summary>
        public NhanVienModel LoginPlainText(string tenDangNhap, string matKhauPlain)
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"SELECT nv.*, vt.TenVaiTro FROM NhanVien nv INNER JOIN VaiTro vt ON nv.MaVaiTro=vt.MaVaiTro WHERE nv.TenDangNhap=@U AND nv.MatKhau=@P AND nv.TrangThai=N'Hoạt động'", conn))
                {
                    cmd.Parameters.AddWithValue("@U", tenDangNhap);
                    cmd.Parameters.AddWithValue("@P", matKhauPlain);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) return MapNhanVien(reader);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Kiểm tra tên đăng nhập đã tồn tại trong hệ thống chưa
        /// </summary>
        public bool IsTenDangNhapExists(string tenDangNhap)
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM NhanVien WHERE TenDangNhap = @U", conn))
                {
                    cmd.Parameters.AddWithValue("@U", tenDangNhap);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        /// <summary>
        /// Kiểm tra email nhân viên đã tồn tại trong hệ thống chưa
        /// </summary>
        public bool IsEmailNhanVienExists(string email)
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM NhanVien WHERE Email = @Email", conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        /// <summary>
        /// Tìm nhân viên theo tên đăng nhập và email (dùng cho quên mật khẩu)
        /// </summary>
        public NhanVienModel GetNhanVienByTenDangNhapAndEmail(string tenDangNhap, string email)
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(
                    @"SELECT nv.*, vt.TenVaiTro FROM NhanVien nv
                      INNER JOIN VaiTro vt ON nv.MaVaiTro = vt.MaVaiTro
                      WHERE nv.TenDangNhap = @U AND nv.Email = @Email", conn))
                {
                    cmd.Parameters.AddWithValue("@U", tenDangNhap);
                    cmd.Parameters.AddWithValue("@Email", email);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) return MapNhanVien(reader);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Cập nhật mật khẩu (chuyển từ plaintext sang hash)
        /// </summary>
        public void UpdatePassword(string maNV, string newPasswordHash)
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("UPDATE NhanVien SET MatKhau=@P WHERE MaNV=@MaNV", conn))
                {
                    cmd.Parameters.AddWithValue("@P", newPasswordHash);
                    cmd.Parameters.AddWithValue("@MaNV", maNV);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private NhanVienModel MapNhanVien(SqlDataReader reader)
        {
            return new NhanVienModel
            {
                MaNV = reader["MaNV"].ToString(),
                HoTen = reader["HoTen"].ToString(),
                NgaySinh = reader["NgaySinh"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["NgaySinh"]),
                SDT = reader["SDT"]?.ToString(),
                Email = reader["Email"]?.ToString(),
                TenDangNhap = reader["TenDangNhap"].ToString(),
                MaVaiTro = reader["MaVaiTro"].ToString(),
                TrangThai = reader["TrangThai"].ToString(),
                TenVaiTro = reader["TenVaiTro"].ToString()
            };
        }

        public List<NhanVienModel> GetAllNhanVien()
        {
            var list = new List<NhanVienModel>();
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"SELECT nv.*, vt.TenVaiTro FROM NhanVien nv INNER JOIN VaiTro vt ON nv.MaVaiTro=vt.MaVaiTro ORDER BY nv.MaNV", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) list.Add(new NhanVienModel
                    {
                        MaNV = reader["MaNV"].ToString(),
                        HoTen = reader["HoTen"].ToString(),
                        NgaySinh = reader["NgaySinh"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["NgaySinh"]),
                        SDT = reader["SDT"]?.ToString(),
                        Email = reader["Email"]?.ToString(),
                        TenDangNhap = reader["TenDangNhap"].ToString(),
                        MaVaiTro = reader["MaVaiTro"].ToString(),
                        TrangThai = reader["TrangThai"].ToString(),
                        TenVaiTro = reader["TenVaiTro"].ToString()
                    });
                }
            }
            return list;
        }

        public List<VaiTroModel> GetAllVaiTro()
        {
            var list = new List<VaiTroModel>();
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT * FROM VaiTro ORDER BY MaVaiTro", conn))
                using (var reader = cmd.ExecuteReader())
                { while (reader.Read()) list.Add(new VaiTroModel { MaVaiTro = reader["MaVaiTro"].ToString(), TenVaiTro = reader["TenVaiTro"].ToString(), MoTa = reader["MoTa"]?.ToString() }); }
            }
            return list;
        }

        // ===== QUẢN LÝ NHÂN VIÊN (ADMIN) =====

        public bool InsertNhanVien(NhanVienModel nv)
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                string sql = @"INSERT INTO NhanVien (MaNV, HoTen, NgaySinh, SDT, Email, TenDangNhap, MatKhau, MaVaiTro, TrangThai)
                               VALUES (@MaNV, @HoTen, @NgaySinh, @SDT, @Email, @TenDangNhap, @MatKhau, @MaVaiTro, @TrangThai)";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaNV", nv.MaNV);
                    cmd.Parameters.AddWithValue("@HoTen", nv.HoTen);
                    cmd.Parameters.AddWithValue("@NgaySinh", (object)nv.NgaySinh ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SDT", (object)nv.SDT ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object)nv.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TenDangNhap", nv.TenDangNhap);
                    cmd.Parameters.AddWithValue("@MatKhau", nv.MatKhau);
                    cmd.Parameters.AddWithValue("@MaVaiTro", nv.MaVaiTro);
                    cmd.Parameters.AddWithValue("@TrangThai", nv.TrangThai ?? "Hoạt động");
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateNhanVien(NhanVienModel nv)
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                string sql = @"UPDATE NhanVien SET HoTen=@HoTen, NgaySinh=@NgaySinh, SDT=@SDT, Email=@Email,
                               TenDangNhap=@TenDangNhap, MaVaiTro=@MaVaiTro, TrangThai=@TrangThai
                               WHERE MaNV=@MaNV";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaNV", nv.MaNV);
                    cmd.Parameters.AddWithValue("@HoTen", nv.HoTen);
                    cmd.Parameters.AddWithValue("@NgaySinh", (object)nv.NgaySinh ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SDT", (object)nv.SDT ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object)nv.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TenDangNhap", nv.TenDangNhap);
                    cmd.Parameters.AddWithValue("@MaVaiTro", nv.MaVaiTro);
                    cmd.Parameters.AddWithValue("@TrangThai", nv.TrangThai);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        /// <summary>
        /// Khóa tài khoản nhân viên (soft delete)
        /// </summary>
        public bool DeleteNhanVien(string maNV)
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                string sql = "UPDATE NhanVien SET TrangThai = N'Đã khóa' WHERE MaNV = @MaNV";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaNV", maNV);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public string GenerateMaNV()
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT ISNULL(MAX(CAST(SUBSTRING(MaNV,3,LEN(MaNV)-2) AS INT)),0)+1 FROM NhanVien", conn))
                    return "NV" + Convert.ToInt32(cmd.ExecuteScalar()).ToString("D2");
            }
        }

        /// <summary>
        /// Lấy doanh thu theo từng loại sản phẩm trong khoảng thời gian
        /// </summary>
        public List<Tuple<string, decimal>> GetDoanhThuTheoLoaiSP(DateTime tuNgay, DateTime denNgay)
        {
            var list = new List<Tuple<string, decimal>>();
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT ISNULL(lsp.TenLoai, N'Không phân loại') AS TenLoai, SUM(ct.ThanhTien) AS DoanhThu
                               FROM ChiTietHDB ct
                               INNER JOIN HoaDonBan hdb ON ct.MaHDB = hdb.MaHDB
                               INNER JOIN SanPham sp ON ct.MaSP = sp.MaSP
                               LEFT JOIN LoaiSanPham lsp ON sp.MaLoai = lsp.MaLoai
                               WHERE hdb.NgayBan >= @T AND hdb.NgayBan <= @D
                               GROUP BY lsp.TenLoai
                               ORDER BY DoanhThu DESC";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@T", tuNgay);
                    cmd.Parameters.AddWithValue("@D", denNgay);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            list.Add(Tuple.Create(reader["TenLoai"].ToString(), Convert.ToDecimal(reader["DoanhThu"])));
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Lấy Top sản phẩm bán chạy nhất (theo số lượng bán)
        /// </summary>
        public List<Tuple<string, string, int, decimal>> GetTopSanPhamBanChay(int top = 5)
        {
            var list = new List<Tuple<string, string, int, decimal>>();
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                string sql = $@"SELECT TOP {top} sp.MaSP, sp.TenSP, SUM(ct.SoLuong) AS TongBan, SUM(ct.ThanhTien) AS TongTien
                                FROM ChiTietHDB ct
                                INNER JOIN SanPham sp ON ct.MaSP = sp.MaSP
                                GROUP BY sp.MaSP, sp.TenSP
                                ORDER BY TongBan DESC";
                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(Tuple.Create(
                            reader["MaSP"].ToString(),
                            reader["TenSP"].ToString(),
                            Convert.ToInt32(reader["TongBan"]),
                            Convert.ToDecimal(reader["TongTien"])));
                }
            }
            return list;
        }

        /// <summary>
        /// Lấy số lượng sản phẩm và tồn kho theo từng loại sản phẩm
        /// </summary>
        public List<Tuple<string, int, int>> GetSoLuongTheoLoai()
        {
            var list = new List<Tuple<string, int, int>>();
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT lsp.TenLoai, COUNT(sp.MaSP) AS SoSP, ISNULL(SUM(sp.SoLuongTon),0) AS TongTon
                               FROM LoaiSanPham lsp
                               LEFT JOIN SanPham sp ON lsp.MaLoai = sp.MaLoai AND sp.TrangThai = N'Đang kinh doanh'
                               GROUP BY lsp.TenLoai
                               ORDER BY TongTon DESC";
                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(Tuple.Create(
                            reader["TenLoai"].ToString(),
                            Convert.ToInt32(reader["SoSP"]),
                            Convert.ToInt32(reader["TongTon"])));
                }
            }
            return list;
        }

        private HoaDonBanModel MapHDB(SqlDataReader r)
        {
            return new HoaDonBanModel
            {
                MaHDB = r["MaHDB"].ToString(),
                MaNV = r["MaNV"].ToString(),
                MaKH = r["MaKH"] == DBNull.Value ? null : r["MaKH"].ToString(),
                NgayBan = Convert.ToDateTime(r["NgayBan"]),
                TongTien = Convert.ToDecimal(r["TongTien"]),
                GiamGia = Convert.ToDecimal(r["GiamGia"]),
                ThanhTienThucTe = Convert.ToDecimal(r["ThanhTienThucTe"]),
                TenNhanVien = r["TenNhanVien"]?.ToString(),
                TenKhachHang = r["TenKhachHang"] == DBNull.Value ? "Khách vãng lai" : r["TenKhachHang"].ToString()
            };
        }
    }
}
