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

        public bool check_session()
        {
            string email = Session["email"] as string;
            if (email != null)
                return true;
            return false;
        }
        public ActionResult Index()
        {
            bool exists_session = check_session();
            if (!exists_session)
            {
                return RedirectToAction("Index", "Login");
            }
            string email = Session["email"] as string;
            BacSi bacsi = EHealthCareService.Instance.getBacSiBy(bs => bs.email.CompareTo(email) == 0);
            ViewData["bacsi"] = bacsi;
            bacsi._ct_chuyenkhoas = EHealthCareService.Instance.getChiTietChuyenKhoa(bacsi);
            List<ChuyenKhoa> chuyenkhoas = EHealthCareService.Instance.getChuyenKhoas();
            ViewData["chuyenkhoas"] = chuyenkhoas;
            List<BenhVien> benhviens = EHealthCareService.Instance.getBenhViens();
            ViewData["benhviens"] = benhviens;
            return View();
        }

        public ActionResult Manage()
        {
            bool exists_session = check_session();
            if (!exists_session)
            {
                return RedirectToAction("Index", "Login");
            }
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
            bool exists_session = check_session();
            if (!exists_session)
            {
                return RedirectToAction("Index", "Login");
            }
            DateTime _ntv = DateTime.Parse(ngay + " " + gio);

            String email_bs = (String)Session["email"];

            ChiTietTuVan cttv = new ChiTietTuVan() { ketQua = false };

            EHealthCareService.Instance.AddChiTietTuVan(cttv);
            EHealthCareService.Instance.Save();

            List<ChiTietTuVan> cttvs = EHealthCareService.Instance.getChiTietTuVans();

            ChiTietTuVan newest_cttv = cttvs.OrderByDescending(ct => ct.id_cttv).First();

            LichTuVan ltv = new LichTuVan() {  email_BS = email, ntv = _ntv , email_BN = null, id_cttv = newest_cttv.id_cttv};

            EHealthCareService.Instance.Add(ltv);
            EHealthCareService.Instance.Save();

            return RedirectToAction("Manage");
        }

        public ActionResult History()
        {
            bool exists_session = check_session();
            if (!exists_session)
            {
                return RedirectToAction("Index", "Login");
            }
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
            bool exists_session = check_session();
            if (!exists_session)
            {
                return RedirectToAction("Index", "Login");
            }
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
                    chitiettuvan.ForEach(ct =>
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
            bool exists_session = check_session();
            if (!exists_session)
            {
                return RedirectToAction("Index", "Login");
            }
            if (DateTime.Compare(date, DateTime.Now) > 0)
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
        public ActionResult Update(DateTime ntns, String kinhnghiem,HttpPostedFileBase file_info)
        {
            bool exists_session = check_session();
            if (!exists_session)
            {
                return RedirectToAction("Index", "Login");
            }
            string email_bs = Session["email"] as string;
            BacSi bs = EHealthCareService.Instance.getBacSiBy(b => b.email.CompareTo(email_bs) == 0);
            bs.ntns = ntns;
            bs.kinhnghiem = kinhnghiem;
            EHealthCareService.Instance.UpdateBS(bs);
            EHealthCareService.Instance.Save();
            if(file_info != null)
            {

                string newNameFile = NewNameFile(file_info);
                SaveFileInfo(file_info, newNameFile);

                var path = Server.MapPath(@"~/App_Data/UpdateInfo/infoUpdates.txt");

                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine(email_bs);
                }
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine(DateTime.Now);
                }
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine(newNameFile);
                }
            }
            return RedirectToAction("Index","Probs");
        }

        private string NewNameFile(HttpPostedFileBase file_info)
        {
            String name = Path.GetFileNameWithoutExtension(file_info.FileName);
            String extension = Path.GetExtension(file_info.FileName);
            return name + DateTime.Now.ToString("yyyyMMddTHHmmss") + extension;
        }
        private void SaveFileInfo(HttpPostedFileBase file_info, string name)
        {
            file_info.SaveAs(Path.Combine(Server.MapPath("~/Content/FileUpdateInfo"), name));
        }

        public ActionResult Mission()
        {
            bool exists_session = check_session();
            if (!exists_session)
            {
                return RedirectToAction("Index", "Login");
            }
            string email = Session["email"] as string;
            BacSi bacsi = EHealthCareService.Instance.getBacSiBy(bs => bs.email.CompareTo(email) == 0);
            List<LichTuVan> lichtuvan_all = EHealthCareService.Instance.getLichTuVans();
            string s_current_time = DateTime.Now.ToShortDateString() + " 00:00:00";
            DateTime d_current_time = DateTime.Parse(s_current_time);

            List<LichTuVan> lichtuvan_of_bs = lichtuvan_all.Where(l => l.email_BS.CompareTo(bacsi.email) == 0 && l.ntv >= d_current_time && l.email_BN != null).ToList();

            ViewData["lichtuvans"] = lichtuvan_of_bs;

            List<ChiTietTuVan> chitiettuvan = EHealthCareService.Instance.getChiTietTuVans();

            List<ChiTietTuVanDAO> chitiettuvan_of_bacsi = new List<ChiTietTuVanDAO>();
            lichtuvan_of_bs.ForEach(ltv =>
           {
               chitiettuvan.ForEach(ct =>
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

        public ActionResult SaveDetail(int id_cttv, string chiDinh, string chuanDoan, string trieuChung, string ghiChu)
        {
            bool exists_session = check_session();
            if (!exists_session)
            {
                return RedirectToAction("Index", "Login");
            }
            ChiTietTuVan cttv = EHealthCareService.Instance.getChiTietTuVan(id_cttv);

            cttv.chiDinh = chiDinh;
            cttv.chuanDoan = chuanDoan;
            cttv.trieuChung = trieuChung;
            cttv.ghiChu = ghiChu;
            cttv.ketQua = true;

            EHealthCareService.Instance.UpdateCTTV(cttv);
            EHealthCareService.Instance.Save();
            return RedirectToAction("Mission");
        }

        public ActionResult ReplaceCalender(string ngaycu, string giocu,string email,DateTime ngay,int gio)
        {
            bool exists_session = check_session();
            if (!exists_session)
            {
                return RedirectToAction("Index", "Login");
            }
            DateTime _ntv = DateTime.Parse(ngaycu + " " + giocu);
            List<LichTuVan> ltvs = EHealthCareService.Instance.getLichTuVans();

            LichTuVan ltv = ltvs.First(l => l.ntv == _ntv && l.email_BS.CompareTo(email.ToLower().Trim()) == 0);
            if(ltv != null)
            {
                DateTime timenew = DateTime.Parse(ngay.ToShortDateString() + " " + gio + ":00:00");
                LichTuVan ltv_new = new LichTuVan();
                ltv_new.ntv = timenew;
                ltv_new.email_BN = ltv.email_BN;
                ltv_new.email_BS = ltv.email_BS;
                ltv_new.id_cttv = ltv.id_cttv;
                ltv_new.phongtuvan = ltv.phongtuvan;
                ltv_new.trangthai = ltv.trangthai;
                
                EHealthCareService.Instance.Add(ltv_new);
                EHealthCareService.Instance.Save();

                EHealthCareService.Instance.Delete(ltv);
                EHealthCareService.Instance.Save();
                return RedirectToAction("Manage");
            }

            return RedirectToAction("Manage");
        }

        public ActionResult HuyLich(string ngay, string gio)
        {
            bool exists_session = check_session();
            if (!exists_session)
            {
                return RedirectToAction("Index", "Login");
            }
            string _s_ntv = ngay +  " " + gio;

            DateTime _d_ntv = DateTime.Parse(_s_ntv);

            string email_bs = Session["email"] as string;

            BacSi bacsi = EHealthCareService.Instance.getBacSiBy( b => b.email.CompareTo(email_bs) == 0);

            LichTuVan _ltv = EHealthCareService.Instance.getLichTuVanBy(bacsi).SingleOrDefault(l => DateTime.Compare(l.ntv,_d_ntv) == 0);

            ChiTietTuVan ct = _ltv.ChiTietTuVan;
            EHealthCareService.Instance.Delete(ct);
            EHealthCareService.Instance.Save();

            EHealthCareService.Instance.Delete(_ltv);
            EHealthCareService.Instance.Save();

            return RedirectToAction("Manage");
        }

        public ActionResult MoPhong(int id_cttv)
        {
            bool exists_session = check_session();
            if (!exists_session)
            {
                return RedirectToAction("Index", "Login");
            }
            string email = Session["email"] as string;
            BacSi bs = EHealthCareService.Instance.getBacSiBy(b => b.email.CompareTo(email)==0);

            List<LichTuVan> ltvs = EHealthCareService.Instance.getLichTuVanBy(bs);
            LichTuVan ltv = ltvs.SingleOrDefault(l => l.id_cttv == id_cttv);
            
            if (DateTime.Now.Subtract(ltv.ntv) > TimeSpan.FromHours(1))
            {
                return RedirectToAction("Mission");
            }
            if(ltv != null)
            {
                string url_room = "http://localhost:3000/index.html?user_id="+email+"&meeting_id="+ltv.phongtuvan+"&user_name="+bs.hoten;
                string file_room = Server.MapPath("~/App_Data/Data_Process/CurrentRoom.txt");
                string[] room_string = System.IO.File.ReadAllText(file_room).Split('\n');
                
                foreach(var i in room_string)
                {
                    int temp;
                    if(int.TryParse(i,out temp))
                    {
                        if (temp == ltv.phongtuvan)
                        {
                            return Redirect(url_room);
                        }
                    }
                   
                }
                System.IO.File.AppendAllText(file_room,  ltv.phongtuvan.ToString() + Environment.NewLine);

                return Redirect(url_room);
                
            }
           
            return RedirectToAction("Mission");
        }
    }
}