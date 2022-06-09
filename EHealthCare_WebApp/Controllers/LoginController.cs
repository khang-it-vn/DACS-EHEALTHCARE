using DbEHealthcare.Entities;
using Services;
using System;
using System.Collections.Generic;
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
    }
}