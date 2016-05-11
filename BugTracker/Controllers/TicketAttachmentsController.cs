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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
    [RequireHttps]
    public class TicketAttachmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Admin")]
        // GET: TicketAttachments
        public ActionResult Index()
        {
            var ticketAttachment = db.TicketAttachment.Include(t => t.Ticket).Include(t => t.User);
            return View(ticketAttachment.ToList());
        }
        [Authorize(Roles = "Admin")]
        // GET: TicketAttachments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachment.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            return View(ticketAttachment);
        }
        [Authorize(Roles = "Admin")]
        // GET: TicketAttachments/Create
        public ActionResult Create()
        {
            ViewBag.TicketId = new SelectList(db.Ticket, "Id", "Title");
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        //Upload an Image
        public static class ImageUploadValidator
        {
            public static bool IsWebFriendlyImage(HttpPostedFileBase file)
            {
                //check for actual object
                if (file == null)
                    return false;
                //check size - file must be less than 2MB and greater than 1kb
                if (file.ContentLength > 2 * 1024 * 1024 || file.ContentLength < 1024)
                    return false;
                try
                {
                    using (var img = Image.FromStream(file.InputStream))
                    {
                        return ImageFormat.Jpeg.Equals(img.RawFormat) ||
                            ImageFormat.Png.Equals(img.RawFormat) ||
                            ImageFormat.Gif.Equals(img.RawFormat);
                    }
                }
                catch
                {
                    return false;
                }
            }
        }


        // POST: TicketAttachments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Create([Bind(Include = "Id,TicketId,FilePath,Description,Created,UserId,FileUrl")] TicketAttachment ticketAttachment, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/img/ticketattachments/"), fileName));
                   ticketAttachment.FileUrl = "~/img/ticketattachments/" + fileName;
                }
                ticketAttachment.Created = System.DateTimeOffset.Now;
                ticketAttachment.User = db.Users.Find(User.Identity.GetUserId());
                db.TicketAttachment.Add(ticketAttachment);
                db.SaveChanges();

                EmailService es = new EmailService();
                IdentityMessage im = new IdentityMessage();
                im.Subject = $"New Attachment - {ticketAttachment.Description}";
                im.Destination = db.Users.Find(ticketAttachment.UserId).Email;
                var callbackUrl = Url.Action("Details", "Tickets", new { id = ticketAttachment.TicketId }, protocol: Request.Url.Scheme);
                im.Body = ticketAttachment.Description + "View the ticket here: <a href=\"" + callbackUrl + "\">here!</a>";
                await es.SendAsync(im);

                return RedirectToAction("Details", "Tickets", new { id = ticketAttachment.TicketId });
            }

            ViewBag.TicketId = new SelectList(db.Ticket, "Id", "Title", ticketAttachment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketAttachment.UserId);
            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachment.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketId = new SelectList(db.Ticket, "Id", "Title", ticketAttachment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketAttachment.UserId);
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,TicketId,FilePath,Description,Created,UserId,FileUrl")] TicketAttachment ticketAttachment, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                
                db.TicketAttachment.Attach(ticketAttachment);
                //db.Entry(ticketAttachment).State = EntityState.Modified;
                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/img/ticketattachments/"), fileName));
                    ticketAttachment.FileUrl = "~/img/ticketattachments/" + fileName;
                    db.Entry(ticketAttachment).Property("FileUrl").IsModified = true;
                    
                }
                db.Entry(ticketAttachment).Property("Description").IsModified = true;
                db.SaveChanges();

                EmailService es = new EmailService();
                IdentityMessage im = new IdentityMessage();
                im.Subject = $"Updated Attachment for {ticketAttachment.Description}";
                im.Destination = db.Users.Find(ticketAttachment.UserId).Email;
                var callbackUrl = Url.Action("Details", "Tickets", new { id = ticketAttachment.TicketId }, protocol: Request.Url.Scheme);
                im.Body = ticketAttachment.Description + "View the ticket here: <a href=\"" + callbackUrl + "\">here!</a>";
                await es.SendAsync(im);

                return RedirectToAction("Details", "Tickets", new { id = ticketAttachment.TicketId });
            }
            ViewBag.TicketId = new SelectList(db.Ticket, "Id", "Title", ticketAttachment.TicketId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", ticketAttachment.UserId);
            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachment.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketAttachment ticketAttachment = db.TicketAttachment.Find(id);
            db.TicketAttachment.Remove(ticketAttachment);
            db.SaveChanges();
            return RedirectToAction("Details", "Tickets", new { id = ticketAttachment.TicketId });
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
