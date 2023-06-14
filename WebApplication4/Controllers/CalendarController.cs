using Glimpse.Ado.Tab;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Runtime.Remoting.Contexts;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebApplication4.Models;

namespace WebApplication3.Controllers
{
    public class CalendarController : Controller
    {
        private readonly string sqlConn = ConfigurationManager.ConnectionStrings["Project1"].ToString();

        //private object db;

        [HttpGet]
        // GET: Calendar
        public ActionResult Index(EventCalendarModel model)
        {
            System.Globalization.CultureInfo _cultureInfo = new System.Globalization.CultureInfo("en-GB");

            var result = new List<DataEvents>();
            DataTable dataTableaEvents = new DataTable();
            var getDataEvents = "SELECT * FROM [Dtb_Events] ";
            using (SqlConnection con = new SqlConnection(sqlConn))
            {
                con.Open();
                using (var cmd = new SqlCommand(getDataEvents, con))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        result.Add(TodDataEventsListModel(reader));

                    }
                    con.Close();
                }
            }
            return View(result);
        }


        public ActionResult Index(DataEvents model)
        {
            return View();
        }

        public ActionResult AddHoliday()
        {
            return View();
        }
        public ActionResult ReadHoliday()
        {
            return View();
        }
        public ActionResult UpdateHoliday()
        {
            return View();
        }
        public ActionResult DeleteHoliday()
        {
            return View();
        }
        public ActionResult AddEvents()
        {
            return View();
        }
        public ActionResult UpdateEvents()
        {
            return View();
        }
        public ActionResult DeleteEvents()
        {
            return View();
        }

        public ActionResult Events1()
        {
            return View();
        }
        public ActionResult ViewCalendar(EventCalendarModel model)
        {
            return View();
        }
        public ActionResult Update()
        {
            return View();
        }
        public ActionResult UpdateEventsdate()
        {
            return View();
        }
        public ActionResult EditEvents()
        {
            return View();
        }
        public ActionResult AddEventstest()
        {
            return View();
        }


        

        [HttpGet]
        // GET: Calendar
        public ActionResult ViewallHoliday(EventCalendarModel model)
        {
            System.Globalization.CultureInfo _cultureInfo = new System.Globalization.CultureInfo("en-GB");
            var result = new List<DataHoliday>();
            DataTable dataTableaEvents = new DataTable();
            var getDataHoliday = "SELECT * FROM [Dtb_Holiday] ";
            using (SqlConnection con = new SqlConnection(sqlConn))
            {
                con.Open();
                using (var cmd = new SqlCommand(getDataHoliday, con))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        result.Add(TodDataHolidayListModel(reader));
                    }
                    con.Close();
                }
            }
            return View(result);
        }

        
        public ActionResult ViewHoliday(DataHoliday model)
        {
            System.Globalization.CultureInfo _cultureInfo = new System.Globalization.CultureInfo("en-GB");
            var result = new List<DataHoliday>();
            DataTable dataTableaHoliday = new DataTable();
            var getDataHoliday = "SELECT ID, Day_Holiday, Details FROM [Dtb_Holiday] ";
            using (SqlConnection con = new SqlConnection(sqlConn))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(getDataHoliday, con))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read()) 
                    {
                        result.Add(TodDataHolidayListModel(reader));
                    }
                    con.Close();
                }
            }
            return View(result);
        }

        public DataHoliday TodDataHolidayListModel(SqlDataReader dataReader) 
        {
            System.Globalization.CultureInfo _cultureInfo = new System.Globalization.CultureInfo("en-GB");
            DataHoliday model = new DataHoliday();
            model.ID = Convert.ToInt16(dataReader["ID"] .ToString() );
            model.Holiday = Convert.ToDateTime(dataReader["Day_Holiday"] == DBNull.Value ? null : dataReader["Day_Holiday"].ToString());
            string formattedDate = model.Holiday.ToString("dd/MM/yyyy");
            model.Day_Holiday = formattedDate;

            model.Details = dataReader["Details"] == DBNull.Value ? null : dataReader["Details"].ToString();
            return model;
        }

        public ActionResult ViewEvents(DataEvents model)
        {
            System.Globalization.CultureInfo _cultureInfo = new System.Globalization.CultureInfo("en-GB");
            var result = new List<DataEvents>();
            DataTable dataTableaEvents = new DataTable();
            var getDataEvents = "SELECT * FROM [Dtb_Events] ";
            using (SqlConnection con = new SqlConnection(sqlConn))
            {
                con.Open();
                using (var cmd = new SqlCommand(getDataEvents, con))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        result.Add(TodDataEventsListModel(reader));

                    }
                    con.Close();
                }
            }
            return View(result);
        }

        public DataEvents TodDataEventsListModel(SqlDataReader dataReader)
        {
            System.Globalization.CultureInfo _cultureInfo = new System.Globalization.CultureInfo("en-GB");
            DataEvents model = new DataEvents();
            model.EventID = Convert.ToInt32(dataReader["EventID"] == DBNull.Value ? null : dataReader["EventID"].ToString());
            model.EventDate = Convert.ToDateTime(dataReader["EventDate"] == DBNull.Value ? null : dataReader["EventDate"].ToString());
            string formattedDate = model.EventDate.ToString("dd/MM/yyyy");
            model.Event_Date = formattedDate;

            model.StartTime = Convert.ToDateTime(dataReader["StartTime"] == DBNull.Value ? null : dataReader["StartTime"].ToString());
            string formattedTime = model.StartTime.ToString("HH:mm:ss");
            model.Start_Time = formattedTime;

            model.EndTime = Convert.ToDateTime(dataReader["EndTime"] == DBNull.Value ? null : dataReader["EndTime"].ToString());
            string formattedtoTimes = model.EndTime.ToString("HH:mm:ss");
            model.End_Time = formattedtoTimes;

            model.Subject = dataReader["Subject"] == DBNull.Value ? null : dataReader["Subject"].ToString();
            model.Detail = dataReader["Detail"] == DBNull.Value ? null : dataReader["Detail"].ToString();
            model.Creator = dataReader["Creator"] == DBNull.Value ? null : dataReader["Creator"].ToString();
            model.Status = Convert.ToBoolean(dataReader["Status"] == DBNull.Value ? null : dataReader["Status"].ToString());
            model.CreateDatetime = Convert.ToDateTime(dataReader["CreateDatetime"] == DBNull.Value ? null : dataReader["CreateDatetime"].ToString());
            model.Department = dataReader["Department"] == DBNull.Value ? null : dataReader["Department"].ToString();
            return model;
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

                var result = new DataHoliday();
                result.dataTableaHoliday = DataTableToJSON(dataTableaHoliday);
                result.dataTableEvents = DataTableToJSON(dataTableEvents);
                Console.WriteLine($"Info: {result}");

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
            System.Globalization.CultureInfo _cultureInfo = new System.Globalization.CultureInfo("en-GB");
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

        [HttpPost] //Add
        public ActionResult AddHoliday(DataHoliday model)
        {

            System.Globalization.CultureInfo _cultureInfo = new System.Globalization.CultureInfo("en-GB");
            try
            {
                var sql = @"insert into Dtb_Holiday  values (@Holiday, @Details)";

                using (SqlConnection con = new SqlConnection(sqlConn))
                {
                    con.Open();
                    using (var cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@Holiday", model.Holiday);
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
            //return View();
        }

        //เพิ่มวันนี้ 19/95/66
        [HttpGet]
        public ActionResult EditHoliday(int ID)
        {
            DataHoliday model = new DataHoliday();
            using (SqlConnection con = new SqlConnection(sqlConn))
            {
                string query = "SELECT * FROM Dtb_Holiday WHERE ID = @id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ID", ID);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        model.ID = Convert.ToInt16(reader["ID"].ToString());
                        model.Holiday = Convert.ToDateTime(reader["Day_Holiday"] == DBNull.Value ? null : reader["Day_Holiday"].ToString());
                        model.Details = reader["Details"] == DBNull.Value ? null : reader["Details"].ToString();
                        
                    }

                    reader.Close();
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult EditHoliday(DataHoliday model)
        {

            using (SqlConnection con = new SqlConnection(sqlConn))
            {
                var sql = @"UPDATE [dbo].[Dtb_Holiday]
                        SET [Day_Holiday] = @day_Holiday
                        ,[Details] = @details
                        WHERE ID = @id";

                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = con;

                    //cmd.Parameters.AddWithValue("@CreateDatetime", model.CreateDatetime);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                return RedirectToAction("ViewHoliday");
            }
        }

        [HttpPost]
        public ActionResult DeleteHoliday(int ID)
        {
            var deleteSql = @"Delete FROM Dtb_Holiday WHERE ID = @id";

            using (SqlConnection connection = new SqlConnection(sqlConn))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(deleteSql, connection))
                {
                    command.Parameters.AddWithValue("@id", ID);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }

            return RedirectToAction("ViewHoliday");
        }


        //[HttpPost] //อันนี้2
        //public ActionResult DeleteHoliday(DataHoliday model)
        //{

        //    try
        //    {
        //        // model.ID = 8;
        //        var sql = @"Delete  FROM Dtb_Holiday WHERE ID = @id";

        //        using (SqlConnection con = new SqlConnection(sqlConn))
        //        {
        //            con.Open();


        //            using (var cmd = new SqlCommand(sql, con))
        //            {
        //                cmd.Parameters.AddWithValue("@ID", model.ID);
        //                cmd.ExecuteNonQuery();
        //                con.Close();
        //            }
        //        }
        //        var status = new ResultModel
        //        {
        //            Status = new HttpStatusCodeResult(200),
        //            Success = true,
        //            Message = null
        //        };

        //        return Json(new[] { new
        //        {
        //            Status  = status,
        //            Details = string.Empty
        //        }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        var status = new ResultModel
        //        {
        //            Status = new HttpStatusCodeResult(200),
        //            Success = true,
        //            Message = ex.Message.ToString()
        //        };

        //        return Json(new[] { new
        //        {
        //            Status  = status,
        //            Details = string.Empty
        //        }
        //        });
        //    }

        //}

        [HttpPost]
        public ActionResult AddEvents(DataEvents model)
        {
            System.Globalization.CultureInfo _cultureInfo = new System.Globalization.CultureInfo("en-GB");
            try
            {
                //DateTime sqlMinDateAsNetDateTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;

                var sql = @"insert into Dtb_Events (EventDate, StartTime, EndTime, Subject, Detail, Creator, Status ,CreateDatetime, Department) values (@EventDate, @StartTime, @EndTime, @Subject, @Detail, @Creator, 1 ,getdate(), @Department)";

                using (SqlConnection con = new SqlConnection(sqlConn))
                {
                    con.Open();
                    using (var cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@EventID", model.EventID);
                        cmd.Parameters.AddWithValue("@EventDate", model.EventDate);
                        cmd.Parameters.AddWithValue("@StartTime", model.StartTime);
                        cmd.Parameters.AddWithValue("@EndTime", model.EndTime);
                        cmd.Parameters.AddWithValue("@Subject", model.Subject);
                        cmd.Parameters.AddWithValue("@Detail", model.Detail);
                        cmd.Parameters.AddWithValue("@Creator", 6230200635);
                        cmd.Parameters.AddWithValue("@Status", model.Status);
                        cmd.Parameters.AddWithValue("@Department", model.Department);
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
            //return View();
        }

        [HttpGet]
        public ActionResult EditEvents(int EventID)
        {
            DataEvents model = new DataEvents();
            using (SqlConnection con = new SqlConnection(sqlConn))
            {
                string query = "SELECT * FROM Dtb_Events WHERE EventID = @eventID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@EventID", EventID);
                    
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        model.EventID = Convert.ToInt32(reader["EventID"] == DBNull.Value ? null : reader["EventID"].ToString());
                        model.EventDate = Convert.ToDateTime(reader["EventDate"] == DBNull.Value ? null : reader["EventDate"].ToString());
                        model.StartTime = Convert.ToDateTime(reader["StartTime"] == DBNull.Value ? null : reader["StartTime"].ToString());
                        model.EndTime = Convert.ToDateTime(reader["EndTime"] == DBNull.Value ? null : reader["EndTime"].ToString());
                        //model.Start_Time = reader["Start_Time"] == DBNull.Value ? null : reader["Start_Time"].ToString();
                        //model.End_Time = reader["End_Time"] == DBNull.Value ? null : reader["End_Time"].ToString();
                        model.Subject = reader["Subject"] == DBNull.Value ? null : reader["Subject"].ToString();
                        model.Detail = reader["Detail"] == DBNull.Value ? null : reader["Detail"].ToString();
                        model.Creator = reader["Creator"] == DBNull.Value ? null : reader["Creator"].ToString();
                        model.Status = Convert.ToBoolean(reader["Status"] == DBNull.Value ? null : reader["Status"].ToString());
                        //model.CreateDatetime = Convert.ToDateTime(reader["CreateDatetime"] == DBNull.Value ? null : reader["CreateDatetime"].ToString());
                        model.Department = reader["Department"] == DBNull.Value ? null : reader["Department"].ToString();
                    }

                    reader.Close();
                }

            }
            return View(model);
        }

        [HttpPost]
        public ActionResult EditEvents(DataEvents model)
        {
            using (SqlConnection con = new SqlConnection(sqlConn))
            {
                var sql = @"UPDATE [dbo].[Dtb_Events]
                        SET [EventDate] = @eventDate
                        ,[StartTime] = @startTime
                        ,[EndTime] = @endTime
                        ,[Subject] = @subject
                        ,[Detail] = @detail
                        ,[Department] = @department
                        WHERE EventID = @eventID";

                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@EventID", 1);
                    cmd.Parameters.AddWithValue("@EventDate", model.Event_Date);
                    cmd.Parameters.AddWithValue("@StartTime", model.Start_Time);
                    cmd.Parameters.AddWithValue("@EndTime", model.End_Time);
                    cmd.Parameters.AddWithValue("@Subject", model.Subject);
                    cmd.Parameters.AddWithValue("@Detail", model.Detail);
                    cmd.Parameters.AddWithValue("@Creator", model.Creator);
                    cmd.Parameters.AddWithValue("@Status", model.Status);
                    cmd.Parameters.AddWithValue("@Department", model.Department);
                    //cmd.Parameters.AddWithValue("@CreateDatetime", model.CreateDatetime);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
                return RedirectToAction("ViewEvents");
            }
        }

        [HttpDelete]
        public ActionResult DeleteEvents(DataEvents model)
        {
            try
            {
                var sql = @"Update  Dtb_Events set status=0  where EventID = @EventID";
                using (SqlConnection con = new SqlConnection(sqlConn))
                {
                    con.Open();


                    using (var cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@EventID", model.EventID);
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
            //return View();
        }


    }

}
