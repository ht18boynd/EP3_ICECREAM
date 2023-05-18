using EP3_ICE_CREAM.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace EP3_ICE_CREAM.Areas.Admin.Controllers
{
    public class ContactController : Controller
    {
        private EP_ICECREAMEntities db = new EP_ICECREAMEntities();
        // GET: Admin/Contact

        //View Contaxt
        public ActionResult Index()
        {
            var data = db.Contacts.ToList();
            return View(data);
        }

        public ActionResult CreateConatx()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateConatx(Contact contact)
        {
            var data = db.Contacts.Add(contact);
            db.SaveChanges();
            return RedirectToAction("");
        }

        public ActionResult DetailsContac(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }


        public ActionResult DeleteContact(int id) 
        {
            Contact contact = db.Contacts.Find(id);
            db.Contacts.Remove(contact);
            Session["Delete"] = contact.id.ToString();
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}