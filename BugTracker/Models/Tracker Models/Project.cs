using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Tracker_Models
{
    public class Project
    {
        public Project()
        {
            Users = new HashSet<ApplicationUser>();
        }

        public int Id { get; set; }

        [Display(Name="Project Name")]
        public string Name { get; set; }

        public DateTimeOffset Created { get; set; }

        [Display(Name="End Date")]
        public DateTimeOffset EndDate { get; set; }

        public string Description { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}