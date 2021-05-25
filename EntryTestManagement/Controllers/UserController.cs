using EntryTestManagement.Models;
using System;
using System.Linq;
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
                    Session["AdminID"] = foundUser.id.ToString();
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
            if(ModelState.IsValid)
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