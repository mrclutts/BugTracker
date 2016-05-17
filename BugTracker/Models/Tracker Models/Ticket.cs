using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Tracker_Models
{
    public class Ticket
    {
        public Ticket()
        {
            this.TicketComments = new HashSet<TicketComment>();
            this.TicketAttachments = new HashSet<TicketAttachment>();
            this.TicketHistories = new HashSet<TicketHistory>();
            this.TicketNotifications = new HashSet<TicketNotification>();

        }
        public virtual ICollection<TicketComment> TicketComments { get; set; }

        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }

        public virtual ICollection<TicketHistory> TicketHistories { get; set; }

        public virtual ICollection<TicketNotification> TicketNotifications { get; set; }

        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset? Updated { get; set; }

        public int ProjectId { get; set; }

        [Display(Name = "Ticket Type")]
        public int TicketTypeId { get; set; }

        [Display(Name = "Ticket Priority")]
        public int TicketPriorityId { get; set; }

        [Display(Name = "Ticket Status")]
        public int TicketStatusId { get; set; }

        public string OwnerId { get; set; }

        [Display(Name = "Assignee")]
        public string AssigneeId { get; set; }

        public virtual ApplicationUser Assignee { get; set; }
       
        public virtual ApplicationUser Owner { get; set; }

        public virtual TicketStatus TicketStatus { get; set; }

        public virtual TicketPriority TicketPriority { get; set; }

        public virtual TicketType TicketType { get; set; }

        public virtual Project Project { get; set; }

        
    }
}