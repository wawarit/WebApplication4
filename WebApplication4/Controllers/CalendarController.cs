using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Web.Script.Serialization;
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
            try
            {
                DataTable dataTableaHoliday = new DataTable();
                DataTable dataTableEvents = new DataTable();
                string year = null;
                if (model == null)
                {
                    year = DateTime.Now.Year.ToString();
                }
                else
                {
                    if (model.Year == null)
                    {
                        year = DateTime.Now.Year.ToString();
                    }
                    else
                    {
                        year = model.Year;
                    }
                }

                var getDataHoliday = "SELECT * FROM [Dtb_Holiday] where year(Day_Holiday) = @year ";
                using (SqlConnection con = new SqlConnection(sqlConn))
                {
                    con.Open();
                    using (var cmd = new SqlCommand(getDataHoliday, con))
                    {
                        cmd.Parameters.AddWithValue("@year", year);
                        var reader = cmd.ExecuteReader();
                        dataTableaHoliday.Load(reader);
                        con.Close();
                    }
                }

                var getDataEvent = "SELECT * FROM [Dtb_Events] where year(EventDate) = @year and Status = 1 ";
                using (SqlConnection con = new SqlConnection(sqlConn))
                {
                    con.Open();
                    using (var cmd = new SqlCommand(getDataEvent, con))
                    {
                        cmd.Parameters.AddWithValue("@year", year);
                        var reader = cmd.ExecuteReader();
                        dataTableEvents.Load(reader);
                        con.Close();
                    }
                }

                var result = new DataHolidayAndEvent();
                result.dataTableaHoliday = DataTableToJSON(dataTableaHoliday);
                result.dataTableEvents = DataTableToJSON(dataTableEvents);

                var status = new ResultModel
                {
                    Status = new HttpStatusCodeResult(200),
                    Success = true,
                    Message = null
                };

                return Json(new[] { new
                {
                    Status  = status,
                    Details = result
                }
                });

            }
            catch (Exception ex)
            {
                var status = new ResultModel
                {
                    Status = new HttpStatusCodeResult(500),
                    Success = false,
                    Message = ex.ToString()
                };

                return Json(new[] { new
                {
                    Status  = status,
                    Details = string.Empty
                }
                });
            }
        }
        public static string DataTableToJSON(DataTable table)
        {
            System.Globalization.CultureInfo _cultureInfo = new System.Globalization.CultureInfo("en-Us");
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in table.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    if (col.DataType == typeof(DateTime))
                    {
                        childRow.Add(col.ColumnName, Convert.ToDateTime(row[col].ToString()).ToString("d/M/yyyy", _cultureInfo));
                    }
                    else
                    {
                        childRow.Add(col.ColumnName, row[col]);
                    }
                }
                parentRow.Add(childRow);
            }
            return jsSerializer.Serialize(parentRow);
        }

        [HttpPost]
        public ActionResult AddHoliday(DataHolidayAndEvent model)
        {
            try
            {
                var sql = @"insert into Dtb_Holiday values (@Day_Holiday, @Details)";

                using (SqlConnection con = new SqlConnection(sqlConn))
                {
                    con.Open();
                    using (var cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@Day_Holiday", model.Holiday);
                        cmd.Parameters.AddWithValue("@Details", model.Details);
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                var status = new ResultModel
                {
                    Status = new HttpStatusCodeResult(200),
                    Success = true,
                    Message = null
                };

                return Json(new[] { new
                {
                    Status  = status,
                    Details = string.Empty
                }
                });

            }
            catch (Exception ex) 
            {
                var status = new ResultModel
                {
                    Status = new HttpStatusCodeResult(200),
                    Success = true,
                    Message = ex.Message.ToString()
                };

                return Json(new[] { new
                {
                    Status  = status,
                    Details = string.Empty
                }
                });
            }            
        }

        [HttpPut]
        public ActionResult UpdateHoliday(DataHolidayAndEvent model)
        {
            try
            {
                var sql = @"UPDATE [dbo].[Dtb_Holiday]
                        SET [Day_Holiday] = @dayHoliday
                        ,[Details] = @details
                        WHERE Day_Holiday = @dayHoliday";

                using (SqlConnection con = new SqlConnection(sqlConn))
                {
                    con.Open();


                    using (var cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@Day_Holiday", model.Holiday);
                        cmd.Parameters.AddWithValue("@Details", model.Details);
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                var status = new ResultModel
                {
                    Status = new HttpStatusCodeResult(200),
                    Success = true,
                    Message = null
                };

                return Json(new[] { new
                {
                    Status  = status,
                    Details = string.Empty
                }
                });
            }
            catch (Exception ex)
            {
                var status = new ResultModel
                {
                    Status = new HttpStatusCodeResult(200),
                    Success = true,
                    Message = ex.Message.ToString()
                };

                return Json(new[] { new
                {
                    Status  = status,
                    Details = string.Empty
                }
                });
            }
        }

        [HttpDelete]
        public ActionResult DeleteHoliday(DataHolidayAndEvent model)
        {
            try
            {
                var sql = @"delete from Dtb_Holiday where @Day_Holiday = @dayHoliday";

                using (SqlConnection con = new SqlConnection(sqlConn))
                {
                    con.Open();


                    using (var cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@Day_Holiday", model.Holiday);
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                var status = new ResultModel
                {
                    Status = new HttpStatusCodeResult(200),
                    Success = true,
                    Message = null
                };

                return Json(new[] { new
                {
                    Status  = status,
                    Details = string.Empty
                }
                });
            }
            catch (Exception ex)
            {
                var status = new ResultModel
                {
                    Status = new HttpStatusCodeResult(200),
                    Success = true,
                    Message = ex.Message.ToString()
                };

                return Json(new[] { new
                {
                    Status  = status,
                    Details = string.Empty
                }
                });
            }
        }

        //[HttpPost]
        //public ActionResult EventsDay(DataHolidayAndEvent model)
        //{
        //    DataTable dtable = new DataTable();

        //    var sql = @"insert into Dtb_Holiday values (@Day_Holiday, @Details)";

        //    using (SqlConnection con = new SqlConnection(sqlConn))
        //    {
        //        con.Open();


        //        using (var cmd = new SqlCommand(sql, con))
        //        {
        //            cmd.Parameters.AddWithValue("@Day_Holiday", model.Holiday);
        //            cmd.Parameters.AddWithValue("@Details", model.Holidayname);
        //            cmd.ExecuteNonQuery();
        //            con.Close();
        //        }
        //    }

        //    return View();
        //}






    }
}