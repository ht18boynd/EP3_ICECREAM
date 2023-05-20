using EP3_ICE_CREAM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using PagedList;
using System.Net;
using System.Web.UI.WebControls.WebParts;

namespace EP3_ICE_CREAM.Controllers
{
    public class HomeController : Controller
    {
        private EP_ICECREAMEntities db = new EP_ICECREAMEntities();
        public ActionResult Index()
        {
            ViewBag.banner = db.Banners.OrderByDescending(s => s.id).Take(3).ToList();
            ViewBag.flavor =db.Flavors.OrderByDescending(s => s.id).Take(8).ToList();
            ViewBag.sold = db.Books.OrderByDescending(s => s.quantity_sold).Take(12).ToList();
            ViewBag.newbook = db.Books.OrderByDescending(s => s.created).Take(7).ToList();
            ViewBag.discountmax = db.Books.OrderByDescending(s => s.discount).Take(3).ToList();
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
        public ActionResult Shop(int? page)
        {

            ViewBag.Flavor_id = new SelectList(db.Flavors, "Flavor_id", "Flavor_title");



            var book = db.Books.OrderByDescending(s => s.id).ToList();
            if (page == null) page = 1;
            int pageSize = 7;
            int pageNumber = (page ?? 1);
            return View(book.ToPagedList(pageNumber, pageSize));

        }
        public ActionResult Books_Details(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var item = db.Books.Find(id);
                //Sẩn phẩm cùng loại
                ViewBag.booksample = db.Books.Where(s => s.Flavor_id == item.Flavor_id).ToList().Take(4);
                ViewBag.sanphamkhacloai = db.Books.Where(s => s.Flavor_id != item.Flavor_id).ToList().Take(4);
                ViewBag.Flavor_id = new SelectList(db.Flavors, "Flavor_id", "Flavor_title");

                return View(item);
            }

        }
        // Mua sản phẩm trong details
        [HttpPost]
        public JsonResult Buy_book_Details(string id, string soluong)
        {
            try
            {
                if (Session["cart"] == null) // Nếu giỏ hàng chưa được khởi tạo
                {
                    Session["cart"] = new List<EP3_ICE_CREAM.Models.Cart>();  // Khởi tạo Session["giohang"] là 1 List<Giohang>
                }
                if (id != null || soluong != null)
                {
                    var book_item = db.Books.Find(id);
                    if (book_item != null)
                    {
                        List<EP3_ICE_CREAM.Models.Cart> cart = Session["cart"] as List<EP3_ICE_CREAM.Models.Cart>;
                        if (cart.Where(s => s._book_id == id).FirstOrDefault() == null) // ko co sp nay trong gio hang
                        {
                            Book sanpham = db.Books.Find(id);  // tim sp theo sanPhamID

                            EP3_ICE_CREAM.Models.Cart newItem = new EP3_ICE_CREAM.Models.Cart()
                            {
                                _book_id = id.ToString(),
                                _book_name = sanpham.titleBook,

                                _quantity_max = (int)sanpham.quantity,
                                _image_main = sanpham.bookImage,
                                _price = (int)sanpham.priceBook,
                                _quantity = int.Parse(soluong)

                            };  // Tạo ra 1 CartItem mới

                            cart.Add(newItem);  // Thêm CartItem vào giỏ 
                        }
                        else
                        {
                            // Nếu sản phẩm khách chọn đã có trong giỏ hàng thì không thêm vào giỏ nữa mà tăng số lượng lên.
                            EP3_ICE_CREAM.Models.Cart cardItem = cart.FirstOrDefault(m => m._book_id == id);
                            cardItem._quantity += int.Parse(soluong);
                        }
                    }
                    return Json(new { code = 200, url = Url.Action("ViewCartShopping", "Home"), msg = "Mua thành công" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { code = 500, msg = "Không thành công" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Mua lỗi" + ex }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ViewCartShopping()//Hiển trị giỏ hàng
        {
            // Xóa Session["cart_temp"]
            Session["cart_temp"] = null;
            List<EP3_ICE_CREAM.Models.Cart> Cart = Session["cart"] as List<EP3_ICE_CREAM.Models.Cart>;
            ViewBag.Cart = Cart;
            return View();
        }
        public ActionResult Recipe(int? page)
        {

            ViewBag.Flavor_id = new SelectList(db.Flavors, "Flavor_id", "Flavor_title");



            var recipe = db.Recipes.OrderByDescending(s => s.id).ToList();
            if (page == null) page = 1;
            int pageSize = 7;
            int pageNumber = (page ?? 1);
            return View(recipe.ToPagedList(pageNumber, pageSize));

        }
        public ActionResult RecipeDetails(string id)
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
            
            return View(recipe);
        }
        public ActionResult About()
        {
            ViewBag.blog = db.Blogs.ToList();
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    
    }
}