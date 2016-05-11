using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using BugTracker.Models.Tracker_Models;
using BugTracker.Helpers;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    [RequireHttps]
    
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //Get:AllProjects 
        [Authorize]
        public ActionResult AllProjects()
        {
            return View(db.Project.ToList());
        }

        // GET: Projects
        [Authorize]
        public ActionResult Projects()
        {
            
            var user = db.Users.Find(User.Identity.GetUserId());
            UserRolesHelper rolehelper = new UserRolesHelper(db);
            if (rolehelper.IsUserInRole(user.Id,"Admin"))
            {
                return View(db.Project.ToList());
            }
            if(rolehelper.IsUserInRole(user.Id, "ProjectManager"))
            {
                //db.Project.Where(z => z.ProjectManagerId == user.Id).ToList()
                return View(user.Projects.ToList());
            }
            else 
            {
                return View(user.Projects.ToList());
            }
        }

        // GET: Projects/Details/5
        [Authorize(Roles = "Admin,ProjectManager,Developer,Submitter")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            //if (!(project.Users.Count == 0))
            //{
                ViewBag.ProjectManager = project.Users.FirstOrDefault(r => r.Roles.Select(y => y.RoleId).Contains(db.Roles.FirstOrDefault(z => z.Name == "ProjectManager").Id))?.DisplayName;

            //}
            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles ="Admin,ProjectManager")]
        public ActionResult Create()
        {
            //ViewBag.ProjectManagerId = new SelectList(db.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(db.Roles.FirstOrDefault(z => z.Name == "ProjectManager").Id)).ToList(), "Id", "FirstName");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin,ProjectManager")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Created,EndDate,Description")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.Created = System.DateTimeOffset.Now;
                
                db.Project.Add(project);
                db.SaveChanges();
                return RedirectToAction("Projects");
            }
            //ViewBag.ProjectManagerId = new SelectList(db.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(db.Roles.FirstOrDefault(z => z.Name == "Developer").Id)).ToList(), "Id", "FirstName", project.ProjectManager);
            return View(project);
        }
        [Authorize(Roles = "Admin")]
        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            //ViewBag.ProjectManagerId = new SelectList(db.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(db.Roles.FirstOrDefault(z => z.Name == "ProjectManager").Id)).ToList(), "Id", "FirstName", project.ProjectManager);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin,ProjectManager")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Created,EndDate,Description")] Project project)
        {
            if (ModelState.IsValid) {

                db.Project.Attach(project);
                //db.Entry(project).State = EntityState.Modified;
                db.Entry(project).Property("Name").IsModified = true;
                
                db.Entry(project).Property("EndDate").IsModified = true;
                
                db.Entry(project).Property("Description").IsModified = true;
                //db.Entry(project).Property("ProjectManagerId").IsModified = true;
                db.SaveChanges();
                return View(project);

            }
            //ViewBag.ProjectManagerId = new SelectList(db.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(db.Roles.FirstOrDefault(z => z.Name == "Developer").Id)).ToList(), "Id", "FirstName", project.ProjectManager);
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Project.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Project.Find(id);
            db.Project.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Projects");
        }

        //Get: Projects/AssignUsers
        [Authorize(Roles ="Admin")]
        public ActionResult AssignUsers(int id)
        {
            var project = db.Project.Find(id);
            ProjectsHelper helper = new ProjectsHelper();
            ProjectAdminViewModel ProjectAdmin = new ProjectAdminViewModel();
            var selected = helper.ListProjectUsers(id).Select(p=>p.Id).ToList();
            ProjectAdmin.Users = new MultiSelectList(db.Users, "Id", "DisplayName", selected);
            ProjectAdmin.Project = project;
            return View(ProjectAdmin);
        }
        //Post: Projects/AssignUsers
        [Authorize(Roles ="Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignUsers (ProjectAdminViewModel model)
        {
            if (ModelState.IsValid)
            {
                ProjectsHelper helper = new ProjectsHelper();
                foreach(var u in model.SelectedUsers)
                {
                    helper.AddUserToProject(u, model.Project.Id);
                }
                foreach(var u in db.Users.ToList())
                {
                    if (!model.SelectedUsers.Contains(u.Id))
                    {
                        helper.RemoveUser(u.Id, model.Project.Id);
                     }
                }
                db.SaveChanges();
            }
            return RedirectToAction("Projects");
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
