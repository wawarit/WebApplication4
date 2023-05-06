using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Models
{
    public class File
    {
        public int id { get; set; }
        public string file_name { get; set; }
        public string genre { get; set; }
        public DateTime? releaseDate { get; set; }
        public string uploadFile { get; set; }

    }
}