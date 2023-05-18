using EP3_ICE_CREAM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.IO;

namespace EP3_ICE_CREAM.Areas.Admin.Controllers
{
    public class RegisteredUserController : Controller
    {
        private EP_ICECREAMEntities db =new EP_ICECREAMEntities();
        // GET: Admin/RegisteredUser
        public ActionResult Index(int? page)
        {
            System.IO.File.Copy(Server.MapPath("/Uploads/ImgNull/null.png"), Path.Combine(Server.MapPath("/Uploads/RegisterdUsers/null.png")), true);

            var user = db.RegisteredUsers.OrderByDescending(s => s.RegisteredUser_id).ToList();
            if (page == null) page = 1;
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(user.ToPagedList(pageNumber, pageSize));
        }
    }
}