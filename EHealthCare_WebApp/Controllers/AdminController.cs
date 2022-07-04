using DbEHealthcare.Entities;
using EHealthCare_WebApp.Models;
using EHealthCare_WebApp.Utils;
using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace EHealthCare_WebApp.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult DangKybs()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangKybs(string name, string email, HttpPostedFileBase file)
        {
            DbEHealthcare.Entities.BacSi bs = EHealthCareService.Instance.getBacSiBy(b => b.email == email);
            if (bs != null)
            {
                return View();
            }
            String newNameFile = SetNameCV(file);
            SaveCV(file, newNameFile);
            DbEHealthcare.Entities.HoSo hs = new DbEHealthcare.Entities.HoSo();
            hs.email = email;
            hs.tencv = newNameFile;
            hs.ten_ung_vien = name;
            EHealthCareService.Instance.Add(hs);
            EHealthCareService.Instance.Save();
            return RedirectToAction("Index", "Home");

        }

        private void SaveCV(HttpPostedFileBase file, string newNameFile)
        {
            try
            {
                file.SaveAs(Path.Combine(Server.MapPath("~/Content/Media"), newNameFile));
            }
            catch (Exception e)
            {

            }
        }

        private string SetNameCV(HttpPostedFileBase file)
        {
            String name = Path.GetFileNameWithoutExtension(file.FileName);
            String extension = Path.GetExtension(file.FileName);
            return name + DateTime.Now.ToString("yyyyMMddTHHmmss") + extension;
        }

        public ActionResult QuanLyCV()
        {
            List<DbEHealthcare.Entities.HoSo> hoSos = EHealthCareService.Instance.getHoSos();

            return View(hoSos);
        }
        public ActionResult QuanLyBV()
        {
            List<DbEHealthcare.Entities.BenhVien> benhViens = EHealthCareService.Instance.getBenhViens();
            return View(benhViens);
        }
        public ActionResult Addbv()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Addbv(string id, string name, string diachi, string httc)
        {
            DbEHealthcare.Entities.BenhVien bv = EHealthCareService.Instance.getBenhViens().SingleOrDefault(b => b.id.CompareTo(id) == 0);
            if (bv == null)
            {
                DbEHealthcare.Entities.BenhVien b = new DbEHealthcare.Entities.BenhVien();
                b.id = id;
                b.ten_bv = name;
                b.diachi_bv = diachi;
                b.httc = httc;
                EHealthCareService.Instance.Add(b);
                EHealthCareService.Instance.Save();
            }
            return RedirectToAction("QuanLyBV");
        }
        public ActionResult Delete(string ibenhvien)
        {

            DbEHealthcare.Entities.BenhVien bv = EHealthCareService.Instance.getBenhViens().SingleOrDefault(h => h.id.CompareTo(ibenhvien) == 0);
            EHealthCareService.Instance.Delete(bv);
            EHealthCareService.Instance.Save();
            return RedirectToAction("QuanLyBV");
        }
        private readonly Random _random = new Random();
        public string RandomString(int size, bool lowerCase = false)
        {
            var builder = new StringBuilder(size);

            // Unicode/ASCII Letters are divided into two blocks
            // (Letters 65–90 / 97–122):
            // The first group containing the uppercase letters and
            // the second group containing the lowercase.  

            // char is a single Unicode character  
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length=26  

            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }
        public string RandomPassword()
        {
            var passwordBuilder = new StringBuilder();

            // 4-Letters lower case   
            passwordBuilder.Append(RandomString(4, true));

            // 4-Digits between 1000 and 9999  
            passwordBuilder.Append(RandomNumber(1000, 9999));

            // 2-Letters upper case  
            passwordBuilder.Append(RandomString(2));
            return passwordBuilder.ToString();
        }
        public int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }
        public ActionResult Addbs(string name, string email)
        {
            DbEHealthcare.Entities.BacSi bs = new DbEHealthcare.Entities.BacSi();
            bs.email = email;
            bs.hoten = name;
            ViewData["benhvien"] = EHealthCareService.Instance.getBenhViens();
            ViewData["chuyenkhoa"] = EHealthCareService.Instance.getChuyenKhoas();
            List<TrinhDo> trinhDos = EHealthCareService.Instance.getTrinhDos();
            ViewData["trinhdo"] = trinhDos;
            return View(bs);
        }
        public ActionResult Xoa(string mail)
        {
            DbEHealthcare.Entities.HoSo hs = EHealthCareService.Instance.getHoSos().SingleOrDefault(h => h.email.CompareTo(mail) == 0);
            EHealthCareService.Instance.Delete(hs);
            EHealthCareService.Instance.Save();
            return RedirectToAction("QuanLyCV");
        }
        [HttpPost]
        public ActionResult Addbs(string name, string email, string benhvien, int chuyenkhoa1, int chuyenkhoa2, int chuyenkhoa3)
        {
            DbEHealthcare.Entities.BacSi bs = new DbEHealthcare.Entities.BacSi();
            DbEHealthcare.Entities.ChiTietChuyenKhoa ct = new DbEHealthcare.Entities.ChiTietChuyenKhoa();
            bs.email = email;
            bs.hoten = name;
            bs.matkhau = RandomPassword();
            bs.id_bv = benhvien;
            EHealthCareService.Instance.Add(bs);
            EHealthCareService.Instance.Save();
            ct.email_BS = email;
            ct.ma_CK = chuyenkhoa1;
            EHealthCareService.Instance.Add(ct);
            EHealthCareService.Instance.Save();
            if (chuyenkhoa2 != -1)
            {
                DbEHealthcare.Entities.ChiTietChuyenKhoa ct2 = new DbEHealthcare.Entities.ChiTietChuyenKhoa();
                ct2.email_BS = email;
                ct2.ma_CK = chuyenkhoa2;
                EHealthCareService.Instance.Add(ct2);
                EHealthCareService.Instance.Save();
            }
            if (chuyenkhoa3 != -1)
            {
                DbEHealthcare.Entities.ChiTietChuyenKhoa ct3 = new DbEHealthcare.Entities.ChiTietChuyenKhoa();
                ct3.email_BS = email;
                ct3.ma_CK = chuyenkhoa3;
                EHealthCareService.Instance.Add(ct3);
                EHealthCareService.Instance.Save();
            }

            string FilePath = Path.Combine(Server.MapPath("~/Utils/TemplateMail"), "MailThongBaoNhanBacSi.html");
            string text = System.IO.File.ReadAllText(FilePath);
            string tb = "Mật khẩu mặc định là: " + bs.matkhau + ".Vui lòng thay đổi mật khẩu lần đầu trước khi sử dụng dịch vụ!";
            text = text.Replace("matkhau", tb);
            string body = text;
            string subject = "Thông Báo Cấp Tài Khoản Bác Sĩ";
            MailSMTP.Send(body, subject, email);
            return RedirectToAction("QuanLyCV");
        }

        public ActionResult Xemcv(string tencv)
        {
            ViewBag.tencv = tencv;
            return View();
        }

        public ActionResult ThongKe()
        {
            int soLuongUser = EHealthCareService.Instance.getBenhNhans().Count();

            int soLuongBS = EHealthCareService.Instance.getBacSis().Count();

            int soLuongBV = EHealthCareService.Instance.getBenhViens().Count();

            int soCuocTuVan = EHealthCareService.Instance.getChiTietTuVans().Count();
            ViewBag.bn = soLuongUser;
            ViewBag.bs = soLuongBS;
            ViewBag.bv = soLuongBV;
            ViewBag.cttv = soCuocTuVan;

            return View();

        }

        // Quản lý info
        public ActionResult QuanLyInfo()
        {
            List<BacSiNew> ls = new List<BacSiNew>();
            try
            {
                var path = Server.MapPath(@"~/App_Data/UpdateInfo/infoUpdates.txt");
                using (StreamReader sr = new StreamReader(path, true))
                {
                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        BacSiNew cl = new BacSiNew();
                        cl.email = line;
                        line = sr.ReadLine();
                        cl.time = DateTime.Parse(line);
                        line = sr.ReadLine();
                        cl.name_file = line;
                        ls.Add(cl);
                    }
                }
                ViewData["BacSi"] = ls;

                List<BacSi> oldBacSi = new List<BacSi>();
                ls.ForEach(l =>
                {
                    BacSi bs = EHealthCareService.Instance.getBacSiBy(b => b.email.CompareTo(l.email) == 0);
                    oldBacSi.Add(bs);
                });
                ViewData["OldBacSis"] = oldBacSi;


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return View();
        }

        // Xóa file cập nhật thông tin
        public ActionResult XoaFileInfo(string email, string date, string namefile)
        {

            List<BacSiNew> ls = new List<BacSiNew>();
            try
            {
                var path = Server.MapPath(@"~/App_Data/UpdateInfo/infoUpdates.txt");
                using (StreamReader sr = new StreamReader(path, true))
                {
                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        BacSiNew cl = new BacSiNew();
                        cl.email = line;
                        line = sr.ReadLine();
                        cl.time = DateTime.Parse(line);
                        line = sr.ReadLine();
                        cl.name_file = line;
                        ls.Add(cl);
                    }
                }
                BacSiNew bs_delete = ls.First(b => b.email.CompareTo(email) == 0 && b.name_file.CompareTo(namefile) == 0);
                ls.Remove(bs_delete);

                using (StreamWriter writer = new StreamWriter(path, false))
                {
                    ls.ForEach(l =>
                    {
                        writer.WriteLine(l.email);
                        writer.WriteLine(l.time);
                        writer.WriteLine(l.name_file);
                    });
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return RedirectToAction("QuanLyInfo");
        }

        /// <summary>
        /// Xem và cập nhật thông tin cho bác sĩ
        /// </summary>
        /// <param name="nameFile"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public ActionResult DetailUpdate(string nameFile, string email)
        {
            List<BacSiNew> ls = new List<BacSiNew>();
            try
            {
                var path = Server.MapPath(@"~/App_Data/UpdateInfo/infoUpdates.txt");
                using (StreamReader sr = new StreamReader(path, true))
                {
                    string line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        BacSiNew cl = new BacSiNew();
                        cl.email = line;
                        line = sr.ReadLine();
                        cl.time = DateTime.Parse(line);
                        line = sr.ReadLine();
                        cl.name_file = line;
                        ls.Add(cl);
                    }
                }
                BacSi bs = EHealthCareService.Instance.getBacSiBy(b => b.email.CompareTo(email) == 0);
                ViewData["bacsi"] = bs;
                ViewData["namefile"] = nameFile;

                List<BenhVien> benhviens = EHealthCareService.Instance.getBenhViens();
                ViewData["benhviens"] = benhviens;

                List<TrinhDo> trinhdos = EHealthCareService.Instance.getTrinhDos();
                ViewData["trinhdos"] = trinhdos;
                List<ChiTietChuyenKhoa> list_ck_of_bs = EHealthCareService.Instance.getChiTietChuyenKhoa().Where(l => l.email_BS.CompareTo(bs.email) == 0).ToList();
                ViewData["ckofbs"] = list_ck_of_bs;

                List<ChuyenKhoa> chuyenkhoas = EHealthCareService.Instance.getChuyenKhoas();
                ViewData["chuyenkhoas"] = chuyenkhoas;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return View();

        }

        [HttpPost]
        public ActionResult DetailUpdate(string email, string hoten, string benhvien,string gioitinh,HttpPostedFileBase hinh,string trinhdo,string chuyenkhoa1, string chuyenkhoa2,string chuyenkhoa3)
        {
            BacSi bacsi = EHealthCareService.Instance.getBacSiBy(b => b.email.CompareTo(email) == 0);

            EHealthCareService.Instance.Delete(bacsi, true);

            bacsi.hoten = hoten;
            bacsi.id_bv = benhvien;
            bacsi.gioitinh = gioitinh == "1" ? true : false;
            bacsi.mahv = trinhdo;

            List<ChiTietChuyenKhoa> ct = new List<ChiTietChuyenKhoa>();
            ChiTietChuyenKhoa ct_ck = new ChiTietChuyenKhoa() ;
            ct_ck.email_BS = bacsi.email;
            ct_ck.ma_CK = int.Parse(chuyenkhoa1);
            ct.Add(ct_ck);
            if(chuyenkhoa2 != "-1")
            {
                ChiTietChuyenKhoa ct_ck2 = new ChiTietChuyenKhoa();
                ct_ck2.email_BS = bacsi.email;
                ct_ck2.ma_CK = int.Parse(chuyenkhoa2);
                ct.Add(ct_ck2);
            }
            if (chuyenkhoa3 != "-1")
            {
                ChiTietChuyenKhoa ct_ck3 = new ChiTietChuyenKhoa();
                ct_ck3.email_BS = bacsi.email;
                ct_ck3.ma_CK = int.Parse(chuyenkhoa3);
                ct.Add(ct_ck3);
            }
            if(hinh != null)
            {
                String newNameImg = SetNameImg(hinh);
                SaveImg(hinh, newNameImg);
                bacsi.hinhanh = newNameImg;
            }
            EHealthCareService.Instance.UpdateBS(bacsi);
            EHealthCareService.Instance.Save();

            EHealthCareService.Instance.Add(ct);
            EHealthCareService.Instance.Save();
            return RedirectToAction("QuanLyInfo");
        }
        private void SaveImg(HttpPostedFileBase hinhanh, string newNameImg)
        {
            hinhanh.SaveAs(Path.Combine(Server.MapPath("~/Content/images"), newNameImg));
        }

        private string SetNameImg(HttpPostedFileBase hinhanh)
        {
            String name = Path.GetFileNameWithoutExtension(hinhanh.FileName);
            String extension = Path.GetExtension(hinhanh.FileName);
            return name + DateTime.Now.ToString("yyyyMMddTHHmmss") + extension;
        }
    }
}