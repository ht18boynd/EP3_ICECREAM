using EP3_ICE_CREAM.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace EP3_ICE_CREAM.Areas.Admin.Controllers
{
    public class FlavorController : Controller
    {
        private EP_ICECREAMEntities db = new EP_ICECREAMEntities();
        // GET: Admin/Flavor
        public ActionResult Index(int? page)
        {


            System.IO.File.Copy(Server.MapPath("/Uploads/ImgNull/null.png"), Path.Combine(Server.MapPath("/Uploads/Flavors/null.png")), true);

            var flavor = db.Flavors.OrderByDescending(s => s.id).ToList();
            if (page == null) page = 1;
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(flavor.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Flavor flavor = db.Flavors.Find(id);
            if (flavor == null)
            {
                return HttpNotFound();
            }
            return View(flavor);
        }

        // GET: Admin/Flavors/Create


        // POST: Admin/Flavors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Flavor flavor, HttpPostedFileBase img, string Flavor_title ,string Flavor_id)
        {
            var data = db.Flavors.Find(Flavor_id);
            if (data == null)
            {
                string _path = Server.MapPath("~/Uploads/Flavors/");
                if (!Directory.Exists(_path))
                {
                    Directory.CreateDirectory(_path);
                }
                if (img != null)
                {
                    string newFileName = DateTime.Now.Millisecond.ToString() + DateTime.Now.ToString("yyyyMMddHHmmss") + "Flavor" + ".jpg";
                    string path0 = Path.Combine(_path + newFileName);
                    img.SaveAs(path0);
                    flavor.Flavor_image = newFileName;
                    flavor.slug = $"/" + newFileName;
                }
                else
                {
                    flavor.Flavor_image = "null.jpg";
                }
                flavor.Flavor_title = Flavor_title;
                flavor.Flavor_id = Flavor_id;

                db.Flavors.Add(flavor);
                db.SaveChanges();

                Session["Create"] = flavor.id.ToString();

                return RedirectToAction("Index");
            }
            Session["ErrorCreate"] = 1;
            return RedirectToAction("Index");


        }

        // GET: Admin/Flavors/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Flavor flavor = db.Flavors.Find(id);
            if (flavor == null)
            {
                return HttpNotFound();
            }
            return View(flavor);
        }

        // POST: Admin/Flavors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Flavor_id,Flavor_title,Flavor_image,slug")] Flavor flavor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(flavor).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(flavor);
        }
        public ActionResult EditPost(int id, HttpPostedFileBase img, string Flavor_title ,string Flavor_id)
        {

            if (id != null)
            {
                Flavor flavor = db.Flavors.Where(s => s.id == id).FirstOrDefault();
                string _path = Server.MapPath("~/Uploads/Flavors/");
                if (img != null)
                {
                    string newFileName = DateTime.Now.Millisecond.ToString() + DateTime.Now.ToString("yyyyMMddHHmmss") + "Flavors" + ".jpg";
                    string path0 = Path.Combine(_path + newFileName);

                    img.SaveAs(path0);
                    // Delete old Image Upload
                    if (flavor.Flavor_image != null)
                    {
                        string physicalPath = Path.Combine(Server.MapPath("~/Uploads/Flavors/"), flavor.Flavor_image);
                        if (System.IO.File.Exists(physicalPath))
                        {
                            System.IO.File.Delete(physicalPath);
                        }
                    }


                    flavor.Flavor_image = newFileName;
                    flavor.slug = $"/" + newFileName;
                }

                flavor.Flavor_title = Flavor_title;
                flavor.Flavor_id = Flavor_id;






                if (ModelState.IsValid)
                {
                    db.Entry(flavor).State = System.Data.Entity.EntityState.Modified;
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


        // GET: Admin/Flavors/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Flavor flavor = db.Flavors.Find(id);
            if (flavor == null)
            {
                return HttpNotFound();
            }
            return View(flavor);
        }

        // POST: Admin/Flavors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Flavor flavor = db.Flavors.Find(id);
            string physicalPath = "";
            // Delete old Image Upload
            if (flavor.Flavor_image == null || flavor.Flavor_title== "")
            {
                physicalPath = Path.Combine(Server.MapPath("~/Uploads/Flavors/null.png"));
            }
            else
            {
                physicalPath = Path.Combine(Server.MapPath("~/Uploads/Flavors/"), flavor.Flavor_image);
            }


            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }
            db.Flavors.Remove(flavor);
            Session["Delete"] = flavor.id.ToString();
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
