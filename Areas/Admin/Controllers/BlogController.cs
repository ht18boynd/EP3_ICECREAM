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
    public class BlogController : Controller
    {
        private EP_ICECREAMEntities db = new EP_ICECREAMEntities();
        // GET: Admin/Blog
        public ActionResult Index(int? page)
        {
            ViewBag.id = new SelectList(db.Blogs, "id");
            System.IO.File.Copy(Server.MapPath("/Uploads/ImgNull/null.png"), Path.Combine(Server.MapPath("/Uploads/Blogs/null.png")), true);

            var blog = db.Blogs.OrderByDescending(s => s.id).ToList();
            if (page == null) page = 1;
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(blog.ToPagedList(pageNumber, pageSize));
        }

        //Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Blog blog, HttpPostedFileBase img, string title, string slug)
        {
            ViewBag.id = new SelectList(db.Blogs, "id");

            string _path = Server.MapPath("~/Uploads/Blogs/");
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            if (img != null)
            {
                string newFileName = DateTime.Now.Millisecond.ToString() + DateTime.Now.ToString("yyyyMMddHHmmss") + "blogs" + ".jpg";
                string path0 = Path.Combine(_path + newFileName);
                img.SaveAs(path0);


                blog.image = newFileName;
            }
            else
            {
                blog.image = "null.jpg";
            }

            blog.title = title;
            blog.slug = slug;

            if (ModelState.IsValid)
            {
                db.Blogs.Add(blog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            Session["Create"] = blog.id.ToString();
            ViewBag.id = new SelectList(db.Blogs, "id");
            return View(blog);
        }


        //Update
        // GET: Admin/Flavors/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // POST: Admin/Flavors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "title,image,slug")] Blog blog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(blog).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blog);
        }
        public ActionResult EditPost(int id, HttpPostedFileBase img, string title, string slug)
        {

            if (id != null)
            {
                Blog blog = db.Blogs.Where(s => s.id == id).FirstOrDefault();
                string _path = Server.MapPath("~/Uploads/Blogs/");
                if (img != null)
                {
                    string newFileName = DateTime.Now.Millisecond.ToString() + DateTime.Now.ToString("yyyyMMddHHmmss") + "Blogs" + ".jpg";
                    string path0 = Path.Combine(_path + newFileName);

                    img.SaveAs(path0);
                    // Delete old Image Upload
                    if (blog.image != null)
                    {
                        string physicalPath = Path.Combine(Server.MapPath("~/Uploads/Blogs/"), blog.image);
                        if (System.IO.File.Exists(physicalPath))
                        {
                            System.IO.File.Delete(physicalPath);
                        }
                    }


                    blog.image = newFileName;
                }

                blog.title = title;
                blog.slug = slug;

                if (ModelState.IsValid)
                {
                    db.Entry(blog).State = System.Data.Entity.EntityState.Modified;
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


        //Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }


        //Delete
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // POST: Admin/Flavors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Blog blog = db.Blogs.Find(id);
            string physicalPath = "";
            // Delete old Image Upload
            if (blog.image == null || blog.title == "")
            {
                physicalPath = Path.Combine(Server.MapPath("~/Uploads/Blogs/null.png"));
            }
            else
            {
                physicalPath = Path.Combine(Server.MapPath("~/Uploads/Blogs/"), blog.image);
            }


            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }
            db.Blogs.Remove(blog);
            Session["Delete"] = blog.id.ToString();
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