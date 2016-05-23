using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Microsoft.Owin.Security;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using IndividualTaskManagement.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;

namespace IndividualTaskManagement.Controllers
{
    public class SubgoalController : Controller
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        private ApplicationDbContext db = new ApplicationDbContext();
       
        // GET: Subgoal
        [Authorize(Roles = "student")]
        public ActionResult Index()
        {
            string user = User.Identity.GetUserId();
            var subgoal = db.Subgoal.Include(s => s.Student).Where(s => s.Student.Id == user);
            return View(subgoal.ToList());
        }

        //private bool IsNeededStudent(string subgoalId)
        //{
        //    var subgoal = User.Identity.GetUserId();
        //    return subgoal == subgoalId;
        //}

        [Authorize(Roles = "student, teacher")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Subgoal subgoal = db.Subgoal.Find(id);
            if (subgoal.EndDate < DateTime.Now)
            {
                subgoal.Overdue = true;
            }
            ViewBag.IsOverdue = subgoal.Overdue;
            //ViewBag.Student = IsNeededStudent(subgoal.Student.Id);
            if (subgoal == null)
            {
                return HttpNotFound();
            }
            return View(subgoal);
        }


        [HttpPost]
        public ActionResult Details(int id)
        {
            Subgoal subgoal = db.Subgoal.Find(id);
            if (subgoal.EndDate < DateTime.Now)
            {
                subgoal.Overdue = true;
            }
       
            ViewBag.IsOverdue = subgoal.Overdue;
            if (HttpContext.Request.Files.AllKeys.Any())
            {
                for (int i = 0; i <= HttpContext.Request.Files.Count; i++)
                {
                    var file = HttpContext.Request.Files["files" + i];
                    if (file != null)
                    {
                        var folder = Server.MapPath("/Files/" + id + "");
                        var fileSavePath = Path.Combine(folder, file.FileName);
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        file.SaveAs(fileSavePath);
                    }
                }
            }
            return View(subgoal);
        }


        public ActionResult Download(int? subgoalId)
        {
            Subgoal subgoal = db.Subgoal.Find(subgoalId);
            string[] files = Directory.GetFiles(Server.MapPath("/Files/"+subgoalId+""));
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.GetFileName(files[i]);
            }
            ViewBag.Files = files;
            return View(subgoal);
        }


        public FileResult DownloadFile(string fileName, int subgoalId)
        {
            var filepath = System.IO.Path.Combine(Server.MapPath("/Files/" + subgoalId + "/"), fileName);
            return File(filepath, MimeMapping.GetMimeMapping(filepath), fileName);
        }


        [Authorize(Roles = "teacher")]
        public ActionResult CreateSubgoal()
        {
            return View();
        }

        [Authorize(Roles = "teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateSubgoal( CreateSubgoalModel subgoalview, int id)
        {
            if (ModelState.IsValid)
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
           

                var subgoal = new Subgoal() { Name = subgoalview.name, Student = db.Users.Find(subgoalview.student_id), Description = subgoalview.description, EndDate = subgoalview.endDate };
                subgoal.Goal = db.Goal.First(c => c.Id == id);
                subgoal.Overdue = false;
                subgoal.AtTerm = false;
                db.Subgoal.Add(subgoal);
                db.SaveChanges();
                if (UserManager.SmsService != null)
                {
                    var message = new IdentityMessage
                    {
                        Destination = subgoal.Student.PhoneNumber,
                        Body = "Your security code is: " 
                    };
                    await UserManager.SmsService.SendAsync(message);
                }
                //await UserManager.SendEmailAsync(subgoal.Student.Id, "Confirm your account", "Please confirm your account by clicking");
                return RedirectToAction("Index", "Goal");
            }
            return View(subgoalview);            
        }

        public ActionResult SubgoalList(int goalId)
        {
            List<Subgoal> subgoal = new List<Subgoal>();
            ViewBag.GoalId = goalId;
           
            foreach (var t in db.Subgoal.ToList())
            {
                if (t.Goal.Id == goalId)
                {
                    subgoal.Add(t);
                   
                }
            }
            return View(subgoal);
        }

        [Authorize(Roles = "teacher")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subgoal subgoal = db.Subgoal.Find(id);
          
            if (subgoal == null)
            {
                return HttpNotFound();
            }
            return View(new EditSubgoalModel(subgoal));
        }

        [Authorize(Roles = "teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditSubgoalModel subgoalview)
        {
            if (ModelState.IsValid)
            {
                var subgoal = db.Subgoal.Find(subgoalview.id);
                subgoal.Name = subgoalview.name;
                subgoal.Description = subgoalview.description;
                subgoal.AtTerm = subgoalview.atTerm;
                subgoal.EndDate = subgoalview.endDate;
                db.Entry(subgoal).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details");
            }
            return View(subgoalview);
        }

        [Authorize(Roles = "teacher, admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subgoal subgoal = db.Subgoal.Find(id);          
            if (subgoal == null)
            {
                return HttpNotFound();
            }
            return View(subgoal);
        }



        [Authorize(Roles = "teacher, admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Subgoal subgoal = db.Subgoal.Find(id);
            db.Subgoal.Remove(subgoal);       
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


        public ActionResult Comment(int subgoalId)
        {
            ViewBag.SubgoalId = subgoalId;
            return View();
        }

        public ActionResult CommentList(int subgoalId)
        {
            Subgoal subgoal = db.Subgoal.First(s => s.Id == subgoalId);
            string userId = User.Identity.GetUserId();
            ViewBag.UserId = userId;
            ViewBag.NeededTeacher = false;

            if (userId == subgoal.Goal.Author.Id)
            {
                ViewBag.NeededTeacher = true;
            }

            List<Comment> comments = db.Comment.Where(c => c.subgoal.Id == subgoalId && c.previosComment == null).ToList<Comment>();

            List<Comment> teacherComments = db.Comment.Where(c => c.subgoal.Id == subgoalId && c.previosComment != null).ToList<Comment>();

            ViewBag.CommentList = comments;
            ViewBag.TeacherComments = teacherComments;

            return View();
        }

        public ActionResult DeleteComment(int commentId)
        {
            var com = db.Comment.First(c => c.id == commentId);
            int subgoalId = com.subgoal.Id;
            db.Comment.Remove(com);

            List<Comment> tComments = db.Comment.Where(tc => tc.previosComment != null && tc.previosComment.id == commentId).ToList<Comment>();

            if (tComments.Count > 0)
            {
                foreach (var c in tComments)
                {
                    db.Comment.Remove(c);
                }
            }

            db.SaveChanges();
            return Redirect("Details/" + subgoalId);
        }

        [HttpPost]
        public ActionResult AddComment(string commentText, int subgoalId)
        {
            var fSubgoal = db.Subgoal.First(c => c.Id == subgoalId);
            string userId = User.Identity.GetUserId();
            var u = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            db.Comment.Add(new Comment { text = commentText, subgoal = fSubgoal, user = u.FindById(userId), date = DateTime.Now, });
            db.SaveChanges();


            return Redirect("Details/" + subgoalId);
        }

        public ActionResult AddTeacherComment(string commentText, int subgoalId, int prevComment)
        {
            var fSubgoal = db.Subgoal.First(c => c.Id == subgoalId);
            string userId = User.Identity.GetUserId();
            var u = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var comment = db.Comment.First(c => c.id == prevComment);
            db.Comment.Add(new Comment { text = commentText, previosComment = comment, subgoal = fSubgoal, user = u.FindById(userId), date = DateTime.Now, });
            db.SaveChanges();


            return Redirect("Details/" + subgoalId);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}