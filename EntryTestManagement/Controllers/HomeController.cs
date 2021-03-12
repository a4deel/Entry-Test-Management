using System;
using System.Linq;
using System.Web.Mvc;
using EntryTestManagement.Models;

namespace EntryTestManagement.Controllers
{
    public class HomeController : Controller
    {
        EntryTestDetailsDBEntities1 DataStorage = new EntryTestDetailsDBEntities1();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserLogin()
        {
            return View();
        }

        public ActionResult AdminLogin()
        {
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
                return RedirectToAction("Dashboard");
            }
            else
            {
                TempData["Error"] = "(Incorrect email/password)";
                return RedirectToAction("AdminLogin");
            }

        }

        public ActionResult RegisterUser()
        {
            return View();
        }

        public ActionResult Dashboard()
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

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }
    }
}