using EntryTestManagement.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
            if (Session["AdminEmail"] != null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
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
                    var Admin = DataStorage.AdminDatas.Where(obj => obj.email.Equals(admin.email)).FirstOrDefault();
                    Session["AdminID"] = foundAdmin.id.ToString();
                    Session["AdminEmail"] = foundAdmin.email.ToString();
                    Session["AdminRole"] = Admin.Role.ToString();
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

        [HttpGet]
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
        public ActionResult AddAdmin(AdminData admin)
        {
            if (ModelState.IsValid)
            {
                string FileName = "";
                string FileExtension = "";
                string UploadPath = "";
                var foundAdmin = DataStorage.AdminDatas.Where(obj => obj.email.Equals(admin.email)).FirstOrDefault();
                if (foundAdmin == null)
                {
                    DataStorage.Configuration.ValidateOnSaveEnabled = false;
                    admin.Role = "Sub";
                    if(admin.ImageFile != null)
                    {
                        FileName = Path.GetFileName(admin.ImageFile.FileName);
                        FileExtension = Path.GetExtension(admin.ImageFile.FileName);
                        FileName = "(" + admin.FirstName + ")" + FileName;
                        UploadPath = Server.MapPath("~\\Content\\styles\\img\\admins\\"+ FileName);
                        admin.Image = FileName;
                    }
                    else
                    {
                        admin.Image = "user";
                    }
                    AdminLogin obj = new AdminLogin
                    {
                        email = admin.email,
                        password = admin.FirstName + "1234"
                    };
                    DataStorage.AdminLogins.Add(obj);
                    DataStorage.AdminDatas.Add(admin);
                    DataStorage.SaveChanges();
                    admin.ImageFile.SaveAs(UploadPath);
                    TempData["Error"] = "New Sub Admin Added Successfully";
                    return RedirectToAction("AddAdmin");
                }
                else
                {
                    TempData["Error"] = "Admin Already Exists";
                    return RedirectToAction("AddAdmin");
                }
            }
            else
            {
                return View(admin);
            }
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

        [HttpGet]
        public ActionResult DeleteAdmin(int? id)
        {
            if (id != null && Session["AdminRole"].Equals("Super"))
            {
                AdminData adminData = DataStorage.AdminDatas.Find(id);
                AdminLogin adminLogin = DataStorage.AdminLogins.Find(id);
                if(adminData != null && adminLogin != null)
                {
                    DataStorage.AdminDatas.Remove(adminData);
                    DataStorage.AdminLogins.Remove(adminLogin);
                    DataStorage.SaveChanges();
                    return RedirectToAction("ViewAdmins");
                }
                else
                {
                    TempData["Error"] = "Only Super Admin can delete/Admin Deletion ID is Null";
                    return RedirectToAction("ViewAdmins");
                }
            }
            else
            {
                return RedirectToAction("ViewAdmins");
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