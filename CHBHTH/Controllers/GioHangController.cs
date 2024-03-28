using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CHBHTH.Models;

namespace CHBHTH.Controllers
{
    public class GioHangController : Controller
    {
        private QLbanhang db = new QLbanhang();
        public List<GioHang> LayGioHang(int userId)
        {
            List<GioHang> lstGioHang = Session[$"GioHang_{userId}"] as List<GioHang>;
            if (lstGioHang == null)
            {
                //Nếu giỏ hàng chưa tồn tại thì khởi tạo
                lstGioHang = new List<GioHang>();
                Session[$"GioHang_{userId}"] = lstGioHang;
            }
            return lstGioHang;
        }
        public ActionResult ThemGioHang(int iMasp, string strURL)
        {
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == iMasp);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            // Lấy UserId từ Session
            int? userId = Session["UserId"] as int?;

            if (userId != null)
            {
                // Lấy giỏ hàng theo UserId
                List<GioHang> lstGioHang = LayGioHang(userId.Value);
                GioHang sanpham = lstGioHang.Find(n => n.iMasp == iMasp);
                if (sanpham == null)
                {
                    sanpham = new GioHang(iMasp, sp.TenSP, sp.AnhSP, double.Parse(sp.GiaBan.ToString()), 1, userId.Value);
                    lstGioHang.Add(sanpham);
                }
                else
                {
                    sanpham.iSoLuong++;
                }
                Session[$"GioHang_{userId.Value}"] = lstGioHang;

                return Redirect(strURL);
            }
            else
            {
                return RedirectToAction("Dangnhap", "User");
            }
        }
        //Cap nhat gio hang
        public ActionResult CapNhatGioHang(int iMaSP, FormCollection f)
        {
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == iMaSP);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            int? userId = Session["UserId"] as int?;
            if (userId != null)
            {
                List<GioHang> lstGioHang = LayGioHang(userId.Value);
                GioHang sanpham = lstGioHang.SingleOrDefault(n => n.iMasp == iMaSP);
                if (sanpham != null)
                {
                    int newQuantity;
                    if (int.TryParse(f["txtSoLuong"].ToString(), out newQuantity))
                    {
                        sanpham.iSoLuong = newQuantity;
                    }
                }
                Session[$"GioHang_{userId.Value}"] = lstGioHang;
            }

            return RedirectToAction("GioHang");
        }
        public ActionResult XoaGioHang(int iMaSP)
        {
            SanPham sp = db.SanPhams.SingleOrDefault(n => n.MaSP == iMaSP);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            int? userId = Session["UserId"] as int?;
            if (userId != null)
            {
                List<GioHang> lstGioHang = LayGioHang(userId.Value);
                GioHang sanpham = lstGioHang.SingleOrDefault(n => n.iMasp == iMaSP);
                if (sanpham != null)
                {
                    lstGioHang.RemoveAll(n => n.iMasp == iMaSP);
                }
                Session[$"GioHang_{userId.Value}"] = lstGioHang;

                if (lstGioHang.Count == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            return RedirectToAction("GioHang");
        }
        public ActionResult GioHang()
        {
            int? userId = Session["UserId"] as int?;
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<GioHang> lstGioHang = LayGioHang(userId.Value);

            return View(lstGioHang);
        }
        //Tính tổng số lượng và tổng tiền
        //Tính tổng số lượng
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;
        }
        //Tính tổng thành tiền
        private double TongTien()
        {
            double dTongTien = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                dTongTien = lstGioHang.Sum(n => n.ThanhTien);
            }
            return dTongTien;
        }
        public ActionResult GioHangPartial()
        {
            if (TongSoLuong() == 0)
            {
                return PartialView();
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }
        public ActionResult SuaGioHang()
        {

            int? userId = Session["UserId"] as int?;
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<GioHang> lstGioHang = LayGioHang(userId.Value);

            return View(lstGioHang);
        }

        [HttpPost]
        public ActionResult DatHang(FormCollection donhangForm)
        {
            // Kiểm tra đăng nhập
            int? userId = Session["UserId"] as int?;
            if (userId == null)
            {
                return RedirectToAction("Dangnhap", "User");
            }
            List<GioHang> lstGioHang = LayGioHang(userId.Value); if (lstGioHang == null || lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            string diachinhanhang = donhangForm["Diachinhanhang"].ToString();
            string thanhtoan = donhangForm["MaTT"].ToString();
            int ptthanhtoan = Int32.Parse(thanhtoan);
            DonHang ddh = new DonHang();
            ddh.MaNguoiDung = userId.Value;
            ddh.NgayDat = DateTime.Now;
            ddh.ThanhToan = ptthanhtoan;
            ddh.DiaChiNhanHang = diachinhanhang;

            decimal tongtien = lstGioHang.Sum(item => item.iSoLuong * (decimal)item.dDonGia);
            ddh.TongTien = tongtien;

            db.DonHangs.Add(ddh);
            db.SaveChanges();

            // Thêm chi tiết đơn hàng
            foreach (var item in lstGioHang)
            {
                ChiTietDonHang ctDH = new ChiTietDonHang();
                decimal thanhtien = item.iSoLuong * (decimal)item.dDonGia;
                ctDH.MaDon = ddh.MaDon;
                ctDH.MaSP = item.iMasp;
                ctDH.SoLuong = item.iSoLuong;
                ctDH.DonGia = (decimal)item.dDonGia;
                ctDH.ThanhTien = thanhtien;
                ctDH.PhuongThucThanhToan = ptthanhtoan;
                db.ChiTietDonHangs.Add(ctDH);
            }
            db.SaveChanges();

            // Xóa giỏ hàng sau khi đã đặt hàng thành công
            Session.Remove($"GioHang_{userId.Value}");

            return RedirectToAction("Index", "Donhangs");
        }

        public ActionResult ThanhToanDonHang()
        {
            ViewBag.MaTT = new SelectList(new[]
                {
                        new { MaTT = 1, TenPT="Thanh toán tiền mặt" },
                        new { MaTT = 2, TenPT="Thanh toán chuyển khoản" },
                    }, "MaTT", "TenPT", 1);
            int? userId = Session["UserId"] as int?;
            if (userId == null)
            {
                return RedirectToAction("Dangnhap", "User");
            }

            // Kiểm tra giỏ hàng
            List<GioHang> lstGioHang = LayGioHang(userId.Value);
            if (lstGioHang == null || lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            decimal tongTien = lstGioHang.Sum(item => item.iSoLuong * (decimal)item.dDonGia);
            var userList = db.TaiKhoans.Select(u => new SelectListItem
            {
                Value = u.MaNguoiDung.ToString(),
                Text = u.HoTen
            }).ToList();
            // Tạo đơn hàng
            DonHang ddh = new DonHang
            {
                MaNguoiDungList = userList,
                MaNguoiDung = userId.Value,
                NgayDat = DateTime.Now
            };

            ViewBag.tongtien = tongTien;
            return View(ddh);
        }


    }
}