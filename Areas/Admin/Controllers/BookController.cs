using EP3_ICE_CREAM.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace EP3_ICE_CREAM.Areas.Admin.Controllers
{
    public class BookController : Controller
    {
        private EP_ICECREAMEntities db = new EP_ICECREAMEntities();
        // GET: Admin/Book
        public ActionResult Index(int? page)
        {
            ViewBag.Flavor_id = new SelectList(db.Flavors, "Flavor_id", "Flavor_title");


            System.IO.File.Copy(Server.MapPath("/Uploads/ImgNull/null.png"), Path.Combine(Server.MapPath("/Uploads/Books/null.png")), true);

            var book = db.Books.OrderByDescending(s => s.id).ToList();
            if (page == null) page = 1;
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(book.ToPagedList(pageNumber, pageSize));
        }
        
        public ActionResult Create()
        {

            ViewBag.Flavor_id = new SelectList(db.Flavors, "Flavor_id", "Flavor_title");

            return View();
        }
        [HttpPost]
        

        [ValidateAntiForgeryToken]
        public ActionResult Create(Book book, HttpPostedFileBase img, string titleBook, string content, string Flavor_id, string author, int priceBook,int discount, int quantity,int quantity_sold)
        {
            ViewBag.Flavor_id = new SelectList(db.Flavors, "Flavor_id", "Flavor_title", book.Flavor_id);

            string _path = Server.MapPath("~/Uploads/Books/");
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            if (img != null)
            {
                string newFileName = DateTime.Now.Millisecond.ToString() + DateTime.Now.ToString("yyyyMMddHHmmss") + "Books" + ".jpg";
                string path0 = Path.Combine(_path + newFileName);
                img.SaveAs(path0);


                book.bookImage = newFileName;
                book.slug = $"/" + newFileName;
            }
            else
            {
                book.bookImage = "null.jpg";
            }
            book.content = content;         
            book.author = author;
            string Book_id = DateTime.Now.ToString("yyyyMMddHHmmss") + "recipes";
            book.Book_id = Book_id;
            book.Flavor_id = Flavor_id;
            book.titleBook = titleBook;
            book.quantity = quantity;
            book.discount = discount;
            book.quantity_sold = quantity_sold;
            book.priceBook = priceBook;
            book.status = 1;
            book.created = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Books.Add(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            Session["Create"] = book.id.ToString();
            ViewBag.Flavor_id = new SelectList(db.Flavors, "Flavor_id", "Flavor_title", book.Flavor_id);
            return View(book);
        }
        [ValidateInput(false)]
        // GET: Admin/recipes/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            ViewBag.Flavor_id = new SelectList(db.Flavors, "Flavor_id", "Flavor_title", book.Flavor_id);
            return View(book);
        }

        // POST: Admin/recipes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(string book_id, HttpPostedFileBase img, string titleBook, string content, string Flavor_id, string author, int priceBook, int discount, int quantity, int quantity_sold)
        {

            if (book_id != null)
            {
                Book book = db.Books.Where(s => s.Book_id == book_id).FirstOrDefault();
                string _path = Server.MapPath("~/Uploads/Books/");
                if (img != null)
                {
                    string newFileName = DateTime.Now.Millisecond.ToString() + DateTime.Now.ToString("yyyyMMddHHmmss") + "Books" + ".jpg";
                    string path0 = Path.Combine(_path + newFileName);
                    // resize
                    //WebImage new_img = new WebImage(img.InputStream);
                    //if (new_img.Width > 500 || new_img.Height > 280)
                    //{
                    //    new_img.Resize(500, 280);
                    //}
                    //new_img.Save(path0);
                    img.SaveAs(path0);
                    // Delete old Image Upload
                    if (book.bookImage != null)
                    {
                        string physicalPath = Path.Combine(Server.MapPath("~/Uploads/Books/"), book.bookImage);
                        if (System.IO.File.Exists(physicalPath))
                        {
                            System.IO.File.Delete(physicalPath);
                        }
                    }


                    book.bookImage = newFileName;
                    book.slug = $"/" + newFileName;
                }

                book.content = content;
                book.author = author;
                
                book.Book_id = book_id;
                book.Flavor_id = Flavor_id;
                book.titleBook = titleBook;
                book.quantity = quantity;
                book.discount = discount;
                book.quantity_sold = quantity_sold;
                book.priceBook = priceBook;
                book.status = 1;
                book.created = DateTime.Now;

                if (ModelState.IsValid)
                {
                    db.Entry(book).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.Flavor_id = new SelectList(db.Flavors, "Flavor_id", "Flavor_title", book.Flavor_id);
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }

        }
        public ActionResult Delete(string id)
        {


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.Books.Find(id);
            ViewBag.Flavor_id = new SelectList(db.Flavors, "Flavor_id", "Flavor_title", book.Flavor_id);

            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Admin/recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Book book = db.Books.Find(id);
            // Delete old Image Upload
            if (book.bookImage != null)
            {
                string physicalPath = Path.Combine(Server.MapPath("~/Uploads/Books/"), book.bookImage);
                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }
            }
            ViewBag.Flavor_id = new SelectList(db.Flavors, "Flavor_id", "Flavor_title", book.Flavor_id);

            db.Books.Remove(book);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}