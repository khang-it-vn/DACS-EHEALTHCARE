using DbEHealthcare.Entities;
using EHealthCare_WebApp.Utils;
using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EHealthCare_WebApp.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CheckLogin(string mail, string password)
        {
            Admin ad = EHealthCareService.Instance.getAdmins().SingleOrDefault(a => a.email.CompareTo(mail) == 0 && a.Pass.CompareTo(password) == 0);
            if(ad != null)
            {
                Session["email"] = ad.email;
                return RedirectToAction("QuanLyCV", "Admin");
            }
            BenhNhan bn = new BenhNhan() { email = mail, matkhau = password};
            BenhNhan info_bn = EHealthCareService.Instance.Login(bn);
            if (info_bn != null)
            {
                Session["img"] = info_bn.hinhanh;
                Session["email"] = info_bn.email;
                return RedirectToAction("Home", "Probn");
            }
            BacSi bs = new BacSi() { email = mail, matkhau = password };

            BacSi info_bs = EHealthCareService.Instance.getBacSiBy(b => b.email == mail && password == b.matkhau);
            
            if(info_bs != null)
            {
                Session["img"] = info_bs.hinhanh;
                Session["email"] = info_bs.email;
                return RedirectToAction("Manage", "Probs");
            }
            return RedirectToAction("Index");
        }

        public ActionResult DangKy()
        {
            string tb = Session["tb"] as string;
            ViewBag.tb = tb == null ? "" : tb;
            return View();
        }

        public ActionResult NhapMailForgotPass()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NhapMailForgotPass(string email)
        {
            BenhNhan bn = new BenhNhan() { email = email };
            bool checkExists = EHealthCareService.Instance.MailExists(bn);
            if (checkExists)
            {
                string FilePath = Path.Combine(Server.MapPath("~/Utils/TemplateMail"), "MailQuenMatKhau.html");
                string text = System.IO.File.ReadAllText(FilePath);
                string url = "http://localhost:57513/Login/ThayDoiMatKhau?email=" + email.Trim();
                text = text.Replace("http://localhost:57513/RePass/", url);
                string body = text;
                string subject = "Thông Báo Xác Nhận Thay Đổi Mật Khẩu";

                MailSMTP.Send(body, subject, email);
                return RedirectToAction("NhapMailForgotPass");
            }
            Session["tb"] = "Mail chưa được đăng ký!";
            return RedirectToAction("NhapMailForgotPass");
        }

        public ActionResult ThayDoiMatKhau(string email)
        {
            ViewBag.email = email;
            return View();
        }

        [HttpPost]
        public ActionResult ThayDoiMatKhau(string email, string newpassword, string password)
        {
            if (newpassword.CompareTo(password) == 0)
            {
                BenhNhan bn = EHealthCareService.Instance.getBenhNhan(b => b.email == email);
                bn.matkhau = newpassword;
                EHealthCareService.Instance.UpdateBN(bn);
                EHealthCareService.Instance.Save();
                return RedirectToAction("Index");
            }

            return View();
        }
        public ActionResult Accept(string name, string email, string password, int gioitinh, string sdt, string quoctich, DateTime ntns, string diachi)
        {
            BenhNhan bn = new BenhNhan();
            bn.hoten = name;
            bn.email = email;
            bn.matkhau = password;
            bn.gioitinh = gioitinh == 1 ? true : false;
            bn.sdt = sdt;
            bn.quoctich = quoctich;
            bn.ntns = ntns;
            bn.dc = diachi;
            bn.hinhanh = "avartar.png";
            EHealthCareService.Instance.Add(bn);
            EHealthCareService.Instance.Save();
            return RedirectToAction("Index");

        }
        [HttpPost]
        public ActionResult DangKy(string name, string mail, string password, int gioitinh, string sdt, string quoctich, DateTime ntns, string diachi)
        {
            BenhNhan bn = new BenhNhan() { email = mail };
            bool checkExists = EHealthCareService.Instance.MailExists(bn);
            if (!checkExists)
            {
                string FilePath = Path.Combine(Server.MapPath("~/Utils/TemplateMail"), "MailXacNhanTaiKhoan.html");
                string text = System.IO.File.ReadAllText(FilePath);
                string url = "http://localhost:57513/Login/Accept?email=" + mail.Trim() + "&name=" + name + "&password=" + password + "&gioitinh=" + gioitinh + "&sdt=" + sdt + "&quoctich=" + quoctich + "&ntns=" + ntns + "&diachi=" + diachi;
                text = text.Replace("http://localhost:57513/Accept/", url);
                string body = text;
                string subject = "Thông Báo Xác Nhận Đăng Ký Tài Khoản E-Healthcare";
                MailSMTP.Send(body, subject, mail);
                return RedirectToAction("DangKy", "Login");
            }
            Session["tb"] = "Mail đã được đăng ký!";
            return RedirectToAction("DangKy");

        }
    }
}