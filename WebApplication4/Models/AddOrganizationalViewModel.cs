using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication5Test.Models
{
    public class AddOrganizationalViewModel
    {
        public List<EmployeeModel> Employees { get; set; }
        public IEnumerable<SelectListItem> Positions { get; set; }
        public IEnumerable<SelectListItem> Divisions { get; set; }
    }
}