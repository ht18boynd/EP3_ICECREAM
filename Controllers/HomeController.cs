using EP3_ICE_CREAM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using PagedList;
using System.Net;
using System.Net.Mail;


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
            ViewBag.sold = db.Books.OrderByDescending(s => s.quantity_sold).Take(3).ToList();
            ViewBag.newbook = db.Books.OrderByDescending(s => s.created).Take(7).ToList();
            ViewBag.discountmax = db.Books.OrderByDescending(s => s.discount).Take(3).ToList();

            ViewBag.member = db.Blogs.ToList();
            ViewBag.brand = db.Brands.ToList();

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
        public ActionResult Shop(int? page ,string searchString)
        {

            ViewBag.Flavor_id = new SelectList(db.Flavors, "Flavor_id", "Flavor_title");           
                var book = db.Books.OrderByDescending(s => s.id).ToList();
            ViewData["CurrentFilter"] = searchString;
            
            if (page == null) page = 1;
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            if (!String.IsNullOrEmpty(searchString))
            {
              var  books = db.Books.Where(b => b.titleBook.Contains(searchString)).ToList();
               
                return View(books.ToPagedList(pageNumber, pageSize));
            }
               
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
                            Session["AddtoCart"] = 1;
                        }
                        else
                        {
                            // Nếu sản phẩm khách chọn đã có trong giỏ hàng thì không thêm vào giỏ nữa mà tăng số lượng lên.
                            EP3_ICE_CREAM.Models.Cart cardItem = cart.FirstOrDefault(m => m._book_id == id);
                            cardItem._quantity += int.Parse(soluong);
                            Session["UpdatetoCart"] = 1;

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
        public ActionResult AddCartItem(string id)//Thêm sản phẩm vào giỏ hàng
        {
            if (Session["cart"] == null) // Nếu giỏ hàng chưa được khởi tạo
            {
                Session["cart"] = new List<EP3_ICE_CREAM.Models.Cart>();  // Khởi tạo Session["giohang"] là 1 List<Giohang>
            }

            if (id == null)
            {
                return View("error");
            }
            else
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
                        _price = (int)sanpham.priceBook - (int)sanpham.priceBook* (int)sanpham.discount/100,

                        _quantity = 1

                    };  // Tạo ra 1 CartItem mới

                    cart.Add(newItem);  // Thêm CartItem vào giỏ 
                }
                else
                {
                    // Nếu sản phẩm khách chọn đã có trong giỏ hàng thì không thêm vào giỏ nữa mà tăng số lượng lên.
                    EP3_ICE_CREAM.Models.Cart cardItem = cart.FirstOrDefault(m => m._book_id == id);
                    cardItem._quantity++;
                }

                return RedirectToAction("Index");
            }
        }
        public ActionResult UpdateCartItem(string id, int? quantity)
        {

            List<EP3_ICE_CREAM.Models.Cart> cart = Session["cart"] as List<EP3_ICE_CREAM.Models.Cart>;
            EP3_ICE_CREAM.Models.Cart cardItem = cart.Where(m => m._book_id == id).FirstOrDefault();
            if (cardItem == null)
            {
                return View("error");
            }
            else
            {
                cardItem._quantity = (int)quantity; 
            }
            Session["UpdateCart"] = 1;
            return RedirectToAction("ViewCartShopping");
        }
        public ActionResult DeleteCartItem(string id)
        {
            List<EP3_ICE_CREAM.Models.Cart> cart = Session["cart"] as List<EP3_ICE_CREAM.Models.Cart>;

            EP3_ICE_CREAM.Models.Cart cartItem = cart.FirstOrDefault(s => s._book_id == id);
            if (cartItem != null)
            {
                cart.Remove(cartItem);
            }
            Session["RemoveCart"] = 1;
            return RedirectToAction("ViewCartShopping");
        }
        // Gửi tin nhắn về gmail
        public JsonResult SendGmailtoUser(string gmail_user, string tieude, string content)
        {
            bool result = SendEmail(gmail_user, tieude, content);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public bool SendEmail(string toEmail, string Subject, string EmailBody)
        {
            try
            {
                string senderEmail = System.Configuration.ConfigurationManager.AppSettings["SenderEmail"].ToString(); //"webnamjp.nvn@gmail.com";
                string senderPassword = System.Configuration.ConfigurationManager.AppSettings["SenderPassword"].ToString(); //"idkzhfvlwvfwtbbo";


                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                client.Timeout = 100000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(senderEmail, senderPassword);

                MailMessage mailMessage = new MailMessage(senderEmail, toEmail, Subject, EmailBody);

                mailMessage.IsBodyHtml = true;
                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                client.Send(mailMessage);
                return true;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Sản phẩm: " + ex + "-");
                return false;
            }
        }

        [HttpPost]
        public JsonResult Buyproduct(string[] id, string username, string gmail, string address, string phone, string check)
        {
            string msg = "";
            try
            {
                if (Session["cart_temp"] == null) // Nếu giỏ hàng chưa được khởi tạo
                {
                    Session["cart_temp"] = new List<Cart_Temp>();  // Khởi tạo Session["giohang"] là 1 List<Giohang>
                }
                // Add book => Cart_Temp


                if (id != null)
                {
                    foreach (var item in id)
                    {
                        System.Diagnostics.Debug.WriteLine("Sản phẩm: " + item + "-");
                        // Kiểm tra sản phảm đã đc chọn mua
                        List<Cart_Temp> cart_temp = Session["cart_temp"] as List<Cart_Temp>;


                        var prod_item = cart_temp.Where(s => s._book_id == item).FirstOrDefault();


                        if (prod_item == null) // nếu chưa đc chọn
                        {
                            List<EP3_ICE_CREAM.Models.Cart> cart = Session["cart"] as List<EP3_ICE_CREAM.Models.Cart>;
                            var cart_item = cart.Where(s => s._book_id == item).FirstOrDefault();
                            Cart_Temp cart_temp_item = new Cart_Temp()
                            {
                                _book_id = cart_item._book_id,
                                _image_main = cart_item._image_main,
                                _book_name = cart_item._book_name,
                                _price = (int)cart_item._price,
                                _quantity = (int)cart_item._quantity
                            };
                            cart_temp.Add(cart_temp_item);
                        }
                        else
                        {
                            msg = "Bạn đã chọn mua sản phẩm này rồi.";
                        }
                    }
                }
                else
                {
                }

                // Create accout
                if (check == "yes") // Tạo tài khoản
                {
                    // Kiểm tra tài khoản đã tồn tại chưa
                    var check_user = db.Visitors.Where(s => s.email == gmail).FirstOrDefault();

                    if (check_user == null)
                    {
                        RandomPassword randomPassword = new RandomPassword();

                        string _password = randomPassword.RD_Password(6);
                        var new_user = new Visitor();
                        new_user.email = gmail;
                        new_user.fullName = username;
                        new_user.address = address;
                        new_user.phone = phone;
                        new_user.password = _password;
                        if (ModelState.IsValid)
                        {
                            db.Visitors.Add(new_user);
                            db.SaveChanges();
                        }
                        // Gửi password cho người dùng
                        string content = "Password:" + _password;
                        SendEmail(gmail, "Thanks You are Buy Books from ICE CREAM:" + DateTime.Now.ToString("ss:mm:hh,MM/dd/yyyy") + "", content);
                        Session["user"] = u_temp("null.jpg", username, gmail, address, phone, "1");
                    }
                    else
                    {
                        Session["user"] = u_temp("null.jpg", check_user.fullName, check_user.email, check_user.address, check_user.phone, "1");
                    }


                }
                else
                {
                    Session["user"] = u_temp("null.jpg", username, gmail, address, phone, "0");
                }




                return Json(new { code = 200, url = Url.Action("Transaction", "Home"), msg = "Chon mua thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Chon mua That bai" + ex }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Transaction() // Giao dịch
        {
            return View(db.Books.ToList());
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
        public ActionResult ChangePassword(string id, string currentPassword, string newPassword, string confirmPassword)
        {
            Session["thongbao_password"] = "";
            string thongbao = "";
            if (currentPassword == null || newPassword == null)
            {
                thongbao = "Bạn chưa nhập thông tin";
            }

            if (newPassword == confirmPassword)
            {
                var user = db.RegisteredUsers.Where(s => s.email == id && s.password == currentPassword).FirstOrDefault();
                if (user != null)
                {
                    user.password = newPassword;
                    if (ModelState.IsValid)
                    {
                        db.SaveChanges();
                        thongbao = "Password Has Reset";
                    }
                }
                else
                {
                    thongbao = "Password Wrong";
                }


            }
            else
            {
                thongbao = "Confirm Password Not Exact !";
            }
            TempData["thongbao_password"] = thongbao;
            return RedirectToAction("ThongtinUser");
        }


    }
}