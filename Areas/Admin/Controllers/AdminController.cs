using EP3_ICE_CREAM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EP3_ICE_CREAM.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        private EP_ICECREAMEntities db = new EP_ICECREAMEntities();

        // GET: Admin/Admin
        public ActionResult Index()
        {
            //Phân Quyền Login

            if (Session["Login"] == null) 
            {
                return RedirectToAction("LoginAdmin");
            }
            return View();
        }

        public ActionResult LoginAdmin()
        {
            return View();
        }

        public JsonResult CheckLoginAdmin(string username, string password)
        {
            if (username != "" || password != "")
            {
                var ad_t = db.Admins.Where(s => s.username == username && s.password == password).FirstOrDefault();
                if (ad_t != null)
                {
                    Session["Login"] = ad_t.id.ToString();
                    return Json(new { code = 200, url = Url.Action("Index", "Admin"), msg = "Thanh công", }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 500, msg = "UserName or Password Error !!" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { code = 500, msg = "UserName or Password not bank !" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Logout()
        {
            Session["Logout"] = 1;
            return RedirectToAction("LoginAdmin");
        }
    }
}