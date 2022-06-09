using DbEHealthcare.Entities;
using EHealthCare_WebApp.Models;
using Services;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace EHealthCare_WebApp.Controllers
{
    public class ProbsController : Controller
    {
        // GET: Probs
        public ActionResult Index()
        {
            string email = Session["email"] as string;
            BacSi bacsi = EHealthCareService.Instance.getBacSiBy(bs => bs.email.CompareTo(email) == 0);
            ViewData["bacsi"] = bacsi;
            return View();
        }

        public ActionResult Manage()
        {
            string email = Session["email"] as string;
            BacSi bs = EHealthCareService.Instance.getBacSiBy(b => b.email == email);
            List<LichTuVan> lichtuvans = EHealthCareService.Instance.getLichTuVanBy(bs);
            dynamic model_bs_ltv = new ExpandoObject();
            model_bs_ltv.BacSi = bs;

            ViewData["LichTuVans"] = lichtuvans;
            return View(model_bs_ltv);

        }

        public ActionResult Books(String email,String ngay,String gio)
        {
            DateTime _ntv = DateTime.Parse(ngay + " " + gio);

            String email_bs = (String)Session["email"];

            ChiTietTuVan cttv = new ChiTietTuVan() { ketQua = false };

            EHealthCareService.Instance.AddChiTietTuVan(cttv);
            EHealthCareService.Instance.Save();

            List<ChiTietTuVan> cttvs = EHealthCareService.Instance.getChiTietTuVans();

            ChiTietTuVan newest_cttv = cttvs.OrderByDescending(ct => ct.id_cttv).First();

            LichTuVan ltv = new LichTuVan() {  email_BS = email, ntv = _ntv , email_BN = null, phongtuvan = newest_cttv.id_cttv};

            EHealthCareService.Instance.Add(ltv);
            EHealthCareService.Instance.Save();

            return RedirectToAction("Manage");
        }

        public ActionResult History()
        {
            String email = Session["email"] as String;
            List<LichTuVan> lichtuvans = EHealthCareService.Instance.getLichTuVans();
            List<LichTuVan> lichtuvanOfSession  =  lichtuvans.Where(ltv => DateTime.Compare(DateTime.Now, ltv.ntv) > 0 && String.Compare(ltv.email_BS, email) == 0 && ltv.email_BN != null).ToList();
            ViewData["lichtuvans"] = lichtuvanOfSession;

            List <ChiTietTuVan> chitietltv = EHealthCareService.Instance.getChiTietTuVans();

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


        public ActionResult FilterMission(DateTime date)
        {
            if (DateTime.Compare(date, DateTime.Now) < 0)
            {
                return RedirectToAction("Mission");
            }

            string email = Session["email"] as string;
            BacSi bacsi = EHealthCareService.Instance.getBacSiBy(bs => bs.email.CompareTo(email) == 0);
            List<LichTuVan> lichtuvan_all = EHealthCareService.Instance.getLichTuVans();

            List<LichTuVan> lichtuvan_of_bs = lichtuvan_all.Where(l => l.email_BS.CompareTo(bacsi.email) == 0 && l.ntv.Day == date.Day && l.ntv.Month == date.Month && l.ntv.Year == date.Year).ToList();

            ViewData["lichtuvans"] = lichtuvan_of_bs;

            List<ChiTietTuVan> chitiettuvan = EHealthCareService.Instance.getChiTietTuVans();

            List<ChiTietTuVanDAO> chitiettuvan_of_bacsi = new List<ChiTietTuVanDAO>();
            lichtuvan_of_bs.ForEach(ltv =>
            {
                chitiettuvan_of_bacsi.ForEach(ct =>
                {
                    if (ct.id_cttv == ltv.id_cttv)
                    {
                        ChiTietTuVanDAO _chitiettuvan = new ChiTietTuVanDAO();

                        _chitiettuvan.id_cttv = ct.id_cttv;
                        _chitiettuvan.chiDinh = ct.chiDinh;
                        _chitiettuvan.chuanDoan = ct.chuanDoan;
                        _chitiettuvan.trieuChung = ct.trieuChung;
                        _chitiettuvan.ghiChu = ct.ghiChu;
                        chitiettuvan_of_bacsi.Add(_chitiettuvan);
                    }
                });
            });

            JavaScriptSerializer json = new JavaScriptSerializer();
            ViewData["chitiettuvans"] = json.Serialize(chitiettuvan_of_bacsi);
            return View();
        }
        public ActionResult FilterHistory(DateTime date)
        {
            if(DateTime.Compare(date, DateTime.Now) > 0)
            {
                return RedirectToAction("History");
            }
            String email = Session["email"] as String;
            List<LichTuVan> lichtuvans = EHealthCareService.Instance.getLichTuVans();
            List<LichTuVan> lichtuvanOfSession = lichtuvans.Where(ltv => date.Year == ltv.ntv.Year && date.Month == ltv.ntv.Month && date.Day == ltv.ntv.Day && String.Compare(ltv.email_BS, email) == 0 && ltv.email_BN != null).ToList();
            ViewData["lichtuvans"] = lichtuvanOfSession;

            List<ChiTietTuVan> chitietltv = EHealthCareService.Instance.getChiTietTuVans();

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

        [HttpPost]
        public ActionResult Update(DateTime ntns, String hoten, String kinhnghiem, String dcbvtt, bool gioitinh, String tdcm, HttpPostedFileBase hinhanh)
        {
            string email = Session["email"] as string;
            BacSi bacsi = EHealthCareService.Instance.getBacSiBy(bs => bs.email.CompareTo(email) == 0);
            bacsi.ntns = ntns;
            bacsi.hoten = hoten;
            bacsi.kinhnghiem = kinhnghiem;
            bacsi.dc_bv_tt = dcbvtt;
            bacsi.gioitinh = gioitinh;
            bacsi.tdcm = tdcm;
            if(hinhanh != null)
            {
               // System.IO.File.Delete(Path.Combine(Server.MapPath("~/Content/images"), bacsi.hinhanh));
                String newNameImg = SetNameImg(hinhanh);
                SaveImg(hinhanh, newNameImg);
                bacsi.hinhanh = newNameImg;
            }

            EHealthCareService.Instance.UpdateBS(bacsi);
            return RedirectToAction("Index");
        }

        public ActionResult Mission()
        {
            string email = Session["email"] as string;
            BacSi bacsi = EHealthCareService.Instance.getBacSiBy(bs => bs.email.CompareTo(email) == 0);
            List<LichTuVan> lichtuvan_all = EHealthCareService.Instance.getLichTuVans();

            List<LichTuVan> lichtuvan_of_bs = lichtuvan_all.Where(l => l.email_BS.CompareTo(bacsi.email) == 0 && l.ntv >= DateTime.Now).ToList();

            ViewData["lichtuvans"] = lichtuvan_of_bs;

            List<ChiTietTuVan> chitiettuvan = EHealthCareService.Instance.getChiTietTuVans();

            List<ChiTietTuVanDAO> chitiettuvan_of_bacsi = new List<ChiTietTuVanDAO>();
            lichtuvan_of_bs.ForEach(ltv =>
           {
               chitiettuvan_of_bacsi.ForEach(ct =>
               {
                   if (ct.id_cttv == ltv.id_cttv)
                   {
                       ChiTietTuVanDAO _chitiettuvan = new ChiTietTuVanDAO();

                       _chitiettuvan.id_cttv = ct.id_cttv;
                       _chitiettuvan.chiDinh = ct.chiDinh;
                       _chitiettuvan.chuanDoan = ct.chuanDoan;
                       _chitiettuvan.trieuChung = ct.trieuChung;
                       _chitiettuvan.ghiChu = ct.ghiChu;
                       chitiettuvan_of_bacsi.Add(_chitiettuvan);
                   }
               });
           });

            JavaScriptSerializer json = new JavaScriptSerializer();
            ViewData["chitiettuvans"] = json.Serialize(chitiettuvan_of_bacsi);
            return View();
        }
    }
}