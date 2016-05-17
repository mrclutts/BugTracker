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
using System.Web.Security;
using Microsoft.AspNet.Identity;
using BugTracker.Helpers;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
    [RequireHttps]
    [Authorize]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        public ActionResult AllTickets()
        {
            var ticket = db.Ticket.Include(t => t.Assignee).Include(t => t.Owner).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            return View(ticket.ToList());
        }
        // GET: Tickets
        [Authorize]
        public ActionResult Tickets()
        {

            var user = db.Users.Find(User.Identity.GetUserId());
            UserRolesHelper rolehelper = new UserRolesHelper(db);
            ProjectsHelper projecthelper = new ProjectsHelper();

            if (rolehelper.IsUserInRole(user.Id, "Admin"))
            {
                var ticket = db.Ticket.Include(t => t.Assignee).Include(t => t.Owner).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
                return View(ticket.ToList());
            }
            else if (rolehelper.IsUserInRole(user.Id, "ProjectManager"))
            {
                //List<Ticket> tickets = new List<Ticket>();
                //foreach (var proj in user.Projects.ToList())
                //{                  
                //    if (projecthelper.IsUserAssigned(user.Id, proj.Id))
                //    {
                //        tickets.AddRange(proj.Tickets);                    
                //    }                 
                //}
                //var tickets = db.Project.Where(z => z.ProjectManagerId == user.Id).SelectMany(p => p.Tickets).ToList();
                var tickets = user.Projects.SelectMany(p => p.Tickets).ToList();
                return View(tickets);
            }
            else if (rolehelper.IsUserInRole(user.Id, "Developer"))
            {
                var ticket = db.Ticket.Include(t => t.Assignee).Include(t => t.Owner).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).Where(t => t.AssigneeId == user.Id);
                return View(ticket.ToList());
            }
            else if (rolehelper.IsUserInRole(user.Id, "Submitter"))
            {
                var ticket = db.Ticket.Include(t => t.Assignee).Include(t => t.Owner).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).Where(t => t.OwnerId == user.Id);
                return View(ticket.ToList());
            }
            else {
                var ticket = db.Ticket.Include(t => t.Assignee).Include(t => t.Owner).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).Where(t => t.OwnerId == user.Id);
                return View(ticket);
            }
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Ticket.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Tickets/Create
        public ActionResult Create()
        {
            ViewBag.AssigneeId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriority, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerId,AssigneeId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.Created = System.DateTimeOffset.Now;
                var user = db.Users.Find(User.Identity.GetUserId());
                ticket.OwnerId = user.Id;

                ticket.TicketStatusId = db.TicketStatus.FirstOrDefault(x => x.Name == "Open").Id;
                //ticket.TicketStatusId = db.TicketStatus.Where(x => x.Name == "Open").Select(i=>i.Id).First();
                ProjectsHelper pjhelp = new ProjectsHelper();
                pjhelp.AddUserToProject(user.Id, ticket.ProjectId);
                db.Ticket.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Tickets");
            }

            ViewBag.AssigneeId = new SelectList(db.Users, "Id", "FirstName", ticket.AssigneeId);
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerId);
            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriority, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Ticket.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.AssigneeId = new SelectList(db.Users.Where(x => x.Roles.Select(y=>y.RoleId).Contains (db.Roles.FirstOrDefault(z=>z.Name == "Developer").Id)).ToList(), "Id", "FirstName", ticket.AssigneeId);
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerId);
            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriority, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerId,AssigneeId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                var userid = User.Identity.GetUserId();
                var changed = System.DateTimeOffset.Now;
                var editId = Guid.NewGuid().ToString();
                var oldTic = db.Ticket.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);
                var project = ticket.ProjectId;

                if(oldTic?.Title != ticket.Title)
                {
                    TicketHistory th1 = new TicketHistory
                    {
                        TicketId = ticket.Id,
                        Property = "Title",
                        OldValue = oldTic?.Title,
                        NewValue = ticket.Title,
                        Changed = changed,
                        UserId = userid
                    };
                    db.TicketHistory.Add(th1);
                }
                if (oldTic?.Description != ticket.Description)
                {
                    TicketHistory th2 = new TicketHistory
                    {
                        TicketId = ticket.Id,
                        Property = "Description",
                        OldValue = oldTic?.Description,
                        NewValue = ticket.Description,
                        Changed = changed,
                        UserId = userid
                    };
                    db.TicketHistory.Add(th2);
                }
                
                if (oldTic?.AssigneeId != ticket.AssigneeId)
                {
                    string oldAssignee = "";
                    if(oldTic.AssigneeId != null){
                        oldAssignee = oldTic?.Assignee.DisplayName;
                    }
                    else
                    {
                        oldAssignee = null;
                    }
                    string newAssignee = db.Users.Find(ticket.AssigneeId).DisplayName;
                    TicketHistory th3 = new TicketHistory
                    {
                        TicketId = ticket.Id,
                        Property = "Assignee",
                        OldValue = oldAssignee,
                        NewValue = newAssignee,
                        Changed = changed,
                        UserId = userid
                    };
                    ProjectsHelper pjhelp = new ProjectsHelper();
                    pjhelp.AddUserToProject(ticket.AssigneeId, ticket.ProjectId);
                    
                    db.TicketHistory.Add(th3);
                }
                if (oldTic?.TicketTypeId != ticket.TicketTypeId)
                {
                    string oldTicketType = oldTic.TicketType.Name;
                    string newTicketType = db.TicketType.Find(ticket.TicketTypeId).Name;

                    TicketHistory th4 = new TicketHistory
                    {
                        TicketId = ticket.Id,
                        Property = "Ticket Type",
                        OldValue = oldTicketType,
                        NewValue = newTicketType,
                        Changed = changed,
                        UserId = userid
                    };
                    db.TicketHistory.Add(th4);
                }
                if (oldTic?.TicketStatusId != ticket.TicketStatusId)
                {
                    //var oldTicketStatus = db.TicketStatus.FirstOrDefault(x => x.Id == oldTic.TicketStatusId);
                    var oldTicketStatus = oldTic.TicketStatus.Name;
                    var newTicketStatus = db.TicketStatus.Find(ticket.TicketStatusId).Name;
                    TicketHistory th5 = new TicketHistory
                    {
                        TicketId = ticket.Id,
                        Property = "Ticket Status",
                        OldValue = oldTicketStatus,
                        NewValue = newTicketStatus,
                        Changed = changed,
                        UserId = userid
                    };
                    db.TicketHistory.Add(th5);
                }
                if (oldTic?.TicketPriorityId != ticket.TicketPriorityId)
                {
                    string oldTicketPriority = oldTic.TicketPriority.Name;
                    string newTicketPriority = db.TicketPriority.Find(ticket.TicketPriorityId).Name;
                    TicketHistory th6 = new TicketHistory
                    {
                        TicketId = ticket.Id,
                        Property = "Ticket Priority",
                        OldValue =oldTicketPriority,
                        NewValue = newTicketPriority,
                        Changed = changed,
                        UserId = userid
                    };
                    db.TicketHistory.Add(th6);
                }

                

                //db.Entry(ticket).State = EntityState.Modified;
                db.Ticket.Attach(ticket);
                ticket.Updated = System.DateTimeOffset.Now;
                db.Entry(ticket).Property("Title").IsModified = true;
                db.Entry(ticket).Property("Description").IsModified = true;
                db.Entry(ticket).Property("AssigneeId").IsModified = true;
                db.Entry(ticket).Property("Updated").IsModified = true;
                db.Entry(ticket).Property("TicketTypeId").IsModified = true;
                db.Entry(ticket).Property("TicketStatusId").IsModified = true;
                db.Entry(ticket).Property("TicketPriorityId").IsModified = true;
                db.SaveChanges();

                

                EmailService es = new EmailService();
                IdentityMessage im = new IdentityMessage();
                im.Subject = $"You have been assigned to {ticket.Title}";
                im.Destination = db.Users.Find(ticket.AssigneeId).Email;
                var callbackUrl = Url.Action("Details", "Tickets", new { id = ticket.Id }, protocol: Request.Url.Scheme);
                im.Body = "Hi " + ticket.Assignee.DisplayName + "," + "<br/> A ticket in your queue has been edited! Details Below:<br/>" + ticket.Description + "View the ticket here: <a href=\"" + callbackUrl + "\">here!</a>";
                await es.SendAsync(im);

                return RedirectToAction("Details", "Tickets", new { id = ticket.Id });
            }
            ViewBag.AssigneeId = new SelectList(db.Users, "Id", "FirstName", ticket.AssigneeId);
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerId);
            ViewBag.ProjectId = new SelectList(db.Project, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriority, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Ticket.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Ticket.Find(id);
            db.Ticket.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
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
