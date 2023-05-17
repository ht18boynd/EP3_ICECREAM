using EP3_ICE_CREAM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EP3_ICE_CREAM.Areas.Admin.Controllers
{
    public class PlavorController : Controller
    {
        private EP_ICECREAMEntities db = new EP_ICECREAMEntities(); 
        // GET: Admin/Plavor
        public ActionResult Index()
        {
            var data = db.Flavors.ToList();
            return View(data);
        }

        //Crate
        public ActionResult CrateFlavors()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrateFlavors(Flavor flavor)
        {

            return View();
        }
    }
}