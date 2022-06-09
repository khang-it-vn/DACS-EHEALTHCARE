using DbEHealthcare.Entities;
using EHealthCare_WebApp.Models;
using EHealthCare_WebApp.Utils;
using Services;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace EHealthCare_WebApp.Controllers
{
    public class ProbnController : Controller
    {
        private string emailbs = "";
        // GET: Probn
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Profile()
        {
            string email = Session["email"] as string;
            BenhNhan bn = EHealthCareService.Instance.getBenhNhan(b => b.email == email );
            return View(bn);
        }
        [HttpPost]
        public ActionResult Update(String hoten, DateTime ntns, bool gioitinh, string dc, string sdt, HttpPostedFileBase hinhanh)
        {

            string mail = Session["email"] as string;

            BenhNhan bn = EHealthCareService.Instance.getBenhNhan(b => b.email == mail);
            if(hinhanh != null)
            {
                String newNameImg = SetNameImg(hinhanh);
                SaveImg(hinhanh,newNameImg);
                bn.hinhanh = newNameImg;
            }
            bn.hoten = hoten;
            bn.ntns = ntns;
            bn.gioitinh = gioitinh;
            bn.dc = dc;
            bn.sdt = sdt;

            EHealthCareService.Instance.UpdateBN(bn);
            // service.Save();

            return RedirectToAction("Profile");
        }

        private void SaveImg(HttpPostedFileBase hinhanh,string newNameImg)
        {
            hinhanh.SaveAs(Path.Combine(Server.MapPath("~/Content/images"),newNameImg));   
        }

        private string SetNameImg(HttpPostedFileBase hinhanh)
        {
            String name = Path.GetFileNameWithoutExtension(hinhanh.FileName);
            String extension = Path.GetExtension(hinhanh.FileName);
            return name + DateTime.Now.ToString("yyyyMMddTHHmmss") + extension;
        }

       public ActionResult Home()
        {
            List<BacSi> bacsis = EHealthCareService.Instance.getBacSis();
            List<ChuyenKhoa> chuyenkhoas = EHealthCareService.Instance.getChuyenKhoas();
            ViewData["bacsis"] = bacsis;
            ViewData["chuyenkhoas"] = chuyenkhoas;

             return View();
        }
        public ActionResult Regis(String email)
        {
            if (email != null)
            {
                this.emailbs = email;
            }
            else
            {
                this.emailbs = Session["bs_email"] as string;
            }
            BacSi bs = EHealthCareService.Instance.getBacSiBy(b => b.email == this.emailbs);
            List<LichTuVan> lichtuvan = EHealthCareService.Instance.getLichTuVanBy(bs);
            dynamic model_bs_ltv = new ExpandoObject();
            model_bs_ltv.BacSi = bs;
            
            ViewData["LichTuVans"] = lichtuvan;
            return View(model_bs_ltv);
        }

        public ActionResult Accept(String email, String ngay, String gio)
        {
            try
            {
                DateTime _ntv = DateTime.Parse(ngay + " " + gio);

                String email_BN = (String)Session["email"];
                this.emailbs = email;
                Session["bs_email"] = email;
                LichTuVan ltv = EHealthCareService.Instance.getLichTuVanBy(l => DateTime.Compare(l.ntv,_ntv) == 0 && l.email_BS.CompareTo(emailbs) == 0);
                ltv.email_BN = email_BN;

                string file_room = Server.MapPath("~/App_Data/Data_Process/room.txt");
                string room_string = System.IO.File.ReadAllText(file_room);
                int room = int.Parse(room_string);
                room++;
                System.IO.File.WriteAllText(file_room, room.ToString());
                
                ltv.phongtuvan = room;
                EHealthCareService.Instance.Edit(ltv);
                EHealthCareService.Instance.Save();

                string body = "<h1>Hoàn tất mail<h1>";
                string subject = "Thông Báo Xác Nhận Đăng Ký Lịch Tư Vấn Khám Bệnh";

                MailSMTP.Send(body, subject, email_BN);
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
            

            return RedirectToAction("Home");
        }

        public ActionResult Filter(int chuyenkhoa)
        {
            List<BacSi> bacsis = EHealthCareService.Instance.getBacSis();
            List<BacSi> bacsis_filter = bacsis.Where(bs => bs.ma_CK == chuyenkhoa).ToList();
            List<ChuyenKhoa> chuyenkhoas = EHealthCareService.Instance.getChuyenKhoas();
            ViewData["bacsis"] = bacsis_filter;
            ViewData["chuyenkhoas"] = chuyenkhoas;
            return View();
        }

        public ActionResult History()
        {
            String email = Session["email"] as String;
            List<LichTuVan> lichtuvans = EHealthCareService.Instance.getLichTuVans();
            List<ChiTietTuVan> chitietltv = EHealthCareService.Instance.getChiTietTuVans();
            List<LichTuVan> lichtuvanOfSession = lichtuvans.Where(ltv => DateTime.Compare(DateTime.Now, ltv.ntv) > 0 && String.Compare(ltv.email_BN, email) == 0).ToList();
            ViewData["lichtuvans"] = lichtuvanOfSession;

            List<ChiTietTuVanDAO> chitietlichtuvan = new List<ChiTietTuVanDAO>();
            foreach(LichTuVan l in lichtuvanOfSession)
            {
                foreach(ChiTietTuVan ct in chitietltv)
                {
                    if(l.id_cttv == ct.id_cttv)
                    {
                        ChiTietTuVanDAO cttv_dao = new ChiTietTuVanDAO();
                        cttv_dao.id_cttv = ct.id_cttv;
                        cttv_dao.chiDinh = ct.chiDinh;
                        cttv_dao.chuanDoan = ct.chuanDoan;
                        cttv_dao.trieuChung = ct.trieuChung;
                        cttv_dao.ghiChu = ct.ghiChu;
                        chitietlichtuvan.Add(cttv_dao);
                    }
                }
            }

            JavaScriptSerializer Json = new JavaScriptSerializer();
            string chitietlichtuvan_json = Json.Serialize(chitietlichtuvan);
            
            ViewData["chitiettuvans"] = chitietlichtuvan_json;
            return View();
        }

        public ActionResult FilterHistory(DateTime date)
        {
            if(date > DateTime.Now)
            {
                return RedirectToAction("History");
            }
            String email = Session["email"] as String;
            List<LichTuVan> lichtuvans = EHealthCareService.Instance.getLichTuVans();
            List<ChiTietTuVan> chitietltv = EHealthCareService.Instance.getChiTietTuVans();
            List<LichTuVan> lichtuvanOfSession = lichtuvans.Where(ltv => ltv.ntv.Day == date.Day && ltv.ntv.Month == date.Month && ltv.ntv.Year == date.Year && String.Compare(ltv.email_BN, email) == 0).ToList();
            ViewData["lichtuvans"] = lichtuvanOfSession;

            List<ChiTietTuVanDAO> chitietlichtuvan = new List<ChiTietTuVanDAO>();
            foreach (LichTuVan l in lichtuvanOfSession)
            {
                foreach (ChiTietTuVan ct in chitietltv)
                {
                    if (l.id_cttv == ct.id_cttv)
                    {
                        ChiTietTuVanDAO cttv_dao = new ChiTietTuVanDAO();
                        cttv_dao.id_cttv = ct.id_cttv;
                        cttv_dao.chiDinh = ct.chiDinh;
                        cttv_dao.chuanDoan = ct.chuanDoan;
                        cttv_dao.trieuChung = ct.trieuChung;
                        cttv_dao.ghiChu = ct.ghiChu;
                        chitietlichtuvan.Add(cttv_dao);
                    }
                }
            }

            JavaScriptSerializer Json = new JavaScriptSerializer();
            string chitietlichtuvan_json = Json.Serialize(chitietlichtuvan);

            ViewData["chitiettuvans"] = chitietlichtuvan_json;
            return View();
        }
        public ActionResult Detail(int id)
        {
            ChiTietTuVan cttv = EHealthCareService.Instance.getChiTietTuVan(id);
            ViewData["cttv"] = cttv;
            return View();
        }

        public ActionResult FilterName(string hoten)
        {
            List<BacSi> bacsis = EHealthCareService.Instance.getBacSis();
            List<BacSi> bacsis_filter = bacsis.Where(bs => bs.hoten.ToLower().IndexOf(hoten.ToLower()) != -1).ToList();
            List<ChuyenKhoa> chuyenkhoas = EHealthCareService.Instance.getChuyenKhoas();
            ViewData["bacsis"] = bacsis_filter;
            ViewData["chuyenkhoas"] = chuyenkhoas;
            return View();
        }

        public ActionResult Books()
        {
            String email = Session["email"] as String;
            List<LichTuVan> lichtuvans = EHealthCareService.Instance.getLichTuVans();
            List<ChiTietTuVan> chitietltv = EHealthCareService.Instance.getChiTietTuVans();
            List<LichTuVan> lichtuvanOfSession = lichtuvans.Where(ltv =>  ltv.ntv > DateTime.Now && String.Compare(ltv.email_BN, email) == 0).ToList();
            ViewData["lichtuvans"] = lichtuvanOfSession;

            List<ChiTietTuVanDAO> chitietlichtuvan = new List<ChiTietTuVanDAO>();
            foreach (LichTuVan l in lichtuvanOfSession)
            {
                foreach (ChiTietTuVan ct in chitietltv)
                {
                    if (l.id_cttv == ct.id_cttv)
                    {
                        ChiTietTuVanDAO cttv_dao = new ChiTietTuVanDAO();
                        cttv_dao.id_cttv = ct.id_cttv;
                        cttv_dao.chiDinh = ct.chiDinh;
                        cttv_dao.chuanDoan = ct.chuanDoan;
                        cttv_dao.trieuChung = ct.trieuChung;
                        cttv_dao.ghiChu = ct.ghiChu;
                        chitietlichtuvan.Add(cttv_dao);
                    }
                }
            }

            JavaScriptSerializer Json = new JavaScriptSerializer();
            string chitietlichtuvan_json = Json.Serialize(chitietlichtuvan);

            ViewData["chitiettuvans"] = chitietlichtuvan_json;
            return View();
        }
    }
}