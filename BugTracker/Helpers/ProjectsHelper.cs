using BugTracker.Models;
using BugTracker.Models.Tracker_Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Helpers
{
    public class ProjectsHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUser User = new ApplicationUser();
        private Project project = new Project();

        public void AddUserToProject(string userId, int projectId)
        {
            User = db.Users.Find(userId);
            project = db.Project.Find(projectId);
            if (!IsUserAssigned(userId, projectId))
            {
                project.Users.Add(User);
                db.SaveChanges();
            }           
        }
        public bool IsUserAssigned(string userId, int projectId)
        {
            var project = db.Project.FirstOrDefault(x => x.Id == projectId);
            //Any() checks to see if there is anything returned by the query
            if (project.Users.Where(x => x.Id == userId).Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void RemoveUser(string userId, int projectId)
        {
            var user = db.Users.Find(userId);
            project = db.Project.Find(projectId);
            if (IsUserAssigned(userId, projectId))
            {
                project.Users.Remove(user);
                db.SaveChanges();
            }

        }
        public List<Project> ListUserProjects(string userId)
        {
            var user = db.Users.Find(userId);
            return (user.Projects.ToList());
        }
        public ICollection<ApplicationUser> ListProjectUsers(int projectId)
        {
            var project = db.Project.Find(projectId);
            if (project.Users == null)
            {
                return (new List<ApplicationUser>());
            }
            else {
               
                return (project.Users);
            }
        }
        public List<ApplicationUser> ListUsersNotOnProjects()
        {
            var list = new List<ApplicationUser>();
            foreach(var user in db.Users.ToList().Where(p => p.Projects == null))
            {
                list.Add(user);
            }
            return (list);

        }
        public List<ApplicationUser> ListUsersNotOnProject(int projectId)
        {
            
            var list = new List<ApplicationUser>();
            foreach (var user in db.Users.ToList()) {
                if(!user.Projects.Select(u => u.Id).Contains(projectId))
                {
                    list.Add(user);
                }
            }
            return (list);
        }
       
    }
}