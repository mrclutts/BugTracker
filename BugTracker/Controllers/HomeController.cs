using BugTracker.Helpers;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            ViewBag.ReturnUrl = "";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Dashboard()
        {
            ProjectsHelper helper = new ProjectsHelper();
            var name = db.Users.Find(User.Identity.GetUserId());
            ViewBag.name = name.DisplayName;

            var users = db.Users.Count();
            var projectTotal = db.Project.Count();
            var count = db.Ticket.Count();
            ViewBag.users = users;
            ViewBag.tickets = count;
            ViewBag.projects = projectTotal;

            double openTickets = db.Ticket.Where(t => t.TicketStatus.Name == "Open").Count();         
            var ticketCount = openTickets / count;
            ViewBag.ticketPercent = Math.Round(ticketCount, 2) * 100;


            double projectCount = db.Project.Where(u => u.Users.Count() == 0).Count();
            var projectPercent = projectCount / projectTotal;
            ViewBag.projectPercent = Math.Round(projectPercent, 2) * 100;

            
            //ViewBag.unassignedUsers = db.Users.Where(p=>p.Projects.Count()==0).Count();
            
            double unassignedUsers = db.Users.Where(p => p.Projects.Count() == 0).Count();
            ViewBag.unassignedUsers = unassignedUsers;
            var percentage = unassignedUsers / users;
            ViewBag.usersPercentage = Math.Round(percentage, 2) * 100;

            ViewBag.usersProjs = name.Projects.Count();
            double userProjects = name.Projects.Count();
            var userPer = userProjects / projectTotal;
            if (double.IsNaN(userPer))
            {
                ViewBag.userProjects = 0;
            }
            else {
                ViewBag.userProjects = Math.Round(userPer, 2) * 100;
            }


            ViewBag.devTickets = db.Ticket.Where(t => t.AssigneeId == name.Id).Count();
            double dev = db.Ticket.Where(t => t.AssigneeId == name.Id).Count();
            var devTick = db.Ticket.Where(u => u.TicketStatus.Name == "Open").Where(x => x.AssigneeId == name.Id).Count();
            var devPer = devTick / dev;
            if (double.IsNaN(devPer))
            {
                ViewBag.devPer = 0;
            }
            else {
                ViewBag.devPer = Math.Round(devPer, 2) * 100;
            }

            ViewBag.pmTickets = name.Projects.SelectMany(t => t.Tickets).Count();
            double pmTicks = name.Projects.SelectMany(t => t.Tickets).Count();
            var pm = name.Projects.SelectMany(t => t.Tickets).Where(s=>s.TicketStatus.Name=="Open").Count();
            var pmPer = pm / pmTicks;
            if (double.IsNaN(pmPer))
            {
                ViewBag.pmPer = 0;
            }
            else {
                ViewBag.pmPer = Math.Round(pmPer, 2) * 100;
            }

            ViewBag.submitterTickets = db.Ticket.Where(t => t.OwnerId == name.Id).Count();
            double sub = db.Ticket.Where(t => t.OwnerId == name.Id).Count();
            var subTick = db.Ticket.Where(u => u.TicketStatus.Name == "Open").Where(x => x.OwnerId == name.Id).Count();
            var subPer = subTick / sub;
            if (double.IsNaN(subPer))
            {
                ViewBag.subPer = 0;
            }
            else {
                ViewBag.subPer = Math.Round(subPer, 2) * 100;
            }

            ViewBag.unassignedTicks = db.Ticket.Where(t => t.AssigneeId == null).Count();
           
            return View();
        }
    }
}