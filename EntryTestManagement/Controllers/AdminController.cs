using EntryTestManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
                string name = "";
                string UploadPath = "";
                var foundAdmin = DataStorage.AdminDatas.Where(obj => obj.email.Equals(admin.email)).FirstOrDefault();
                if (foundAdmin == null)
                {
                    string fullName = admin.FirstName + " " + admin.LastName;
                    DataStorage.Configuration.ValidateOnSaveEnabled = false;
                    admin.Role = "Sub";
                    if (admin.ImageFile != null)
                    {
                        name = Path.GetFileName(admin.ImageFile.FileName);
                        UploadPath = Server.MapPath("~\\Content\\styles\\img\\admins\\" + name);
                        admin.Image = name;
                    }
                    else
                    {
                        admin.Image = "user.png";
                    }
                    string pass = RandomPassword();
                    AdminLogin obj = new AdminLogin
                    {
                        email = admin.email,
                        password = pass
                    };
                    DataStorage.AdminLogins.Add(obj);
                    DataStorage.AdminDatas.Add(admin);
                    DataStorage.SaveChanges();
                    admin.ImageFile.SaveAs(UploadPath);
                    string subject = "Admin Password Reset Invitation";
                    var message = "Your dummy password is generated kindly reset this password and update it. Your password is "+pass;
                    sendEmail(admin.email,subject,message,fullName);
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
                    if (admin != null)
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
                if (adminData != null && adminLogin != null)
                {
                    DataStorage.AdminDatas.Remove(adminData);
                    DataStorage.AdminLogins.Remove(adminLogin);
                    DataStorage.SaveChanges();
                    return RedirectToAction("ViewAdmins");
                }
                else
                {
                    TempData["Error"] = "Admin Not Found";
                    return RedirectToAction("ViewAdmins");
                }
            }
            else
            {
                TempData["Error"] = "Null ID is given/Only Super Admin Can Perform This Operation";
                return RedirectToAction("ViewAdmins");
            }
        }

        [HttpGet]
        public ActionResult EditAdmin(int? id)
        {
            if (id != null && Session["AdminRole"].Equals("Super"))
            {
                AdminData adminData = DataStorage.AdminDatas.Find(id);
                if (adminData != null)
                {
                    return View(adminData);
                }
                else
                {
                    TempData["Error"] = "Admin Not Found";
                    return RedirectToAction("ViewAdmins");
                }
            }
            else
            {
                TempData["Error"] = "Null ID is given/Only Super Admin Can Perform This Operation";
                return RedirectToAction("ViewAdmins");
            }
        }

        [HttpPost]
        public ActionResult EditAdmin(AdminData admin)
        {
            if (ModelState.IsValid)
            {
                string name = "";
                string UploadPath = "";
                if (admin.ImageFile != null)
                {
                    name = Path.GetFileName(admin.ImageFile.FileName);
                    UploadPath = Server.MapPath("~\\Content\\styles\\img\\admins\\" + name);
                    admin.Image = name;
                }
                else
                {
                    admin.Image = "user.png";
                }
                admin.Role = "Sub";
                DataStorage.Entry(admin).State = EntityState.Modified;
                DataStorage.SaveChanges();
                admin.ImageFile.SaveAs(UploadPath);
                return RedirectToAction("ViewAdmins");
            }
            else
            {
                return View(admin);
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

        private string RandomPassword()
        {
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            Random random = new Random();
            int size = random.Next(8, validChars.Length);
            char[] chars = new char[size];
            for (int i = 0; i < size; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }
            return new string(chars);
        }

        private void sendEmail(string receiver, string subject, string message,String name)
        {
            var fromAddress = new MailAddress("head.fyp@gmail.com", "System Super Admin");
            var toAddress = new MailAddress(receiver,name);
            string fromPassword = "head.fyp123!";
            string Subject = subject;
            string body = message;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var mail = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(mail);
            }
        }
    }
}