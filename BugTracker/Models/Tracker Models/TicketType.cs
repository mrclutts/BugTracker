using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Tracker_Models
{
    public class TicketType
    {
        public int Id { get; set; }

        [Display(Name="Ticket Type")]
        public string Name { get; set; }
    }
}