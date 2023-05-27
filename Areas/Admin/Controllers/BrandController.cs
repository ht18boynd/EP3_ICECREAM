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
    public class BrandController : Controller
    {
        private EP_ICECREAMEntities db = new EP_ICECREAMEntities();

        // GET: Admin/Brands
        public ActionResult Index(int? page)
        {
            if (Session["Login"] != null)
            {

                System.IO.File.Copy(Server.MapPath("/Uploads/ImgNull/null.png"), Path.Combine(Server.MapPath("/Uploads/Brands/null.png")), true);

                var brand = db.Brands.OrderByDescending(s => s.id).ToList();
                if (page == null) page = 1;
                int pageSize = 5;
                int pageNumber = (page ?? 1);
                return View(brand.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return RedirectToAction("LoginAdmin", "Admin");
            }


        }

        // GET: Admin/brands/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["Login"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Brand brand = db.Brands.Find(id);
                if (brand == null)
                {
                    return HttpNotFound();
                }
                return View(brand);
            }
            else { return RedirectToAction("LoginAdmin", "Admin"); }
        }

        // GET: Admin/brands/Create


        // POST: Admin/brands/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Brand brand, HttpPostedFileBase img, string brand_Name)
        {
            if (Session["Login"] != null)
            {

                string _path = Server.MapPath("~/Uploads/Brands/");
                if (!Directory.Exists(_path))
                {
                    Directory.CreateDirectory(_path);
                }
                if (img != null)
                {
                    string newFileName = DateTime.Now.Millisecond.ToString() + DateTime.Now.ToString("yyyyMMddHHmmss") + "Brands" + ".jpg";
                    string path0 = Path.Combine(_path + newFileName);
                    img.SaveAs(path0);
                    brand.image = newFileName;
                }
                else
                {
                    brand.image = "null.jpg";
                }
                brand.brand_Name = brand_Name;

                db.Brands.Add(brand);
                db.SaveChanges();

                Session["Create"] = brand.id.ToString();

                return RedirectToAction("Index");
            }
            else { return RedirectToAction("LoginAdmin", "Admin"); }
        }

        // GET: Admin/Brand/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["Login"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Brand brand = db.Brands.Find(id);
                if (brand == null)
                {
                    return HttpNotFound();
                }
                return View(brand);
            }
            else { return RedirectToAction("LoginAdmin", "Admin"); }
        }

        // POST: Admin/brands/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,brand_Name,image")] Brand brand)
        {
            if (ModelState.IsValid)
            {
                db.Entry(brand).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(brand);
        }
        public ActionResult EditPost(int id, HttpPostedFileBase img, string brand_Name)
        {

            if (id != null)
            {
                Brand brand = db.Brands.Where(s => s.id == id).FirstOrDefault();
                string _path = Server.MapPath("~/Uploads/Brands/");
                if (img != null)
                {
                    string newFileName = DateTime.Now.Millisecond.ToString() + DateTime.Now.ToString("yyyyMMddHHmmss") + "brands" + ".jpg";
                    string path0 = Path.Combine(_path + newFileName);

                    img.SaveAs(path0);
                    // Delete old Image Upload
                    if (brand.image != null)
                    {
                        string physicalPath = Path.Combine(Server.MapPath("~/Uploads/Brands/"), brand.image);
                        if (System.IO.File.Exists(physicalPath))
                        {
                            System.IO.File.Delete(physicalPath);
                        }
                    }


                    brand.image = newFileName;
                }

                brand.brand_Name = brand_Name;




                if (ModelState.IsValid)
                {
                    db.Entry(brand).State = System.Data.Entity.EntityState.Modified;
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


        // GET: Admin/brands/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["Login"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Brand brand = db.Brands.Find(id);
                if (brand == null)
                {
                    return HttpNotFound();
                }
                return View(brand);
            }
            else { return RedirectToAction("LoginAdmin", "Admin"); }
        }

        // POST: Admin/brands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
           Brand brand = db.Brands.Find(id);
            string physicalPath = "";
            // Delete old Image Upload
            if (brand.image == null || brand.brand_Name == "")
            {
                physicalPath = Path.Combine(Server.MapPath("~/Uploads/Brands/null.png"));
            }
            else
            {
                physicalPath = Path.Combine(Server.MapPath("~/Uploads/Brands/"), brand.image);
            }


            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }
            db.Brands.Remove(brand);
            Session["Delete"] = brand.id.ToString();
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
