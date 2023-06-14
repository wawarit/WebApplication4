using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication5Test.Models
{
    public class EmployeeModel
    {
        public int id_number { get; set; }
        public string name { get; set; }
        public string position { get; set; }
        public string department { get; set; }
        public string division { get; set; }
        public string status { get; set; }
        public string image { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }

        public List<SelectListItem> ItemsPosition { get; set; }
        public List<SelectListItem> ItemsDepartment { get; set; }
        public List<SelectListItem> ItemsDivision { get; set; }
        public string ImagePath { get; set; }


    }
}