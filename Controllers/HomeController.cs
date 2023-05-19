using EP3_ICE_CREAM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace EP3_ICE_CREAM.Controllers
{
    public class HomeController : Controller
    {
        private EP_ICECREAMEntities db = new EP_ICECREAMEntities();
        public ActionResult Index()
        {
            ViewBag.banner = db.Banners.OrderByDescending(s => s.id).Take(3).ToList();
            ViewBag.sanphambanchay = db.Books.OrderByDescending(s => s.quantity_sold).Take(12).ToList();
            ViewBag.sanphammoinhat = db.Books.OrderByDescending(s => s.created).Take(12).ToList();
            ViewBag.giamgiathapnhat = db.Books.OrderByDescending(s => s.discount).Take(3).ToList();
            return View(db.Books.ToList().Take(12));
        }

        public ActionResult Login()
           
        {
            return View();
        }

        public JsonResult CheckLogin(string email, string password)
        {
            if (email != "" || password != "")
            {
                var user_t = db.RegisteredUsers.Where(s => s.email == email && s.password == password).FirstOrDefault();
                if (user_t != null)
                {
                    Session["Login"] = user_t.RegisteredUser_id.ToString();
                    Session["user"] = u_temp("null.jpg", user_t.fullName, email, user_t.address, user_t.phone, "1");
                    return Json(new { code = 200, url = Url.Action("Index", "Home"), msg = "Thanh công", _user = user_t }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 500, msg = "Email or Password Error !!" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { code = 500, msg = "Email or Password not bank !" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Logout()
        {
            Session.Remove("user");
            Session["Logout"] = 1;
            return RedirectToAction("Login");
        }

        public ActionResult Register()
        {
           return View();
        }
        [HttpPost]
        public ActionResult Register(RegisteredUser user , string email)
        {
            if ((email != ""))
            {
                var user_t = db.RegisteredUsers.Where(s => s.email == user.email).SingleOrDefault();
                if (user_t == null)
                {
                    user.created = DateTime.Now;
                    db.RegisteredUsers.Add(user);
                    db.SaveChanges();
                    Session["Create"] = 1;
                    return RedirectToAction("Login");

                }
                Session["ErrorCreate"] = 1;
                return View("Register");
            }
            
            Session["ErrorCreate"] = 1;
            return View("Register");
            
        }
        public user_temp u_temp(string _avatar, string _username, string _gmail, string _address, string _phone, string _checked)
        {
            user_temp tempp = new user_temp();
            tempp._avatar = _avatar;
            tempp._username = _username;
            tempp._gmail = _gmail;
            tempp._address = _address;
            tempp._phone = _phone;
            tempp._checked = _checked;
            return tempp;
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
       public ActionResult Recipe()
        {
            ViewBag.recipe = db.Recipes.OrderByDescending(s => s.id).Take(3).ToList();

        }
    }
}