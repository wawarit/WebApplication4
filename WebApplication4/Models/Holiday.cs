using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Models
{
    public class Holiday 
    {
        // GET: Holiday
        public int Id { get; set; }
        public DateTime Day_Holiday { get; set; }
        public string Details { get; set; }
    }
}