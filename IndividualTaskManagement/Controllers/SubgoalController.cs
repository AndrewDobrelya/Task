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
            var folder = Server.MapPath("/Files/" + id + "");
            try
            {
                ViewBag.CountFiles = new DirectoryInfo(folder).GetFiles().Length;
            }
            catch (DirectoryNotFoundException)
            {
                ViewBag.CountFiles = 0;
                //For successful entering to repository
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

            try
            {
                string[] files = Directory.GetFiles(Server.MapPath("/Files/" + subgoalId + ""));
                string[] dates = new string[files.Count()];
                string[] length = new string[files.Count()];
                for (int i = 0; i < files.Length; i++)
                {
                    FileInfo oFileInfo = new FileInfo(files[i]);
                    dates[i] = oFileInfo.CreationTime.ToString();
                    if (oFileInfo.Length >= (1 << 20))
                    {
                        length[i] = string.Format("{0} megabytes", oFileInfo.Length >> 20);
                    }
                    else if (oFileInfo.Length >= (1 << 10))
                    {
                        length[i] = string.Format("{0} kilobytes", oFileInfo.Length >> 10);
                    }
                    else if (oFileInfo.Length < (1 << 10))
                    {
                        length[i] = string.Format("{0} bytes", oFileInfo.Length);
                    }
                                  
                    files[i] = Path.GetFileName(files[i]);
                }
                ViewBag.Files = files;
                ViewBag.Dates = dates;
                ViewBag.Length = length;
                ViewBag.id = subgoalId;
            }
            catch (DirectoryNotFoundException)
            {
                //For successful entering to repository
            }
            return View(subgoal);
        }


        public FileResult DownloadFile(string fileName, int subgoalId)
        {
            var filepath = Path.Combine(Server.MapPath("/Files/" + subgoalId + "/"), fileName);
            return File(filepath, MimeMapping.GetMimeMapping(filepath), fileName);
        }

        public ActionResult DeleteFile(string fileName, int subgoalId)
        {
            var filepath = Path.Combine(Server.MapPath("/Files/" + subgoalId + "/"), fileName);
            FileInfo oFileInfo = new FileInfo(filepath);
            oFileInfo.Delete();
            return RedirectToAction("Download/", new {subgoalId = subgoalId } );
        } 

        private void UpdateComletness(Subgoal subgoal , bool isDelete)
        {
            if (isDelete)
            {

            }
            var goal = db.Goal.Find(subgoal.Goal.Id);            
            var subgoals = db.Subgoal.Where(s => s.Goal.Id == subgoal.Goal.Id);
            int countsubgoal = subgoals.Count();       
            var complite = subgoals.Where(s => s.AtTerm == true);
            int countCoplite = complite.Count();
            if (isDelete)
            {
                if (subgoal.AtTerm == true)
                {
                    countCoplite = countCoplite - 1;
                }
               countsubgoal = countsubgoal - 1;
               
            }
            if (countCoplite == 0)
            {
                goal.Completeness = 0;
                goal.Name = goal.Name;
               
            }
            else
            {
                int copleteness = 100 / countsubgoal * countCoplite;
                goal.Completeness = copleteness;
                goal.Name = goal.Name;
            }      
                      
            db.Entry(goal).State = EntityState.Modified;
            db.SaveChanges();
        }

        private void SendSMSNotification(Subgoal subgoal)
        {
            if (UserManager.SmsService != null && subgoal.Student.PhoneNumber != null)
            {
                var message = new IdentityMessage
                {
                    Destination = subgoal.Student.PhoneNumber,
                    Body = "Your security code is: "
                };
                UserManager.SmsService.Send(message);
            }
        }

        [Authorize(Roles = "teacher")]
        public ActionResult CreateSubgoal()
        {
            
        
            var users = new ApplicationDbContext().Users;
            var rolesIdToUser = new ApplicationDbContext().Roles.Where(p => p.Name == "student").SelectMany(p => p.Users).ToList();
            var students = rolesIdToUser.Select(i => users.FirstOrDefault(u => u.Id == i.UserId)).ToList();
            ViewBag.Students = from student in students select new { Id = student.Id, FullName = student.FirstName + " " + student.LastName };
            
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
                subgoal.AtTerm = false;
                db.Subgoal.Add(subgoal);
                db.SaveChanges();
                UpdateComletness(subgoal, false);
                SendSMSNotification(subgoal);
                return RedirectToAction("Details/" + subgoal.Goal.Id, "Goal");
            }
            return View(subgoalview);            
        }

        public ActionResult SubgoalList(int goalId)
        {
            List<Subgoal> subgoal = new List<Subgoal>();
            ViewBag.GoalId = goalId;
           
            foreach (var item in db.Subgoal.ToList())
            {
                if (item.Goal.Id == goalId)
                {
                    subgoal.Add(item);
                   
                }
            }
            return View(subgoal);
        }

        public ActionResult SubgoalFinish(int id)
        {
            var subgoal = db.Subgoal.FirstOrDefault(s => s.Id == id);
            
            subgoal.AtTerm = true;

            SendSMSNotification(subgoal);
            db.SaveChanges();
            UpdateComletness(subgoal , false);
            return RedirectToAction("Details/" + subgoal.Goal.Id, "Goal");

        }

        [Authorize(Roles = "teacher")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subgoal subgoal = db.Subgoal.Find(id);           
            ViewBag.StudentName = subgoal.Student.FirstName +" "+ subgoal.Student.LastName;
            ViewBag.id = subgoal.Goal.Id;
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
                
                //subgoal.Student = db.Users.Find(subgoalview.id);
                subgoal.Name = subgoalview.name;
                subgoal.Description = subgoalview.description;
                //subgoal.AtTerm = subgoalview.atTerm;
                subgoal.EndDate = subgoalview.endDate;
                db.Entry(subgoal).State = EntityState.Modified;
                db.SaveChanges();              
                return RedirectToAction("Details/" + subgoal.Goal.Id,"Goal");
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
            int s = subgoal.Goal.Id;
            UpdateComletness(subgoal, true);
            db.Subgoal.Remove(subgoal);       
            db.SaveChanges();           
            if (User.IsInRole("admin"))
            {

                return RedirectToAction("Details/" + s, "Goal");
            }
            else
            {

                return RedirectToAction("Details/" + s, "Goal");
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