using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Models
{
    public class DataEvents
    {
        public int EventID { get; set; }
        public DateTime EventDate { get; set; }
        public string Event_Date { get; set; }
        public DateTime StartTime { get; set; }
        public String Start_Time { get; set; }
        public DateTime EndTime { get; set; }
        public String End_Time { get; set; }
        public string Subject { get; set; }
        public string Detail { get; set; }
        public string Creator { get; set; }
        public Boolean Status { get; set; }
        public DateTime CreateDatetime { get; set; }
        public string Department { get; set; }
    }
}