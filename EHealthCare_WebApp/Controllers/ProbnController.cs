using DbEHealthcare.Entities;
using EHealthCare_WebApp.Models;
using EHealthCare_WebApp.Utils;
using MoMo;
using Newtonsoft.Json.Linq;
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
            bacsis.ForEach(bs =>
            {
                bs._ct_chuyenkhoas = EHealthCareService.Instance.getChiTietChuyenKhoa(bs);
            });
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
                // doan nay xu ly thanh toan
                string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
                string partnerCode = "MOMO5RGX20191128";
                string accessKey = "M8brj9K6E22vXoDB";
                string serectkey = "nqQiVSgDMy809JoPF6OzP5OdBUB550Y4";
                string orderInfo = "test thanh toan";
                string redirectUrl = "https://webhook.site/b3088a6a-2d17-4f8d-a383-71389a6c600b";
                string ipnUrl = "https://webhook.site/b3088a6a-2d17-4f8d-a383-71389a6c600b";
                string requestType = "captureWallet";

                string amount = "20000";
                string orderId = Guid.NewGuid().ToString();
                string requestId = Guid.NewGuid().ToString();
                string extraData = "";

                //Before sign HMAC SHA256 signature
                string rawHash = "accessKey=" + accessKey +
                    "&amount=" + amount +
                    "&extraData=" + extraData +
                    "&ipnUrl=" + ipnUrl +
                    "&orderId=" + orderId +
                    "&orderInfo=" + orderInfo +
                    "&partnerCode=" + partnerCode +
                    "&redirectUrl=" + redirectUrl +
                    "&requestId=" + requestId +
                    "&requestType=" + requestType
                    ;

             

                MoMoSecurity crypto = new MoMoSecurity();
                //sign signature SHA256
                string signature = crypto.signSHA256(rawHash, serectkey);
              

                //build body json request
                JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "partnerName", "Test" },
                { "storeId", "MomoTestStore" },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderId },
                { "orderInfo", orderInfo },
                { "redirectUrl", redirectUrl },
                { "ipnUrl", ipnUrl },
                { "lang", "en" },
                { "extraData", extraData },
                { "requestType", requestType },
                { "signature", signature }

            };
               
                string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

                JObject jmessage = JObject.Parse(responseFromMomo);
                
                System.Diagnostics.Process.Start(jmessage.GetValue("payUrl").ToString());

                //

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

                string FilePath = Path.Combine(Server.MapPath("~/Utils/TemplateMail"), "MailDangKyThanhCong.html");
                StreamReader str = new StreamReader(FilePath);
                string MailText = str.ReadToEnd();
                str.Close();
                string body = MailText;
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
            List<BacSi> bacsis_filter = new List<BacSi>();
            List<ChiTietChuyenKhoa> _ct_chuyenkhoa = EHealthCareService.Instance.getChiTietChuyenKhoa();
            _ct_chuyenkhoa.ForEach( ct => 
            {
                if(ct.ma_CK == chuyenkhoa)
                {
                    BacSi _bs = bacsis.SingleOrDefault(bs => bs.email.CompareTo(ct.email_BS) == 0);

                    bacsis_filter.Add(_bs);
                }
            });

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
            List<BacSi> bacsis = EHealthCareService.Instance.getBacSis();
            bacsis.ForEach(bs =>
            {
                bs._ct_chuyenkhoas = EHealthCareService.Instance.getChiTietChuyenKhoa(bs);
            });
            ViewData["bacsis"] = bacsis;
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
            List<BacSi> bacsis = EHealthCareService.Instance.getBacSis();
            bacsis.ForEach(bs =>
            {
                bs._ct_chuyenkhoas = EHealthCareService.Instance.getChiTietChuyenKhoa(bs);
            });
            ViewData["bacsis"] = bacsis;

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