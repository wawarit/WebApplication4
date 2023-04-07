using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using WebApplication4.Models;

namespace WebApplication3.Controllers
{
    public class CalendarController : Controller
    {
        private readonly string sqlConn = ConfigurationManager.ConnectionStrings["Project"].ToString();

        [HttpGet]
        // GET: Calendar
        public ActionResult Index(EventCalendarModel model)
        {
            return View();
        }
        [HttpPost]
        public JsonResult getEventday(EventCalendarModel model)
        {
            return null;
            //try
            //{
            //    DataTable dataTableaHoliday = new DataTable();
            //    DataTable dataTableEvents = new DataTable();
            //    string year = null;
            //    if (model == null)
            //    {
            //        year = DateTime.Now.Year.ToString();
            //    }
            //    else
            //    {
            //        if (model.Year == null)
            //        {
            //            year = DateTime.Now.Year.ToString();
            //        }
            //        else
            //        {
            //            year = model.Year;
            //        }
            //    }

            //    var getDataHoliday = "SELECT * FROM [Dtb_Holiday] where year(Day_Holiday) = @year";
            //    using (SqlConnection con = new SqlConnection(sqlConn))
            //    {
            //        con.Open();
            //        using (var cmd = new SqlCommand(getDataHoliday, con))
            //        {
            //            cmd.Parameters.AddWithValue("@year", year);
            //            var reader = cmd.ExecuteReader();
            //            dataTableaHoliday.Load(reader);
            //            con.Close();
            //        }
            //    }

            //    var getDataEvent = "SELECT * FROM [Dtb_Events] where year(EventDate) = @year";
            //    using (SqlConnection con = new SqlConnection(sqlConn))
            //    {
            //        con.Open();
            //        using (var cmd = new SqlCommand(getDataEvent, con))
            //        {
            //            cmd.Parameters.AddWithValue("@year", year);
            //            var reader = cmd.ExecuteReader();
            //            dataTableEvents.Load(reader);
            //            con.Close();
            //        }
            //    }

            //    var result = new DataHolidayAndEvent();
            //    result.dataTableaHoliday = dataTableaHoliday;
            //    result.dataTableEvents = dataTableEvents;

            //    var status = new ResultModel
            //    {
            //        Status = new HttpStatusCodeResult(200),
            //        Success = true,
            //        Message = null
            //    };

            //    return Json(new[] { new
            //    {
            //        Status  = status,
            //        Details = result
            //    }
            //    });

            //}
            //catch(Exception ex)
            //{
            //    var status = new ResultModel
            //    {
            //        Status = new HttpStatusCodeResult(500),
            //        Success = false,
            //        Message = ex.ToString()
            //    };

            //    return Json(new[] { new
            //    {
            //        Status  = status,
            //        Details = string.Empty
            //    }
            //    });
            //}
        }


        //public JsonResult WelcomeNote()
        //{
        //    bool isAdmin = false;
        //TODO: Check the user if it is admin or normal user, (true - Admin, false - Normal user)  
        //    string output = isAdmin ? "Welcome to the Admin User" : "Welcome to the User";

        //    return Json(output, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public ActionResult Holiday(Day data)
        //{
        //    DataTable dtable = new DataTable();

        //    var sql = @"insert into Dtb_Holiday values (@Day_Holiday, @Details)";

        //    using (SqlConnection con = new SqlConnection(sqlConn))
        //    {
        //        con.Open();


        //        using (var cmd = new SqlCommand(sql, con))
        //        {
        //            cmd.Parameters.AddWithValue("@Day_Holiday", data.Holiday);
        //            cmd.Parameters.AddWithValue("@Details", data.Holidayname);
        //            cmd.ExecuteNonQuery();
        //            con.Close();
        //        }
        //    }

        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Eventday(Day data)
        //{
        //    DataTable dtable = new DataTable();

        //    var sql = @"insert into Dtb_Holiday values (@Day_Holiday, @Details)";

        //    using (SqlConnection con = new SqlConnection(sqlConn))
        //    {
        //        con.Open();


        //        using (var cmd = new SqlCommand(sql, con))
        //        {
        //            cmd.Parameters.AddWithValue("@Day_Holiday", data.Holiday);
        //            cmd.Parameters.AddWithValue("@Details", data.Holidayname);
        //            cmd.ExecuteNonQuery();
        //            con.Close();
        //        }
        //    }

        //    return View();
        //}






    }
}