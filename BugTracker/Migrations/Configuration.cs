namespace BugTracker.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "BugTracker.Models.ApplicationDbContext";
        }

        protected override void Seed(BugTracker.Models.ApplicationDbContext context)
        {

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "ProjectManager"))
            {
                var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));
                roleManager.Create(new IdentityRole { Name = "ProjectManager" });
            }
            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }
            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }
            if (!context.Users.Any(u => u.Email == "mrclutts@ncsu.edu"))
            {
                var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));
                userManager.Create(new ApplicationUser
                {
                    UserName = "mrclutts@ncsu.edu",
                    Email = "mrclutts@ncsu.edu",
                    FirstName = "Matt",
                    LastName = "Clutts",
                    DisplayName = "Matt"
                }, "Potato99!");

                var userId = userManager.FindByEmail("mrclutts@ncsu.edu").Id;
                userManager.AddToRole(userId, "Admin");
            }
            if (!context.Users.Any(u => u.Email == "HonestAbe@whitehouse.gov"))
            {
                var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));
                userManager.Create(new ApplicationUser
                {
                    UserName = "HonestAbe@whitehouse.gov",
                    Email = "HonestAbe@whitehouse.gov",
                    FirstName = "Abe",
                    LastName = "Lincoln",
                    DisplayName = "Honest Abe"
                }, "Potato99!");

                var userId = userManager.FindByEmail("HonestAbe@whitehouse.gov").Id;
                userManager.AddToRole(userId, "Developer");
            }
            if (!context.Users.Any(u => u.Email == "ThomasJefferson@whitehouse.gov"))
            {
                var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));
                userManager.Create(new ApplicationUser
                {
                    UserName = "ThomasJefferson@whitehouse.gov",
                    Email = "ThomasJefferson@whitehouse.gov",
                    FirstName = "Thomas",
                    LastName = "Jefferson",
                    DisplayName = "Thomas"
                }, "Potato99!");

                var userId = userManager.FindByEmail("ThomasJefferson@whitehouse.gov").Id;
                userManager.AddToRole(userId, "Submitter");
            }
            if (!context.Users.Any(u => u.Email == "GeorgeWashington@whitehouse.gov"))
            {
                var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));
                userManager.Create(new ApplicationUser
                {
                    UserName = "GeorgeWashington@whitehouse.gov",
                    Email = "GeorgeWashington@whitehouse.gov",
                    FirstName = "George",
                    LastName = "Washington",
                    DisplayName = "George"
                }, "Potato99!");

                var userId = userManager.FindByEmail("GeorgeWashington@whitehouse.gov").Id;
                userManager.AddToRole(userId, "ProjectManager");
            }
        }
    }
}
