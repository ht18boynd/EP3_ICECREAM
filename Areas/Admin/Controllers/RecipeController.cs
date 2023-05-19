using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EP3_ICE_CREAM.Models;
using PagedList;
using EntityState = System.Data.Entity.EntityState;

namespace EP3_ICE_CREAM.Areas.Admin.Controllers
{
    public class RecipeController : Controller
    {
        private EP_ICECREAMEntities db = new EP_ICECREAMEntities();

        // GET: Admin/Recipe
        public ActionResult Index(int? page ,string search="" ,string SortColumn="Recipe_id" , string  IconClass="fa-sort-asc")
        {
            System.IO.File.Copy(Server.MapPath("/Uploads/ImgNull/null.png"), Path.Combine(Server.MapPath("/Uploads/Recipes/null.png")), true);

            if (search !=null)
            {
                //Search

                List<Recipe> r = db.Recipes.Where(row => row.Recipe_title.Contains(search)).ToList();
                ViewBag.Search = search;
                return View(r);
            }
            //Search
            
                List<Recipe> recipes = db.Recipes.Where(row => row.Recipe_title.Contains(search)).ToList();
                ViewBag.Search = search;
           
            
            //Page

            var recipe = db.Recipes.OrderByDescending(s => s.id).ToList();
            if (page == null) page = 1;
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            ViewBag.Flavor_id = new SelectList(db.Flavors, "Flavor_id", "Flavor_title");

            return View(recipe.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Create()
        {

            ViewBag.Flavor_id = new SelectList(db.Flavors, "Flavor_id", "Flavor_title");

            return View();
        }
        [HttpPost, ValidateInput(false)]

        [ValidateAntiForgeryToken]
        public ActionResult Create(Recipe recipe, HttpPostedFileBase img, string Recipe_title, string Flavor_id ,string Recipe_Author ,string Recipe_ingredients, string Recipe_procedure)
        {
            ViewBag.Flavor_id = new SelectList(db.Flavors, "Flavor_id", "Flavor_title", recipe.Flavor_id);

            string _path = Server.MapPath("~/Uploads/Recipes/");
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            if (img != null)
            {
                string newFileName = DateTime.Now.Millisecond.ToString() + DateTime.Now.ToString("yyyyMMddHHmmss") + "Recipes" + ".jpg";
                string path0 = Path.Combine(_path + newFileName);
                img.SaveAs(path0);
               

                recipe.Recipe_image = newFileName;
                recipe.slug = $"/" + newFileName;
            }
            else
            {
                recipe.Recipe_image = "null.jpg";
            }
            recipe.Recipe_ingredients = Recipe_ingredients;
        
            recipe.Recipe_procedure = Recipe_procedure;
            recipe.Recipe_title = Recipe_title;
            string Recipe_id = DateTime.Now.ToString("yyyyMMddHHmmss") + "recipes";
            recipe.Recipe_id = Recipe_id;
            recipe.Flavor_id = Flavor_id;
            recipe.Recipe_Author = Recipe_Author;
            if (ModelState.IsValid)
            {
                db.Recipes.Add(recipe);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
           

            Session["CreateSuccess"] = recipe.id.ToString();
            ViewBag.Flavor_id = new SelectList(db.Flavors, "Flavor_id", "Flavor_title", recipe.Flavor_id);
            return View(recipe);
        }
        [ValidateInput(false)]
        // GET: Admin/recipes/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            ViewBag.Flavor_id = new SelectList(db.Flavors, "Flavor_id", "Flavor_title", recipe.Flavor_id);
            return View(recipe);
        }

        // POST: Admin/recipes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(string Recipe_id, HttpPostedFileBase img, string Recipe_title, string Flavor_id, string Recipe_Author, string Recipe_ingredients, string Recipe_procedure)
        {

                Recipe recipe = db.Recipes.Where(s => s.Recipe_id == Recipe_id).FirstOrDefault();
                string _path = Server.MapPath("~/Uploads/Recipes/");
                if (img != null)
                {
                    string newFileName = DateTime.Now.Millisecond.ToString() + DateTime.Now.ToString("yyyyMMddHHmmss") + "recipes" + ".jpg";
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
                    if (recipe.Recipe_image != null)
                    {
                        string physicalPath = Path.Combine(Server.MapPath("~/Uploads/Recipes/"), recipe.Recipe_image);
                        if (System.IO.File.Exists(physicalPath))
                        {
                            System.IO.File.Delete(physicalPath);
                        }
                    }


                    recipe.Recipe_image = newFileName;
                    recipe.slug = $"/" + newFileName;
                }

                recipe.Recipe_ingredients = Recipe_ingredients;

                recipe.Recipe_procedure = Recipe_procedure;
                recipe.Recipe_title = Recipe_title;
                recipe.Recipe_id = Recipe_id;
                recipe.Flavor_id = Flavor_id;
                recipe.Recipe_Author = Recipe_Author;

                if (ModelState.IsValid)
                {
                    db.Entry(recipe).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                Session["UpdateSuccess"] = 1;
                ViewBag.Flavor_id = new SelectList(db.Flavors, "Flavor_id", "Flavor_title", recipe.Flavor_id);
                return RedirectToAction("Index");
            
       

        }
  


        // GET: Admin/recipes/Delete/5
        public ActionResult Delete(string id)
        {
            

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = db.Recipes.Find(id);
            ViewBag.Flavor_id = new SelectList(db.Flavors, "Flavor_id", "Flavor_title", recipe.Flavor_id);

            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
        }

        // POST: Admin/recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Recipe recipe = db.Recipes.Find(id);
            // Delete old Image Upload
            if (recipe.Recipe_image != null)
            {
                string physicalPath = Path.Combine(Server.MapPath("~/Uploads/Recipes/"), recipe.Recipe_image);
                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }
            }
            ViewBag.Flavor_id = new SelectList(db.Flavors, "Flavor_id", "Flavor_title", recipe.Flavor_id);

            db.Recipes.Remove(recipe);
            db.SaveChanges();
            Session["DeleteSuccess"] = 1;
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
