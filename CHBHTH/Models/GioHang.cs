using CHBHTH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CHBHTH.Models
{
    public class GioHang
    {
        private QLbanhang db = new QLbanhang();
        public int iMasp { get; set; }
        public string sTensp { get; set; }
        public string sAnhBia { get; set; }
        public double dDonGia { get; set; }
        public int iSoLuong { get; set; }
        public int TaiKhoanId { get; set; }
        public double ThanhTien
        {
            get { return iSoLuong * dDonGia; }
        }
        //Hàm tạo cho giỏ hàng
        public GioHang() { }
        public GioHang(int masp, string tensp, string anhBia, double donGia, int soLuong, int taiKhoanId)
        {
            iMasp = masp;
            sTensp = tensp;
            sAnhBia = anhBia;
            dDonGia = donGia;
            iSoLuong = soLuong;
            TaiKhoanId = taiKhoanId;
        }

    }
}