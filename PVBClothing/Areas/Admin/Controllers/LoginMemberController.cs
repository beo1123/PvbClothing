﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PVBClothing.Models;

namespace PVBClothing.Areas.Admin.Controllers
{
    public class LoginMemberController : Controller
    {
        PVBClothingEntities db = new PVBClothingEntities();
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var tk = collection["username"];
            var mk = collection["password"];

            var notMember = db.Members.Where(model => model.roleId == 3).SingleOrDefault(model => model.userName == tk && model.password == mk);
            var check = db.Members.Where(model => model.roleId == 1 || model.roleId == 2).SingleOrDefault(model => model.userName == tk && model.password == mk);
            if (ModelState.IsValid)
            {
                if (check == null)
                {
                    if (notMember != null)
                    {
                        ModelState.AddModelError("", "Role not invalid.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "There was a problem logging in. Check your username and password or create an account.");
                    }
                }

                else
                {
                  
                        FormsAuthentication.SetAuthCookie(check.lastName, false);
                        Session["userNameAdmin"] = check.userName;
                        Session["infoAdmin"] = check;
                        return RedirectToAction("Index", "DashBoard");
                    
                }
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}