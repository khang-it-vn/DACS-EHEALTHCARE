using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EHealthCare_WebApp.Controllers
{
    public class HomeController : Controller
    {
        public bool check_session()
        {
            string email = Session["email"] as string;
            if (email != null)
                return true;
            return false;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}