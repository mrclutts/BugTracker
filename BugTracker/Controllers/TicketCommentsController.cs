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
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
    [RequireHttps]
    public class TicketCommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        [Authorize(Roles = "Admin")]
        // GET: TicketComments
        public ActionResult Index()
        {
            var ticketComment = db.TicketComment.Include(t => t.Ticket).Include(t => t.User);
            return View(ticketComment.ToList());
        }
        [Authorize(Roles = "Admin")]
        // GET: TicketComments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComment.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            return View(ticketComment);
        }

        // GET: TicketComments/Create
        public ActionResult Create()
        {
            ViewBag.TicketId = new SelectList(db.Ticket, "Id", "Title");
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: TicketComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,TicketId,FilePath,Description,Created,UserId,FileUrl")] TicketComment ticketComment)
        {
            if (ModelState.IsValid)
            {
                ticketComment.User = db.Users.Find(User.Identity.GetUserId());
                ticketComment.Created = System.DateTimeOffset.Now;
                db.TicketComment.Add(ticketComment);
                db.SaveChanges();

                EmailService es = new EmailService();
                IdentityMessage im = new IdentityMessage();
                im.Subject = $"Updated Attachment for {ticketComment.Description}";
                im.Destination = db.Ticket.Find(ticketComment.TicketId).Assignee.Email;
                var callbackUrl = Url.Action("Details", "Tickets", new { id = ticketComment.TicketId }, protocol: Request.Url.Scheme);
                im.Body = "Hi " + ticketComment.Ticket.Assignee.DisplayName + ",<br/>" + ticketComment.User.DisplayName + " has made a comment on a ticket in your queue." + ticketComment.Description + "View the ticket here: <a href=\"" + callbackUrl + "\">here!</a>";
                await es.SendAsync(im);

                return RedirectToAction("Details", "Tickets", new { id = ticketComment.TicketId });
            }

            ViewBag.TicketId = new SelectList(db.Ticket, "Id", "Title", ticketComment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComment.UserId);
            return View(ticketComment);
        }

        // GET: TicketComments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComment.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketId = new SelectList(db.Ticket, "Id", "Title", ticketComment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComment.UserId);
            return View(ticketComment);
        }

        // POST: TicketComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,TicketId,FilePath,Description,Created,UserId,FileUrl")] TicketComment ticketComment)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(ticketComment).State = EntityState.Modified;
                db.TicketComment.Attach(ticketComment);
                ticketComment.Updated = System.DateTimeOffset.Now;
                db.Entry(ticketComment).Property("Description").IsModified = true;
                db.SaveChanges();

                EmailService es = new EmailService();
                IdentityMessage im = new IdentityMessage();
                im.Subject = $"Updated Attachment for {ticketComment.Description}";
                im.Destination = db.Ticket.Find(ticketComment.TicketId).Assignee.Email;
                var callbackUrl = Url.Action("Details", "Tickets", new { id = ticketComment.TicketId }, protocol: Request.Url.Scheme);
                im.Body = "A user has edited a comment on a ticket in your queue. <br/>" + ticketComment.Description + "View the ticket here: <a href=\"" + callbackUrl + "\">here!</a>";
                await es.SendAsync(im);

                return RedirectToAction("Details", "Tickets", new { id = ticketComment.TicketId });
            }
            ViewBag.TicketId = new SelectList(db.Ticket, "Id", "Title", ticketComment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketComment.UserId);
            return View(ticketComment);
        }

        // GET: TicketComments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComment.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            return View(ticketComment);
        }

        // POST: TicketComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketComment ticketComment = db.TicketComment.Find(id);
            db.TicketComment.Remove(ticketComment);
            db.SaveChanges();
            return RedirectToAction("Details", "Tickets", new { id = ticketComment.TicketId });
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
