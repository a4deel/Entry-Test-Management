using EntryTestManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EntryTestManagement.Controllers
{
    public class AdminController : Controller
    {
        EntryTestDetailsDBEntities1 DataStorage = new EntryTestDetailsDBEntities1();
        // GET: Admin
        public ActionResult Index()
        {
            if (Session["AdminEmail"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("AdminLogin");
            }
        }

        public ActionResult AdminLogin()
        {
            Session.Clear();
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult AdminLogin(AdminLogin admin)
        {
            if (ModelState.IsValid)
            {
                var foundAdmin = DataStorage.AdminLogins.Where(obj => obj.email.Equals(admin.email) && obj.password.Equals(admin.password)).FirstOrDefault();
                if (foundAdmin != null)
                {
                    Session["AdminEmail"] = foundAdmin.email.ToString();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Error"] = "*Incorrect Email/Password";
                    return RedirectToAction("AdminLogin");
                }
            }
            else
            {
                return View(admin);
            }
        }

        public ActionResult AddAdmin()
        {
            if (Session["AdminEmail"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("AdminLogin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAdmin(String a)
        {
            return RedirectToAction("Index");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("AdminLogin");
        }
    }
}