using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Models
{
    public class DataEvents
    {
        // GET: DataEvents
        //public string dataTableEvents { get; set; }

        public string EventID { get; set; }
        public string EventDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Subject { get; set; }
        public string Detail { get; set; }
        public string Creator { get; set; }
        public string Status { get; set; }
        public string CreateDatetime { get; set; }
    }
}