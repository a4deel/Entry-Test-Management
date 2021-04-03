using EntryTestManagement.Models;
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
                return RedirectToAction("UserLogin");
            }
        }

        public ActionResult UserLogin()
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
        public ActionResult UserLogin(UserLogin user)
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
                    return RedirectToAction("UserLogin");
                }
            }
            else
            {
                return View(user);
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
            if (ModelState.IsValid)
            {
                var foundUser = DataStorage.UserLogins.Where(obj => obj.email.Equals(user.email)).FirstOrDefault();
                if (foundUser == null)
                {
                    user.password = GetMD5(user.password);
                    DataStorage.Configuration.ValidateOnSaveEnabled = false;
                    DataStorage.UserLogins.Add(user);
                    DataStorage.SaveChanges();
                    return RedirectToAction("UserLogin");
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