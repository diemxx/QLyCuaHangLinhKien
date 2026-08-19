using ComputerStore_WPF.Models;
using System.Collections.Generic;

namespace ComputerStore_WPF.Repositories
{
   
    public interface ISanPhamRepository
    {
        List<SanPhamModel> GetAll();
        SanPhamModel GetById(string maSP);
        List<SanPhamModel> Search(string keyword, string maLoai = null, string maNCC = null, decimal? giaMin = null, decimal? giaMax = null);
        bool Insert(SanPhamModel sp);
        bool Update(SanPhamModel sp);
        bool Delete(string maSP);
        List<LoaiSanPhamModel> GetAllLoaiSanPham();
        bool InsertLoaiSanPham(LoaiSanPhamModel loai);
        bool UpdateLoaiSanPham(LoaiSanPhamModel loai);
        bool DeleteLoaiSanPham(string maLoai);
        List<NhaCungCapModel> GetAllNhaCungCap();
        bool InsertNhaCungCap(NhaCungCapModel ncc);
        bool UpdateNhaCungCap(NhaCungCapModel ncc);
        bool DeleteNhaCungCap(string maNCC);
    }
}
