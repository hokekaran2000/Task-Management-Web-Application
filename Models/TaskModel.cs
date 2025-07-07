using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskManagementApp.Models
{
    public class TaskModel
    {
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UserId { get; set; }
        public DateTime? Deadline { get; set; }
        public string Priority { get; set; }

    }
}