using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PVBClothing.Models;

namespace PVBClothing.Controllers
{
    public class CartController : Controller
    {
        PVBClothingEntities db = new PVBClothingEntities();
        // GET: Cart
        public List<Cart> getCart() // Create list cart and save in session 
        {
            List<Cart> listCart = Session["Cart"] as List<Cart>;
            if (listCart == null)
            {
                // If the list item in the cart empty, it will create a list contains items
                listCart = new List<Cart>();
                Session["Cart"] = listCart;
            }
            return listCart;
        }

        public ActionResult AddToCart(int id, string strURL) //  Add item in cart 
        {
            List<Cart> listCart = getCart();
            Cart item = listCart.Find(model => model.IdItem == id);
            if (item == null)
            {
                item = new Cart(id);
                listCart.Add(item);
                return Redirect(strURL);
            }
            else
            {
                item.Quantity++;
                return Redirect(strURL);
            }
        }

        private int Quanlity() // Lấy tổng số sản phẩm giỏ hàng hiện tại
        {
            int amount = 0;
            List<Cart> listCart = Session["Cart"] as List<Cart>;
            if (listCart != null)
            {
                amount = listCart.Sum(model => model.Quantity);
            }
            return amount;
        }
        private double TotalPrice() //  Lấy tổng số tiền sản phẩm
        {
            double total = 0;
            List<Cart> listCart = Session["Cart"] as List<Cart>;
            if (listCart != null)
            {
                total = listCart.Sum(model => model.PriceTotal);
            }
            return total;
        }
        public PartialViewResult Navbar() // Hiển thị số lượng sản phẩm và tiền trên navbar
        {
            ViewBag.quanlityItem = Quanlity();
            ViewBag.totalPrice = TotalPrice();
            return PartialView();
        }


        public ActionResult NoICart()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Cart()
        {
            List<Cart> listCart = getCart();
            Session["Cart"] = listCart;
            if (listCart.Count == 0)
            {
                return RedirectToAction("NoICart", "Cart");
            }
            ViewBag.quanlityItem = Quanlity();
            ViewBag.totalPrice = TotalPrice();
            return View(listCart);
        }

        public ActionResult DeteteCart(int id)
        {
            List<Cart> listCart = getCart();
            Cart item = listCart.SingleOrDefault(model => model.IdItem == id);
            if (item != null)
            {
                listCart.RemoveAll(model => model.IdItem == id);
                return RedirectToAction("Cart", "Cart");
            }
            if (listCart.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Cart", "Cart");
        }
        [HttpPost]
        //public ActionResult UpdateCart(FormCollection form)
        public ActionResult Cart(FormCollection form)
        {
            string[] qualities = form.GetValues("quanlity");
            List<Cart> listCart = getCart();
            for (int i = 0; i < listCart.Count; i++)
            {
                listCart[i].Quantity = Convert.ToInt32(qualities[i]);
            }
            Session["Cart"] = listCart;
            if (Quanlity() == 0)
            {
                Session["Cart"] = null;
                return RedirectToAction("NoICart", "Cart");
            }
            else
            {
                return RedirectToAction("Cart", "Cart");
            }
        }

        public static string ConvertTimeTo24(string hour)
        {
            string h = "";
            switch (hour)
            {
                case "1":
                    h = "13";
                    break;
                case "2":
                    h = "14";
                    break;
                case "3":
                    h = "15";
                    break;
                case "4":
                    h = "16";
                    break;
                case "5":
                    h = "17";
                    break;
                case "6":
                    h = "18";
                    break;
                case "7":
                    h = "19";
                    break;
                case "8":
                    h = "20";
                    break;
                case "9":
                    h = "21";
                    break;
                case "10":
                    h = "22";
                    break;
                case "11":
                    h = "23";
                    break;
                case "12":
                    h = "0";
                    break;
            }
            return h;
        }
        public static string CreateKey(string tiento) // Tạo chuỗi mã hóa ngày - giờ cho mã hóa đơn
        {
            string key = tiento;
            string[] partsDay;
            partsDay = DateTime.Now.ToShortDateString().Split('/');
            //Ví dụ 07/08/2009
            string d = String.Format("{0}{1}{2}", partsDay[0], partsDay[1], partsDay[2]);
            key = key + d;
            string[] partsTime;
            partsTime = DateTime.Now.ToLongTimeString().Split(':');
            //Ví dụ 7:08:03 PM hoặc 7:08:03 AM
            if (partsTime[2].Substring(3, 2) == "PM")
                partsTime[0] = ConvertTimeTo24(partsTime[0]);
            if (partsTime[2].Substring(3, 2) == "AM")
                if (partsTime[0].Length == 1)
                    partsTime[0] = "0" + partsTime[0];
            //Xóa ký tự trắng và PM hoặc AM
            partsTime[2] = partsTime[2].Remove(2, 3);
            string t;
            t = String.Format("_{0}{1}{2}", partsTime[0], partsTime[1], partsTime[2]);
            key = key + t;
            return key;
        }

        // Trang checkout khi đã đăng nhập tài khoản
        [HttpGet]
        public ActionResult Checkout()
        {
            List<Cart> listCart = getCart();
            ViewBag.quanlityItem = Quanlity();
            ViewBag.totalPrice = TotalPrice();

            return View(listCart);
        }
        [HttpPost]
        public ActionResult Checkout(FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Invoince bill = new Invoince();
                    Member member = (Member)Session["info"]; // Lấy thông tin tài khoản từ session 
                    List<Cart> listCart = getCart();
                    bill.invoinceNo = CreateKey("HD");
                    bill.userName = member.userName;
                    bill.dateOrder = DateTime.Now;
                    bill.status = true;
                    bill.deliveryDate = null;
                    bill.deliveryStatus = false;

                    // Biến totalmney lưu tổng tiền sản phẩm từ giỏ hàng
                    int totalmoney = 0;
                    foreach (var item in listCart)
                    {
                        totalmoney += Convert.ToInt32(item.PriceTotal);
                    }
                    bill.totalMoney = totalmoney;
                    db.Invoinces.Add(bill);
                    db.SaveChanges();
                    foreach (var item in listCart)
                    {
                        InvoinceDetail ctdh = new InvoinceDetail();
                        ctdh.invoinceNo = bill.invoinceNo;
                        ctdh.productId = item.IdItem;
                        ctdh.quanlityProduct = item.Quantity;
                        ctdh.unitPrice = item.unitPrice;
                        ctdh.totalPrice = (int?)(long)item.PriceTotal;
                        ctdh.totalDiscount = item.Discount * item.Quantity;
                        db.InvoinceDetails.Add(ctdh);
                    }
                    db.SaveChanges();
                    return RedirectToAction("SubmitBill", "Cart");
                }
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        //Trang checkout khi không đăng nhập tài khoản
        [HttpGet]
        public ActionResult CheckoutNoAccount()
        {
            List<Cart> listCart = getCart();
            ViewBag.quanlityItem = Quanlity();
            ViewBag.totalPrice = TotalPrice();
            return View(listCart);
        }
        [HttpPost]
        public ActionResult CheckoutNoAccount(Customer customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var check = db.Customers.Where(model => model.lastName == customer.lastName && model.email == customer.email).FirstOrDefault();
                    // Kiểm tra xem tên khách hàng có email nhập đã tồn tại hay chưa
                    if (check == null) // Nếu chưa thì lưu lại thông tin vào bảng customer
                    {
                        db.Customers.Add(customer);
                        db.SaveChanges();
                        Invoince bill = new Invoince();
                        List<Cart> listCart = getCart();
                        bill.customerId = customer.customerId;
                        bill.invoinceNo = CreateKey("HD");
                        bill.dateOrder = DateTime.Now;
                        bill.status = true;
                        bill.deliveryDate = null;
                        bill.deliveryStatus = false;
                        int totalmoney = 0;
                        foreach (var item in listCart)
                        {
                            totalmoney += Convert.ToInt32(item.PriceTotal);
                        }
                        bill.totalMoney = totalmoney;
                        db.Invoinces.Add(bill);
                        db.SaveChanges();
                        foreach (var item in listCart)
                        {
                            InvoinceDetail ctdh = new InvoinceDetail();
                            ctdh.invoinceNo = bill.invoinceNo;
                            ctdh.productId = item.IdItem;
                            ctdh.quanlityProduct = item.Quantity;
                            ctdh.unitPrice = item.unitPrice;
                            ctdh.totalPrice = (int?)(long)item.PriceTotal;
                            ctdh.totalDiscount = item.Discount * item.Quantity;
                            db.InvoinceDetails.Add(ctdh);
                        }
                        db.SaveChanges();
                        return RedirectToAction("SubmitBill", "Cart");
                    }
                    else
                    {
                        Invoince bill = new Invoince();
                        List<Cart> listCart = getCart();
                        bill.customerId = check.customerId;
                        bill.invoinceNo = CreateKey("HD");
                        bill.dateOrder = DateTime.Now;
                        bill.status = true;
                        bill.deliveryDate = null;
                        bill.deliveryStatus = false;
                        int totalmoney = 0;
                        foreach (var item in listCart)
                        {
                            totalmoney += Convert.ToInt32(item.PriceTotal);
                        }
                        bill.totalMoney = totalmoney;
                        db.Invoinces.Add(bill);
                        db.SaveChanges();
                        foreach (var item in listCart)
                        {
                            InvoinceDetail ctdh = new InvoinceDetail();
                            ctdh.invoinceNo = bill.invoinceNo;
                            ctdh.productId = item.IdItem;
                            ctdh.quanlityProduct = item.Quantity;
                            ctdh.unitPrice = item.unitPrice;
                            ctdh.totalPrice = (int?)(long)item.PriceTotal;
                            ctdh.totalDiscount = item.Discount * item.Quantity;
                            db.InvoinceDetails.Add(ctdh);
                        }
                        db.SaveChanges();
                        return RedirectToAction("SubmitBill", "Cart");
                    }

                }

                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public ActionResult SubmitBill()
        {
            ViewBag.quanlityItem = Quanlity();
            ViewBag.totalPrice = TotalPrice();
            return View();
        }
        [HttpPost]
        public ActionResult SubmitBill(FormCollection form)
        {
            Session.Remove("Cart");
            //Session["Cart"] = null;
            return RedirectToAction("Index", "Home");
        }

    }
}