using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PVBClothing.Models;

namespace PVBClothing.Controllers
{
    public class HomeController : Controller
    {
        PVBClothingEntities db = new PVBClothingEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Contact(Contact contact)
        {
            contact.dateContact = DateTime.Now;
            db.Contacts.Add(contact);
            var result = db.SaveChanges();
            if (result > 0)
            {
                ViewBag.MessageSuccess = "Your message has been received by us. We will contact you as soon as possible.";
            }
            return View(contact);
        }

        [HttpGet]
        public ActionResult EditProfie(string userName)
        {
            Member member = db.Members.Find(userName);
            Session["imgPath"] = member.avatar;
            return View(member);
        }
        [HttpPost]
        public ActionResult EditProfie(Member member, HttpPostedFileBase uploadFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (uploadFile != null)
                    {
                        var fileName = Path.GetFileName(uploadFile.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/img/avatar"), fileName);

                        member.avatar = "~/Content/img/avatar/" + fileName;
                        db.Entry(member).State = EntityState.Modified;

                        string oldImgPath = Request.MapPath(Session["imgPath"].ToString()); // Lấy đường dẫn ảnh (absolute path)
                        var avatarName = Session["imgPath"].ToString(); // Lấy đường dẫn ảnh (relative path)
                        var checkAvatart = db.Members.Where(model => model.avatar == avatarName).ToList(); // Kiểm tra ảnh có trùng với avatar của member nào không

                        if (db.SaveChanges() > 0)
                        {
                            uploadFile.SaveAs(path);
                            if (System.IO.File.Exists(oldImgPath) && checkAvatart.Count < 2) // Nếu tồn tại hình trong folder và không member nào có hình này thì xóa ra khỏi folder
                            {
                                System.IO.File.Delete(oldImgPath);
                            }
                            var info = db.Members.Where(model => model.userName == member.userName).SingleOrDefault();// Lấy thông tin mới cập nhập lưu vào session
                            Session["info"] = info;
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        member.avatar = Session["imgPath"].ToString();
                        db.Entry(member).State = EntityState.Modified;
                        if (db.SaveChanges() > 0)
                        {
                            var info = db.Members.Where(model => model.userName == member.userName).SingleOrDefault();// Lấy thông tin mới cập nhập lưu vào session
                            Session["info"] = info;
                            return RedirectToAction("Index");
                        }
                    }
                }
                ViewBag.roleId = new SelectList(db.Roles, "roleId", "roleName", member.roleId);
                return View(member);
            }
            catch (Exception ex)
            {
                TempData["msgEditProfieFailed"] = "Edit failed! " + ex.Message;
                return RedirectToAction("Index");
            }
        }


    }
}