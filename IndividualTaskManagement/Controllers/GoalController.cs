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

        //[Authorize(Roles = "teacher")]
        public ActionResult Index()
        {
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

    }
}