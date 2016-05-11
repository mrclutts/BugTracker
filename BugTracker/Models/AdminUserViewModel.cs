using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class AdminUserViewModel
    {
        public string Name { get; set; }

        public string Id { get; set; }

        public MultiSelectList Roles {get; set;}

        public ApplicationUser User { get; set; }

        public string[] SelectedRoles { get; set; }

    }
}