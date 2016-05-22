using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using IndividualTaskManagement.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace IndividualTaskManagement.Controllers
{
    public class SubgoalController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        private static int goalId;
        // GET: Subgoal
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "teacher")]
        public ActionResult CreateSubgoal()
        {
            return View();
        }

        [Authorize(Roles = "teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSubgoal( CreateSubgoalModel subgoalview, int id)
        {
            if (ModelState.IsValid)
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
           

                var subgoal = new Subgoal() { Name = subgoalview.name, Student = db.Users.Find(subgoalview.student_id), Description = subgoalview.description, EndDate = subgoalview.endDate };
                subgoal.Goal = db.Goal.First(c => c.Id == id);
                subgoal.Overdue = false;
                subgoal.AtTetm = false;
                db.Subgoal.Add(subgoal);
                db.SaveChanges();
                return RedirectToAction("Index", "Goal");
            }
            return View(subgoalview);            
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult CreateSubgoal([Bind(Include = "Id")] CreateSubgoalModel subgoal, int id)
        //{
           

        //        db.Test.Add(test);
        //        db.SaveChanges();
        //        return RedirectToAction("Details", "Course", new Goal { Id = id });
        //    }

        //    return View(test);
        //}
    }
}