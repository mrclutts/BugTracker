using BugTracker.Helpers;
using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace BugTracker.Controllers
{
    [RequireHttps]
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        //Get Users/Index
        public ActionResult Users()
        {
            UserRolesHelper helper = new UserRolesHelper(db);
            List<RoleAdminViewModel> model = new List<RoleAdminViewModel>();
            foreach (var user in db.Users.ToList())
            {
                RoleAdminViewModel RoleModel = new RoleAdminViewModel();
                RoleModel.Role = helper.ListUserRoles(user.Id).ToList();
                RoleModel.User = user;
                model.Add(RoleModel);
            }
            return View(model);
        }

        //Get Edit/UserRole
        public ActionResult EditUser(string id)
        {
            var user = db.Users.Find(id);
            AdminUserViewModel AdminModel = new AdminUserViewModel();
            UserRolesHelper helper = new UserRolesHelper(db);
            var selected = helper.ListUserRoles(id);
            AdminModel.Roles = new MultiSelectList(db.Roles, "Name", "Name", selected);
            AdminModel.User = user;

            return View(AdminModel);
        }

        //Post EditUser
        [HttpPost]
        public ActionResult EditUser(AdminUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                UserRolesHelper helper = new UserRolesHelper(db);
                foreach (var r in model.SelectedRoles)
                {
                    if (!helper.IsUserInRole(model.Id, r))
                    {
                        helper.AddUserToRole(model.Id, r);
                    }
                }
                foreach(var r in db.Roles.ToList())
                {
                    if(!model.SelectedRoles.Contains(r.Name))
                    {
                        helper.RemoveUserFromRole(model.Id, r.Name);
                    }
                }
                
                db.SaveChanges();
            }
            return RedirectToAction("Users");
        }
        //Get Admin/Admin
        public ActionResult Admin()
        {
            return View();
        }
    }
}