using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using PagedList;
using EP3_ICE_CREAM.Models;

namespace EP3_ICE_CREAM.Areas.Admin.Controllers
{
    public class BannersController : Controller
    {
        private EP_ICECREAMEntities db = new EP_ICECREAMEntities();

        // GET: Admin/Banners
        public ActionResult Index(int? page)
        {
            if (Session["Login"] != null)
            {

                System.IO.File.Copy(Server.MapPath("/Uploads/ImgNull/null.png"), Path.Combine(Server.MapPath("/Uploads/Banners/null.png")), true);

                var banner = db.Banners.OrderByDescending(s => s.id).ToList();
                if (page == null) page = 1;
                int pageSize = 5;
                int pageNumber = (page ?? 1);
                return View(banner.ToPagedList(pageNumber, pageSize));
            }
            else { 
                return RedirectToAction("LoginAdmin", "Admin"); 
            }

           
        }

        // GET: Admin/Banners/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["Login"] != null)
            {
                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Banner banner = db.Banners.Find(id);
            if (banner == null)
            {
                return HttpNotFound();
            }
            return View(banner);
            }
            else { return RedirectToAction("LoginAdmin","Admin"); }
        }

        // GET: Admin/Banners/Create
      

        // POST: Admin/Banners/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Banner banner, HttpPostedFileBase img, string Banner_name)
        {
            if (Session["Login"] != null)
            {

                string _path = Server.MapPath("~/Uploads/Banners/");
                if (!Directory.Exists(_path))
                {
                    Directory.CreateDirectory(_path);
                }
                if (img != null)
                {
                    string newFileName = DateTime.Now.Millisecond.ToString() + DateTime.Now.ToString("yyyyMMddHHmmss") + "Danhmuc" + ".jpg";
                    string path0 = Path.Combine(_path + newFileName);
                    img.SaveAs(path0);
                    banner.Banner_image = newFileName;
                    banner.slug = $"/" + newFileName;
                }
                else
                {
                    banner.Banner_image = "null.jpg";
                }
                banner.Banner_name = Banner_name;

                db.Banners.Add(banner);
                db.SaveChanges();

                Session["Create"] = banner.id.ToString();

                return RedirectToAction("Index");
            }
            else { return RedirectToAction("LoginAdmin", "Admin"); }
            }

        // GET: Admin/Banners/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["Login"] != null)
            {
                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Banner banner = db.Banners.Find(id);
            if (banner == null)
            {
                return HttpNotFound();
            }
            return View(banner);
            }
            else { return RedirectToAction("LoginAdmin", "Admin"); }
        }

        // POST: Admin/Banners/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Banner_name,Banner_image,slug")] Banner banner)
        {
            if (ModelState.IsValid)
            {
                db.Entry(banner).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(banner);
        }
        public ActionResult EditPost(int id, HttpPostedFileBase img, string Banner_name)
        {

            if (id != null)
            {
                Banner banner = db.Banners.Where(s => s.id == id).FirstOrDefault();
                string _path = Server.MapPath("~/Uploads/Banners/");
                if (img != null)
                {
                    string newFileName = DateTime.Now.Millisecond.ToString() + DateTime.Now.ToString("yyyyMMddHHmmss") + "Banners" + ".jpg";
                    string path0 = Path.Combine(_path + newFileName);
                    
                    img.SaveAs(path0);
                    // Delete old Image Upload
                    if (banner.Banner_image != null)
                    {
                        string physicalPath = Path.Combine(Server.MapPath("~/Uploads/Banners/"), banner.Banner_image);
                        if (System.IO.File.Exists(physicalPath))
                        {
                            System.IO.File.Delete(physicalPath);
                        }
                    }


                    banner.Banner_image = newFileName;
                    banner.slug = $"/" + newFileName;
                }

                banner.Banner_name = Banner_name;
               
               
              

                if (ModelState.IsValid)
                {
                    db.Entry(banner).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    Session["Success"] = 1;
                    return RedirectToAction("Index");
                }
                Session["Success"] = 1;
                return RedirectToAction("Index");
            }
            else
            {
                Session["Error"] = 1;
                return View();
            }

        }


        // GET: Admin/Banners/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["Login"] != null)
            {
                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Banner banner = db.Banners.Find(id);
            if (banner == null)
            {
                return HttpNotFound();
            }
            return View(banner);
            }
            else { return RedirectToAction("LoginAdmin", "Admin"); }
        }

        // POST: Admin/Banners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Banner banner = db.Banners.Find(id);
            string physicalPath = "";
            // Delete old Image Upload
            if (banner.Banner_image == null || banner.Banner_name == "")
            {
                physicalPath = Path.Combine(Server.MapPath("~/Uploads/Banners/null.png"));
            }
            else
            {
                physicalPath = Path.Combine(Server.MapPath("~/Uploads/Banners/"), banner.Banner_image);
            }


            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }
            db.Banners.Remove(banner);
            Session["Delete"] = banner.id.ToString();
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
