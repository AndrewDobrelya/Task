using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using IndividualTaskManagement.Models;

namespace IndividualTaskManagement.Controllers
{
    public class SubjectController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Subject
        public ActionResult Index()
        {
            return View(db.Subject.ToList());
        }

        // GET: Subject/Create
        public ActionResult Create()
        {
            return View();  
        }

        // POST: Subject/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Subject subject)
        {
            if (ModelState.IsValid)
            {
                db.Subject.Add(subject);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(subject);
        }

        // GET: Subject/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subject subject = db.Subject.Find(id);
            if (subject == null)
            {
                return HttpNotFound();
            }
            return View(subject);
        }

        // POST: Subject/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Subject subject)
        {
            if (ModelState.IsValid)
            {
                db.Entry(subject).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(subject);
        }
    
        // GET: Subject/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subject subject = db.Subject.Find(id);
            if (subject == null)
            {
                return HttpNotFound();
            }
            else
            {
                db.Subject.Remove(subject);
                db.SaveChanges();
                return RedirectToAction("Index");
            }            
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
