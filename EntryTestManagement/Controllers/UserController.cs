using EntryTestManagement.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
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
                string email = Session["UserEmail"].ToString();
                var foundUser = DataStorage.UserDatas.Where(obj => obj.email.Equals(email)).FirstOrDefault();
                if (foundUser == null)
                {
                    Session["Added"] = "Not Added";
                }
                else
                {
                    Session["UserName"] = foundUser.FirstName + " " + foundUser.LastName;
                    Session["UserID"] = foundUser.id;
                    Session["Added"] = "Added";
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult Login()
        {
            if (Session["UserEmail"] != null)
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
        public ActionResult Login(UserLogin user)
        {
            if (ModelState.IsValid)
            {
                user.password = GetMD5(user.password);
                var foundUser = DataStorage.UserLogins.Where(obj => obj.email.Equals(user.email) && obj.password.Equals(user.password)).FirstOrDefault();
                if (foundUser != null)
                {
                    Session["UserID"] = foundUser.id.ToString();
                    Session["UserEmail"] = foundUser.email.ToString();
                    Session["LoggedIn"] = "User";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Message"] = "*Incorrect Email/Password";
                    return RedirectToAction("Login");
                }
            }
            else
            {
                return View(user);
            }
        }

        public ActionResult Register()
        {
            if (Session["UserEmail"] != null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(UserLogin user)
        {
            if (ModelState.IsValid)
            {
                var foundUser = DataStorage.UserLogins.Where(obj => obj.email.Equals(user.email)).FirstOrDefault();
                if (foundUser == null)
                {
                    user.password = GetMD5(user.password);
                    DataStorage.Configuration.ValidateOnSaveEnabled = false;
                    DataStorage.UserLogins.Add(user);
                    DataStorage.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["Message"] = "*Email already exists";
                    return RedirectToAction("RegisterUser");
                }
            }
            else
            {
                return View(user);
            }
        }

        [HttpGet]
        public ActionResult Complain()
        {
            if (Session["UserEmail"] != null)
            {
                return View();
            }
            else
            {
                TempData["Message"] = "Dear User Kindly Login First";
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public ActionResult Complain(Complaint comp)
        {
            if (ModelState.IsValid)
            {
                comp.Date = DateTime.Now.ToString("dddd, dd MMMM yyyy hh:mm tt");
                comp.Status = "Pending";
                comp.UserEmail = Session["UserEmail"].ToString();
                DataStorage.Complaints.Add(comp);
                DataStorage.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View(comp);
            }
        }

        public ActionResult Add()
        {
            if (Session["UserEmail"] != null)
            {
                return View();
            }
            else
            {
                TempData["Message"] = "Dear User Kindly Login First";
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(UserData user)
        {
            if (ModelState.IsValid)
            {
                string name = "";
                int ID = 0;
                string email = Session["UserEmail"].ToString();
                string UploadPath = "";
                var foundUser = DataStorage.UserDatas.Where(obj => obj.email.Equals(email)).FirstOrDefault();
                var lastUser = DataStorage.UserDatas.LastOrDefault();
                if (foundUser == null)
                {
                    if(lastUser != null)
                    {
                        ID = lastUser.id + 1;
                        user.ChallanNo = "BF520" + ID;
                    }
                    else
                    {
                        ID = 1;
                        user.ChallanNo = "BF520" + ID;
                    }
                    string fullName = user.FirstName + " " + user.LastName;
                    user.Status = "Applied";
                    user.email = email;
                    DataStorage.Configuration.ValidateOnSaveEnabled = false;
                    if (user.ImageFile != null)
                    {
                        name = Path.GetFileName(user.ImageFile.FileName);
                        UploadPath = Server.MapPath("~\\Content\\styles\\img\\users\\" + name);
                        user.Image = name;
                        user.ImageFile.SaveAs(UploadPath);
                    }
                    else
                    {
                        user.ChallanNo = "BF520" + ID;
                        user.Image = "user.png";
                    }
                    DataStorage.UserDatas.Add(user);
                    DataStorage.SaveChanges();
                    Session["Added"] = "Added";

                    string subject = "User Registration Complated Successfully";
                    string message = "Dear " + fullName + ", You personal information has been successfully recieved to our servers. You'll shortly be able to download your challan form from the system.";
                    sendEmail(email, subject, message, fullName);
                    TempData["Message"] = "Information Added Successfully";

                    return RedirectToAction("index");
                }
                else
                {
                    TempData["Message"] = "User Already Exists ";
                    return RedirectToAction("Add");
                }
            }
            else
            {
                return View(user);
            }
        }

        public ActionResult Challan()
        {
            if (Session["UserEmail"] != null)
            {
                string email = Session["UserEmail"].ToString();
                var foundUser = DataStorage.UserDatas.Where(obj => obj.email.Equals(email)).FirstOrDefault();
                if(foundUser != null) {
                    return View(foundUser);
                }
                else
                {
                    TempData["Message"] = "Information not added yet.";
                    return RedirectToAction("index");
                }
            }
            else
            {
                TempData["Message"] = "Dear User Kindly Login First";
                return RedirectToAction("Login");
            }
        }

        //View Single User
        [HttpGet]
        public ActionResult UserProfile(int? id)
        {
            if (Session["UserEmail"] != null)
            {
                string email = Session["UserEmail"].ToString();
                var foundUser = DataStorage.UserDatas.Where(obj => obj.email.Equals(email)).FirstOrDefault();
                if (foundUser != null)
                {
                    if (id != null)
                    {
                        var User = DataStorage.UserDatas.Find(id);
                        if (User != null)
                        {
                            return View(User);
                        }
                        else
                        {
                            TempData["Message"] = "User Not Found";
                            return RedirectToAction("index");
                        }
                    }
                    else
                    {
                        TempData["Message"] = "NULL ID is given";
                        return RedirectToAction("index");
                    }
                }
                else
                {
                    TempData["Message"] = "User Information Not Added yet";
                    return RedirectToAction("index");
                }
            }
            else
            {
                TempData["Message"] = "Dear User Kindly Login First";
                return RedirectToAction("Login");
            }
        }

        //Edit Users
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (Session["UserEmail"] != null)
            {
                if(id != null)
                {
                    var foundUser = DataStorage.UserDatas.Find(id);
                    if (foundUser != null)
                    {
                        return View(foundUser);
                    }
                    else
                    {
                        TempData["Message"] = "User Not Found";
                        return RedirectToAction("index");
                    }
                }
                else
                {
                    TempData["Message"] = "Null ID is Given";
                    return RedirectToAction("index");
                }
            }
            else
            {
                TempData["Message"] = "Dear User Kindly Login First";
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        public ActionResult EditUser(UserData user)
        {
            if (ModelState.IsValid)
            {
                string name = "";
                string UploadPath = "";
                if (user.ImageFile != null)
                {
                    name = Path.GetFileName(user.ImageFile.FileName);
                    UploadPath = Server.MapPath("~\\Content\\styles\\img\\users\\" + name);
                    user.Image = name;
                    user.ImageFile.SaveAs(UploadPath);
                }
                //else
                //{
                //    user.Image = Session["Image"].ToString();
                //    Session["Image"] = "";
                //}
                DataStorage.Entry(user).State = EntityState.Modified;
                DataStorage.SaveChanges();
                return RedirectToAction("index");
            }
            else
            {
                return View(user);
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

        private void sendEmail(string receiver, string subject, string message, String name)
        {
            var fromAddress = new MailAddress("head.fyp@gmail.com", "System Super User");
            var toAddress = new MailAddress(receiver, name);
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