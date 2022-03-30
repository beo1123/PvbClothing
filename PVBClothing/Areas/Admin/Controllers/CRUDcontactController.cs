using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PVBClothing.Models;
using System.IO;
using PagedList;
using PagedList.Mvc;
using System.Web.Security;
using System.Net.Mail;
using System.Text;

namespace PVBClothing.Areas.Admin.Controllers
{
    public class CRUDcontactController : Controller
    {
        PVBClothingEntities db = new PVBClothingEntities();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult DsContact()
        {
            try
            {
                var dsContact = (from i in db.Contacts
                                 orderby i.dateContact descending
                                 select new
                                 {
                                     id = i.id,
                                     name = i.name,
                                     email = i.email,
                                     message = i.message
                                 }).ToList();

                return Json(new { code = 200, dsContact = dsContact, msg = "Successfully get list contact!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Get list contact false: " + ex.Message, JsonRequestBehavior.AllowGet });
            }
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                Contact contact = (from i in db.Contacts
                                   select i).SingleOrDefault(model => model.id == id);
                db.Contacts.Remove(contact);
                db.SaveChanges();
                return Json(new { code = 200, msg = "Successfully delete!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { code = 500, msg = "Failed delete! " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        
    }
}