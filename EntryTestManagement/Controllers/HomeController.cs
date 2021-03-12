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
    }
}