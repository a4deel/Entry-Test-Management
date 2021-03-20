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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AdminLogin(AdminLogin admin)
        {
            if (ModelState.IsValid)
            {
                var foundAdmin = DataStorage.AdminLogins.Where(obj => obj.email.Equals(admin.email) && obj.password.Equals(admin.password)).FirstOrDefault();
                if (foundAdmin != null)
                {
                    Session["AdminID"] = foundAdmin.id.ToString();
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

        public ActionResult ViewAdmins()
        {
            if (Session["AdminEmail"] != null)
            {
                var admins = DataStorage.AdminDatas.ToList();
                return View(admins);
            }
            else
            {
                return RedirectToAction("AdminLogin");
            }
        }

        [HttpGet]
        public ActionResult AdminProfile(int? id)
        {
            if (Session["AdminEmail"] != null)
            {
                if (id != null)
                {
                    var admin = DataStorage.AdminDatas.Find(id);
                    if(admin != null)
                    {
                        return View(admin);
                    }
                    else
                    {
                        return RedirectToAction("ViewAdmins");
                    }
                }
                else
                {
                    return RedirectToAction("ViewAdmins");
                }
            }
            else
            {
                return RedirectToAction("AdminLogin");
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