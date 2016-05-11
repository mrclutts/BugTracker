﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Tracker_Models
{
    public class TicketComment
    {
        public int Id { get; set; }

        public int TicketId { get; set; }

        public string Description { get; set; }

        public DateTimeOffset Created { get; set; }

        public string UserId { get; set; }

        public DateTimeOffset Updated { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual Ticket Ticket { get; set; }

    }
}