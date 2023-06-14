using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class DataHoliday
    {
        public string dataTableaHoliday { get; set; }
        public string dataTableEvents { get; set; }
        public int ID { get; set; }
        [DisplayFormat(DataFormatString = "{0:d}")]

        public DateTime Holiday { get; set; }
        public String Day_Holiday { get; set; }
        public string Details { get; set; }

        public string Sheetname { get; set; }

        //public string Rows { get; set; }
        //public string codeEventsday { get; set; }
        //public string Eventsday { get; set; }
        //public string StartTime { get; set; }
        //public string EndTime { get; set; }
        //public string Subject { get; set; }
        //public string Eventsdetail { get; set; }
        //public string Creator { get; set; }
        //public string Status { get; set; }
        //public string Creatdate { get; set; }
    }
}