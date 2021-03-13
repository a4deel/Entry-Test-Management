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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdminLogin(String email, String password)
        {
            var foundAdmin = DataStorage.AdminLogins.Where(obj => obj.email.Equals(email) && obj.password.Equals(password)).FirstOrDefault();
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

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("AdminLogin");
        }
    }
}