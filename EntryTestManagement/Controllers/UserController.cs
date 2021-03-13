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
    public class UserController : Controller
    {
        EntryTestDetailsDBEntities1 DataStorage = new EntryTestDetailsDBEntities1();
        // GET: User
        public ActionResult Index()
        {
            if (Session["UserEmail"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("UserLogin");
            }
        }

        public ActionResult UserLogin()
        {
            Session.Clear();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserLogin(String email, String password)
        {
            password = GetMD5(password);
            var foundUser = DataStorage.UserLogins.Where(obj => obj.email.Equals(email) && obj.password.Equals(password)).FirstOrDefault();
            if (foundUser != null)
            {
                Session["UserEmail"] = foundUser.email.ToString();
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "*Incorrect Email/Password";
                return RedirectToAction("UserLogin");
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("UserLogin");
        }

        public ActionResult RegisterUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterUser(UserLogin user)
        {
            var foundUser = DataStorage.UserLogins.Where(obj => obj.email.Equals(user.email)).FirstOrDefault();
            if (foundUser == null)
            {
                user.password = GetMD5(user.password);
                DataStorage.UserLogins.Add(user);
                DataStorage.SaveChanges();
                return RedirectToAction("UserLogin");
            }
            else
            {
                TempData["Error"] = "*Email already exists";
                return RedirectToAction("RegisterUser");
            }
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