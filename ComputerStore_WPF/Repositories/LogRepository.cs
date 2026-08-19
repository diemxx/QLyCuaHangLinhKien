using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using ComputerStore_WPF.DataAccess;
using ComputerStore_WPF.Models;

namespace ComputerStore_WPF.Repositories
{
    public class LogRepository
    {
        public void InsertLog(string tuKhoa, string nguoiTimKiem)
        {
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("INSERT INTO LichSuTimKiem (TuKhoa, ThoiGian, NguoiTimKiem) VALUES (@TK, GETDATE(), @NTK)", conn))
                {
                    cmd.Parameters.AddWithValue("@TK", tuKhoa);
                    cmd.Parameters.AddWithValue("@NTK", (object)nguoiTimKiem ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<LichSuTimKiemModel> GetAll()
        {
            var list = new List<LichSuTimKiemModel>();
            using (var conn = AppDbContext.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"SELECT ls.*, nv.HoTen AS TenNguoiTimKiem FROM LichSuTimKiem ls LEFT JOIN NhanVien nv ON ls.NguoiTimKiem=nv.MaNV ORDER BY ls.ThoiGian DESC", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new LichSuTimKiemModel
                        {
                            MaLS = Convert.ToInt32(reader["MaLS"]),
                            TuKhoa = reader["TuKhoa"].ToString(),
                            ThoiGian = Convert.ToDateTime(reader["ThoiGian"]),
                            NguoiTimKiem = reader["NguoiTimKiem"]?.ToString(),
                            TenNguoiTimKiem = reader["TenNguoiTimKiem"]?.ToString()
                        });
                    }
                }
            }
            return list;
        }
    }
}
