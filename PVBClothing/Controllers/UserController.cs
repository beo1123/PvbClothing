using PVBClothing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PVBClothing.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        PVBClothingEntities db = new PVBClothingEntities();

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (Session["info"] != null) // Nếu đã đăng nhập rồi thì sửa link vào đăng nhập sẽ điều hướng sang trang chủ
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Login(FormCollection collection)
        {
            var tk = collection["username"];
            var mk = collection["password"];
            mk = Encryptor.MD5Hash(mk);

            if (ModelState.IsValid)
            {
                var check = db.Members.FirstOrDefault(model => model.userName == tk && model.password == mk);

                if (check != null)
                {
                    Session["FullName"] = check.firstName + " " + check.lastName;
                    Session["info"] = check;
                    return Json(new { msg = "Login success!", Url = "/" });
                    
                }
                else
                {
                   
                    return Json(new { msg = "There was a problem logging in. Check your username and password or create an account.", Url = "/User/Login" });


                }
            }
            return View();


        }

        public ActionResult logout()
        {
            Session.Remove("info");
            //Session["info"] = null;
            //Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register( Member member)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    var check = db.Members.Where(model => model.userName == member.userName).FirstOrDefault();
                    



                    if (check != null)
                    {
                        // check username constained in database
                        TempData["msgEmailAlreadyExist"] = "There was a problem creating your account. Your username already exists!";
                       // ModelState.AddModelError("", "There was a problem creating your account. Your username already exists.");
                        return View("Login");
                    }
                    else
                    {
                        
                        member.password = Encryptor.MD5Hash(member.password);
                        member.RegisteredDate = DateTime.Now;
                        member.roleId = 3;
                        member.avatar = "~/Content/img/logo1.png";
                        member.status = true;
                        db.Members.Add(member);
                        var result = db.SaveChanges();
                        if (result > 0)
                        {
                            TempData["msgSuccess"] = "Successfully create account!";
                            return RedirectToAction("Login");
                        }


                    }
                }
                
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                TempData["msgFailed"] = "Failed create account! ";
                return RedirectToAction("Login");
            }
        }


    }
}