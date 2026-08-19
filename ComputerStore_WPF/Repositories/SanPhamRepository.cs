using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ComputerStore_WPF.DataAccess;
using ComputerStore_WPF.Models;

namespace ComputerStore_WPF.Repositories
{
    
    public class SanPhamRepository : ISanPhamRepository
    {
        #region Sản phẩm

        public List<SanPhamModel> GetAll()
        {
            var list = new List<SanPhamModel>();
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT sp.*, lsp.TenLoai, ncc.TenNCC 
                               FROM SanPham sp 
                               LEFT JOIN LoaiSanPham lsp ON sp.MaLoai = lsp.MaLoai 
                               LEFT JOIN NhaCungCap ncc ON sp.MaNCC = ncc.MaNCC
                               ORDER BY sp.MaSP";
                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(MapSanPham(reader));
                    }
                }
            }
            return list;
        }

        public SanPhamModel GetById(string maSP)
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT sp.*, lsp.TenLoai, ncc.TenNCC 
                               FROM SanPham sp 
                               LEFT JOIN LoaiSanPham lsp ON sp.MaLoai = lsp.MaLoai 
                               LEFT JOIN NhaCungCap ncc ON sp.MaNCC = ncc.MaNCC
                               WHERE sp.MaSP = @MaSP";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaSP", maSP);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            return MapSanPham(reader);
                    }
                }
            }
            return null;
        }

        public List<SanPhamModel> Search(string keyword, string maLoai = null, string maNCC = null, decimal? giaMin = null, decimal? giaMax = null)
        {
            var list = new List<SanPhamModel>();
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                string sql = @"SELECT sp.*, lsp.TenLoai, ncc.TenNCC 
                               FROM SanPham sp 
                               LEFT JOIN LoaiSanPham lsp ON sp.MaLoai = lsp.MaLoai 
                               LEFT JOIN NhaCungCap ncc ON sp.MaNCC = ncc.MaNCC
                               WHERE 1=1";

                var parameters = new List<SqlParameter>();

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    sql += " AND (sp.TenSP LIKE @Keyword OR sp.MaSP LIKE @Keyword OR sp.ThongSoKyThuat LIKE @Keyword)";
                    parameters.Add(new SqlParameter("@Keyword", "%" + keyword + "%"));
                }

                if (!string.IsNullOrWhiteSpace(maLoai))
                {
                    sql += " AND sp.MaLoai = @MaLoai";
                    parameters.Add(new SqlParameter("@MaLoai", maLoai));
                }

                if (!string.IsNullOrWhiteSpace(maNCC))
                {
                    sql += " AND sp.MaNCC = @MaNCC";
                    parameters.Add(new SqlParameter("@MaNCC", maNCC));
                }

                if (giaMin.HasValue)
                {
                    sql += " AND sp.GiaBan >= @GiaMin";
                    parameters.Add(new SqlParameter("@GiaMin", giaMin.Value));
                }

                if (giaMax.HasValue)
                {
                    sql += " AND sp.GiaBan <= @GiaMax";
                    parameters.Add(new SqlParameter("@GiaMax", giaMax.Value));
                }

                sql += " ORDER BY sp.MaSP";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(MapSanPham(reader));
                        }
                    }
                }
            }
            return list;
        }

        public bool Insert(SanPhamModel sp)
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                string sql = @"INSERT INTO SanPham (MaSP, TenSP, MaLoai, MaNCC, HinhAnh, ThongSoKyThuat, GiaNhap, GiaBan, SoLuongTon, BaoHanh, TrangThai) 
                               VALUES (@MaSP, @TenSP, @MaLoai, @MaNCC, @HinhAnh, @ThongSoKyThuat, @GiaNhap, @GiaBan, @SoLuongTon, @BaoHanh, @TrangThai)";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaSP", sp.MaSP);
                    cmd.Parameters.AddWithValue("@TenSP", sp.TenSP);
                    cmd.Parameters.AddWithValue("@MaLoai", sp.MaLoai);
                    cmd.Parameters.AddWithValue("@MaNCC", sp.MaNCC);
                    cmd.Parameters.AddWithValue("@HinhAnh", (object)sp.HinhAnh ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ThongSoKyThuat", (object)sp.ThongSoKyThuat ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@GiaNhap", sp.GiaNhap);
                    cmd.Parameters.AddWithValue("@GiaBan", sp.GiaBan);
                    cmd.Parameters.AddWithValue("@SoLuongTon", sp.SoLuongTon);
                    cmd.Parameters.AddWithValue("@BaoHanh", sp.BaoHanh);
                    cmd.Parameters.AddWithValue("@TrangThai", sp.TrangThai ?? "Đang kinh doanh");
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Update(SanPhamModel sp)
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                string sql = @"UPDATE SanPham SET TenSP=@TenSP, MaLoai=@MaLoai, MaNCC=@MaNCC, HinhAnh=@HinhAnh, 
                               ThongSoKyThuat=@ThongSoKyThuat, GiaNhap=@GiaNhap, GiaBan=@GiaBan, 
                               SoLuongTon=@SoLuongTon, BaoHanh=@BaoHanh, TrangThai=@TrangThai 
                               WHERE MaSP=@MaSP";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaSP", sp.MaSP);
                    cmd.Parameters.AddWithValue("@TenSP", sp.TenSP);
                    cmd.Parameters.AddWithValue("@MaLoai", sp.MaLoai);
                    cmd.Parameters.AddWithValue("@MaNCC", sp.MaNCC);
                    cmd.Parameters.AddWithValue("@HinhAnh", (object)sp.HinhAnh ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ThongSoKyThuat", (object)sp.ThongSoKyThuat ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@GiaNhap", sp.GiaNhap);
                    cmd.Parameters.AddWithValue("@GiaBan", sp.GiaBan);
                    cmd.Parameters.AddWithValue("@SoLuongTon", sp.SoLuongTon);
                    cmd.Parameters.AddWithValue("@BaoHanh", sp.BaoHanh);
                    cmd.Parameters.AddWithValue("@TrangThai", sp.TrangThai);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Delete(string maSP)
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                // Soft delete - chuyển trạng thái thay vì xóa
                string sql = "UPDATE SanPham SET TrangThai = N'Ngừng kinh doanh' WHERE MaSP = @MaSP";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaSP", maSP);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public string GenerateMaSP()
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT ISNULL(MAX(CAST(SUBSTRING(MaSP,3,LEN(MaSP)-2) AS INT)),0)+1 FROM SanPham", conn))
                    return "SP" + Convert.ToInt32(cmd.ExecuteScalar()).ToString("D3");
            }
        }

        private SanPhamModel MapSanPham(SqlDataReader reader)
        {
            return new SanPhamModel
            {
                MaSP = reader["MaSP"].ToString(),
                TenSP = reader["TenSP"].ToString(),
                MaLoai = reader["MaLoai"].ToString(),
                MaNCC = reader["MaNCC"].ToString(),
                HinhAnh = reader["HinhAnh"]?.ToString(),
                ThongSoKyThuat = reader["ThongSoKyThuat"]?.ToString(),
                GiaNhap = Convert.ToDecimal(reader["GiaNhap"]),
                GiaBan = Convert.ToDecimal(reader["GiaBan"]),
                SoLuongTon = Convert.ToInt32(reader["SoLuongTon"]),
                BaoHanh = Convert.ToInt32(reader["BaoHanh"]),
                TrangThai = reader["TrangThai"].ToString(),
                TenLoai = reader["TenLoai"]?.ToString(),
                TenNCC = reader["TenNCC"]?.ToString()
            };
        }

        #endregion

        #region Loại sản phẩm

        public List<LoaiSanPhamModel> GetAllLoaiSanPham()
        {
            var list = new List<LoaiSanPhamModel>();
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                string sql = "SELECT * FROM LoaiSanPham ORDER BY MaLoai";
                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new LoaiSanPhamModel
                        {
                            MaLoai = reader["MaLoai"].ToString(),
                            TenLoai = reader["TenLoai"].ToString(),
                            MoTa = reader["MoTa"]?.ToString()
                        });
                    }
                }
            }
            return list;
        }

        public bool InsertLoaiSanPham(LoaiSanPhamModel loai)
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                string sql = "INSERT INTO LoaiSanPham (MaLoai, TenLoai, MoTa) VALUES (@MaLoai, @TenLoai, @MoTa)";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaLoai", loai.MaLoai);
                    cmd.Parameters.AddWithValue("@TenLoai", loai.TenLoai);
                    cmd.Parameters.AddWithValue("@MoTa", (object)loai.MoTa ?? DBNull.Value);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateLoaiSanPham(LoaiSanPhamModel loai)
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                string sql = "UPDATE LoaiSanPham SET TenLoai=@TenLoai, MoTa=@MoTa WHERE MaLoai=@MaLoai";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaLoai", loai.MaLoai);
                    cmd.Parameters.AddWithValue("@TenLoai", loai.TenLoai);
                    cmd.Parameters.AddWithValue("@MoTa", (object)loai.MoTa ?? DBNull.Value);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteLoaiSanPham(string maLoai)
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                // Kiểm tra có sản phẩm thuộc loại này không
                string checkSql = "SELECT COUNT(*) FROM SanPham WHERE MaLoai = @MaLoai";
                using (var checkCmd = new SqlCommand(checkSql, conn))
                {
                    checkCmd.Parameters.AddWithValue("@MaLoai", maLoai);
                    int count = (int)checkCmd.ExecuteScalar();
                    if (count > 0) return false; // Không xóa nếu còn sản phẩm
                }

                string sql = "DELETE FROM LoaiSanPham WHERE MaLoai = @MaLoai";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaLoai", maLoai);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region Nhà cung cấp

        public List<NhaCungCapModel> GetAllNhaCungCap()
        {
            var list = new List<NhaCungCapModel>();
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                string sql = "SELECT * FROM NhaCungCap ORDER BY MaNCC";
                using (var cmd = new SqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new NhaCungCapModel
                        {
                            MaNCC = reader["MaNCC"].ToString(),
                            TenNCC = reader["TenNCC"].ToString(),
                            SDT = reader["SDT"]?.ToString(),
                            DiaChi = reader["DiaChi"]?.ToString(),
                            Email = reader["Email"]?.ToString(),
                            TrangThai = reader["TrangThai"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public bool InsertNhaCungCap(NhaCungCapModel ncc)
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                string sql = "INSERT INTO NhaCungCap (MaNCC, TenNCC, SDT, DiaChi, Email, TrangThai) VALUES (@MaNCC, @TenNCC, @SDT, @DiaChi, @Email, @TrangThai)";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaNCC", ncc.MaNCC);
                    cmd.Parameters.AddWithValue("@TenNCC", ncc.TenNCC);
                    cmd.Parameters.AddWithValue("@SDT", (object)ncc.SDT ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DiaChi", (object)ncc.DiaChi ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object)ncc.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TrangThai", ncc.TrangThai ?? "Hoạt động");
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateNhaCungCap(NhaCungCapModel ncc)
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                string sql = "UPDATE NhaCungCap SET TenNCC=@TenNCC, SDT=@SDT, DiaChi=@DiaChi, Email=@Email, TrangThai=@TrangThai WHERE MaNCC=@MaNCC";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaNCC", ncc.MaNCC);
                    cmd.Parameters.AddWithValue("@TenNCC", ncc.TenNCC);
                    cmd.Parameters.AddWithValue("@SDT", (object)ncc.SDT ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DiaChi", (object)ncc.DiaChi ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object)ncc.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TrangThai", ncc.TrangThai);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool DeleteNhaCungCap(string maNCC)
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                string checkSql = "SELECT COUNT(*) FROM SanPham WHERE MaNCC = @MaNCC";
                using (var checkCmd = new SqlCommand(checkSql, conn))
                {
                    checkCmd.Parameters.AddWithValue("@MaNCC", maNCC);
                    int count = (int)checkCmd.ExecuteScalar();
                    if (count > 0) return false;
                }

                string sql = "UPDATE NhaCungCap SET TrangThai = N'Ngừng hợp tác' WHERE MaNCC = @MaNCC";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaNCC", maNCC);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion
    }
}
