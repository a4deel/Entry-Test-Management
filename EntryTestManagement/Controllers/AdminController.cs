using EntryTestManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
                admin.password = GetMD5(admin.password);
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

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("AdminLogin");
        }

        private string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }
    }
}