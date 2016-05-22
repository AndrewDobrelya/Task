using IndividualTaskManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;


namespace IndividualTaskManagement.Controllers
{
    public class GoalController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Task

        [Authorize(Roles = "teacher")]
        public ActionResult Index()
        {
              List<string> sList = new List<string>();
            //int i = 0;
            #region Nowork
            //var userId = new ApplicationDbContext().Users;
            //var rolesId = new ApplicationDbContext().Roles.Where(p => p.Name == "student").SelectMany(p => p.Users);
         
            //var student = from f in rolesId join h in userId on f.UserId equals h.Id select new { h.Id, h.FirstName, h.LastName };
            //var student2 = from f2 in db.Users join h2 in db.Roles.Where(p => p.Name == "student").SelectMany(p => p.Users) on f2.Id equals h2.RoleId select new { h2.UserId, f2.LastName };
            #endregion

            #region Nowork2
            //var student3 = new ApplicationDbContext().Roles.Where(p => p.Name == "student").SelectMany(p => p.Users).ToList();

            //var usw = new ApplicationDbContext().Users.Where(p => p.Id == student3[0].UserId).SelectMany(p => p.UserName);
            //var usw2 = new ApplicationDbContext().Users.Where(p => p.Id == student3[0].UserId).SelectMany(p => p.Roles);
            //var usw3 = new ApplicationDbContext().Users.Where(p => p.Id == student3[0].UserId).SelectMany(p => p.Id);
            #endregion

            #region Nowork3
            //for (int i = 0; i < student.Count(); i++)
            //{
            //    sList.Add(student[i].UserId);
            //}
            //var sqwe = new ApplicationDbContext().Users.Where(p => p.Id == student[0].UserId);

            //for (int i = 0; i < sList.Count(); i++)
            //{
            //    sqwe = new ApplicationDbContext().Users.Where(p => p.Id == sList[i]);
            //}

            //for (int i = 0; i < student.Count(); i++)
            //{
            //   var sss = new ApplicationDbContext().Users.Where(p => p.Id == student[i].UserId).SelectMany(p => p.Roles);
            //    int sqwe = 0;
            //    sqwe++;
            //}

            //var s = "sds";

            //var s = student[1];
            //var s2 = student[0];
            #endregion

            string user = User.Identity.GetUserId();
            var goal = db.Goal.Include(l => l.Author).Where(l => l.Author.Id == user);           
            return View(goal.ToList());
        }

        [Authorize(Roles = "teacher")]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AddGoalModel goalview)
        {
            if (ModelState.IsValid)
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var goal = new Goal() { Name = goalview.name, Subject = db.Subject.Find(goalview.subject_id)};
                goal.StartDate = DateTime.Now;
                goal.Author = userManager.FindById(User.Identity.GetUserId());
                goal.Completeness = 0;
                db.Goal.Add(goal);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(goalview);
        }

        [Authorize(Roles = "teacher")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Goal goal = db.Goal.Find(id);            
            if (goal == null)
            {
                return HttpNotFound();
            }
            return View(new AddGoalModel(goal));
        }



        [Authorize(Roles = "teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AddGoalModel goalview)
        {
            if (ModelState.IsValid)
            {
                var goal = db.Goal.Find(goalview.id);
                goal.Name = goalview.name;                
                goal.Subject = db.Subject.Find(goalview.subject_id);
                

                db.Entry(goal).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(goalview);
        }

        [Authorize(Roles = "teacher, admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Goal goal = db.Goal.Find(id);
            if (goal == null)
            {
                return HttpNotFound();
            }
            return View(goal);
        }



        [Authorize(Roles = "teacher, admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Goal goal = db.Goal.Find(id);
            db.Goal.Remove(goal);
            db.SaveChanges();
            if (User.IsInRole("admin"))
            {

                return RedirectToAction("List");
            }
            else
            {

                return RedirectToAction("Index");
            }
        }

        private bool IsNeededTeacher(string goalId)
        {
            var task = User.Identity.GetUserId();
            return task == goalId;
        }

        [Authorize(Roles = "teacher")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Goal goal = db.Goal.Find(id);
            ViewBag.Teacher = IsNeededTeacher(goal.Author.Id);
            if (goal == null)
            {
                return HttpNotFound();
            }
            return View(goal);
        }

    }
}