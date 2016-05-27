using IndividualTaskManagement.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IndividualTaskManagement.Controllers
{
    public class HomeController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult About()
        {

            ViewBag.Statuses = db.Users.Where(u => u.FirstName == "Andrew").ToList();
            ViewBag.Categories = db.Users.Where(u => u.FirstName == "Andrew").ToList();
            ViewBag.Users = db.Subgoal.Where(u => u.Name == "Moon").ToList();

            return View();
        }          

            
        
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }       
    }
}