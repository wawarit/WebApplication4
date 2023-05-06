using Glimpse.Ado.Tab;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Remoting.Contexts;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebApplication4.Models;

namespace WebApplication3.Controllers
{
    public class CalendarController : Controller
    {
        private readonly string sqlConn = ConfigurationManager.ConnectionStrings["Project"].ToString();

        private object db;

        [HttpGet]
        // GET: Calendar
        public ActionResult Index(EventCalendarModel model)
        {
            return View();
        }
        public ActionResult Index(DataEvents model)
        {
            return View();
        }

        public ActionResult Test()
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

        
        public ActionResult ViewHoliday(DataEvents model)
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
            DataEvents model = new DataEvents();
            model.EventID = Convert.ToInt16(dataReader["EventID"] == DBNull.Value ? null : dataReader["EventID"].ToString());
            model.EventDate = Convert.ToDateTime(dataReader["EventDate"] == DBNull.Value ? null : dataReader["EventDate"].ToString());
            model.StartTime = Convert.ToDateTime(dataReader["StartTime"] == DBNull.Value ? null : dataReader["StartTime"].ToString());
            model.EndTime = Convert.ToDateTime(dataReader["EndTime"] == DBNull.Value ? null : dataReader["EndTime"].ToString());
            model.Subject = dataReader["Subject"] == DBNull.Value ? null : dataReader["Subject"].ToString();
            model.Detail = dataReader["Detail"] == DBNull.Value ? null : dataReader["Detail"].ToString();
            model.Creator = dataReader["Creator"] == DBNull.Value ? null : dataReader["Creator"].ToString();
            model.Status = Convert.ToBoolean(dataReader["Status"] == DBNull.Value ? null : dataReader["Status"].ToString());
            model.CreateDatetime = Convert.ToDateTime(dataReader["CreateDatetime"] == DBNull.Value ? null : dataReader["CreateDatetime"].ToString());
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
            //return View();
        }

        [HttpGet] // Set the attribute to Read
        public ActionResult ReadHoliday(DataHoliday model)

        {
            try
            {
                var sql = @"select * from Dtb_Holiday";

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

        [HttpPost] //update //อันนี้1 //เปลี่ยนจาก put เป็น post
        public ActionResult UpdateHoliday(DataHoliday model)
        {
            try
            {
                var sql = @"UPDATE [dbo].[Dtb_Holiday]
                        SET [Day_Holiday] = @dayHoliday
                        ,[Details] = @details
                        WHERE ID = @id";

                using (SqlConnection con = new SqlConnection(sqlConn))
                {
                    con.Open();

                    using (var cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@Day_Holiday", model.Holiday);
                        cmd.Parameters.AddWithValue("@Details", model.Details);
                        cmd.Parameters.AddWithValue("@ID", model.ID);
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }

                }
                //using (var context = new demoCalendarEntities())
                //{
                //    var data = model.Details.where(" @Details = @Details ",).Details();
                //    return View(data);
                //}

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
        

        [HttpPut]
        //จังหวะกดแก้ไขในหน้า view data ส่งวันไปแก้ไข เพราะ table นี้ไม่มี ID
        public ActionResult Update(DateTime Day_Holiday)
        {
            //แก้ไข
           
            var ViewHoliday = "select * from Dtb_Holiday where Day_Holiday = @dayHoliday";
            
            using (SqlConnection con = new SqlConnection(sqlConn))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(ViewHoliday, con))
                {
                    var reader = cmd.ExecuteReader();
                    con.Close();
                }
            }
            return View(ViewHoliday);
        }

        [HttpPut]
        //ลบไม่มีเพราะไม่มี id 
        public ActionResult UpdateandDelete(DateTime Day_Holiday)
        {
            //แก้ไข

            var ViewHoliday = "UPDATE[dbo].[Dtb_Holiday] SET Day_Holiday = '@Day_Holiday' , Details = '@Details' WHERE Day_Holiday = @dayHoliday";
            using (SqlConnection con = new SqlConnection(sqlConn))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(ViewHoliday, con))
                {
                    var reader = cmd.ExecuteReader();
                    con.Close();
                }
            }
            return View(ViewHoliday);
        }

        //delete update ตรงหน้า view เชื่อมกับ public 2 อันบน
        public ActionResult Delete(DateTime Day_Holiday)
        {
            //แก้ไข

            var ViewHoliday = "UPDATE [dbo].[Dtb_Holiday] SET Status = '0' WHERE Day_Holiday = @dayHoliday";
            using (SqlConnection con = new SqlConnection(sqlConn))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(ViewHoliday, con))
                {
                    var reader = cmd.ExecuteReader();
                    con.Close();
                }
            }
            return View(ViewHoliday);
        }


        [HttpPost] //อันนี้2
        public ActionResult DeleteHoliday(DataHoliday model)
        {

            try
            {
                // model.ID = 8;
                var sql = @"Delete from Dtb_Holiday where ID = @id";

                using (SqlConnection con = new SqlConnection(sqlConn))
                {
                    con.Open();


                    using (var cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@ID", model.ID);
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

        [HttpPost]
        public ActionResult AddEvents(DataEvents model)
        {
            System.Globalization.CultureInfo _cultureInfo = new System.Globalization.CultureInfo("en-GB");
            try
            {
                var sql = @"insert into Dtb_Events values (@EventID, @EventDate, @StartTime, @EndTime, @Subject, @Detail, @Creator, @Status ,@CreateDatetime)";

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
                        cmd.Parameters.AddWithValue("@Creator", model.Creator);
                        cmd.Parameters.AddWithValue("@Status", model.Status);
                        cmd.Parameters.AddWithValue("@CreateDatetime", model.CreateDatetime);
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

        [HttpPost]
        public ActionResult UpdateEvents(DataEvents model)
        {
            System.Globalization.CultureInfo _cultureInfo = new System.Globalization.CultureInfo("en-GB");
            try
            {
                var sql = @"UPDATE [dbo].[Dtb_Events]
                        SET [StartTime] = @startTime
                        ,[EndTime] = @endtime
                        WHERE EventID = @eventID";

                using (SqlConnection con = new SqlConnection(sqlConn))
                {
                    con.Open();


                    using (var cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@StartTime", model.StartTime);
                        cmd.Parameters.AddWithValue("@EndTime", model.EndTime);
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

        [HttpDelete]
        public ActionResult DeleteEvents(DataEvents model)
        {
            try
            {
                var sql = @"delete from Dtb_Events where @EventID = @eventID";

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

        //เพิ่มใหม่ อยู่ index
        //[HttpGet]
        //public JsonResult GetData()
        //{
        //    System.Globalization.CultureInfo _cultureInfo = new System.Globalization.CultureInfo("en-GB");
        //    using (SqlConnection con = new SqlConnection(sqlConn))
        //    {
        //        con.Open();
        //        using (SqlCommand cmd = new SqlCommand("SELECT * FROM Dtb_Events", con))
        //        {
        //            SqlDataReader reader = cmd.ExecuteReader();
        //            List<object> data = new List<object>();
        //            while (reader.Read())
        //            {
        //                data.Add(new
        //                {
        //                    EventID = reader["EventID"],
        //                    EventDate = reader["EventDate"],
        //                    StartTime = reader["StartTime"],
        //                    EndTime = reader["EndTime"],
        //                    Subject = reader["Subject"]
                            
        //                });
        //            }
        //            return Json(data, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //}


        //เพิ่มข้อมูลที่อยู่ใน viewcalendar จะทำให้เชื่อมกับปฎิทินอันเล็ก
        //[HttpGet]
        //public JsonResult GetData()
        //{
        //    // retrieve data from SQL database
        //    using (SqlConnection conn = new SqlConnection("<connection string>"))
        //    {
        //        conn.Open();
        //        SqlCommand cmd = new SqlCommand("SELECT * FROM Dtb_Events", conn);
        //        SqlDataReader rdr = cmd.ExecuteReader();

        //        // create a list of objects to store the data
        //        List<object> data = new List<object>();

        //        // iterate over the result set and add each row to the list
        //        while (rdr.Read())
        //        {
        //            data.Add(new
        //            {
        //                EventID = rdr["EventID"],
        //                EventDate = rdr["EventDate"],
        //                StartTime = rdr["StartTime"],
        //                EndTime = rdr["EndTime"],
        //                Subject = rdr["Subject"],

        //            });
        //        }

        //        // return the list of objects as a JSON object
        //        return Json(data, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //[HttpGet]
        //public ActionResult GetData(EventCalendarModel model)
        //{

        //    try
        //    {
        //        DataTable dataTableEvents = new DataTable();

        //        var getDataEvent = "SELECT * FROM [Dtb_Events] where year(EventDate) = @year and Status = 1 ";
        //        using (SqlConnection con = new SqlConnection(sqlConn))
        //        {
        //            con.Open();
        //            using (var cmd = new SqlCommand(getDataEvent, con))
        //            {
        //                //cmd.Parameters.AddWithValue("@year", year);
        //                var reader = cmd.ExecuteReader();
        //                dataTableEvents.Load(reader);
        //                con.Close();

        //                SqlDataReader rdr = cmd.ExecuteReader();

        //                //        // create a list of objects to store the data
        //                List<object> data = new List<object>();

        //                while (rdr.Read())
        //                {
        //                    data.Add(new
        //                    {
        //                        EventID = rdr["EventID"],
        //                        EventDate = rdr["EventDate"],
        //                        StartTime = rdr["StartTime"],
        //                        EndTime = rdr["EndTime"],
        //                        Subject = rdr["Subject"],

        //                    });
        //                }

        //                // return the list of objects as a JSON object
        //                return Json(data, JsonRequestBehavior.AllowGet);
        //            }
        //        }

        //        var result = new DataHoliday();

        //        result.dataTableEvents = DataTableToJSON(dataTableEvents);

        //        var status = new ResultModel
        //        {
        //            Status = new HttpStatusCodeResult(200),
        //            Success = true,
        //            Message = null
        //        };

        //        return Json(new[] { new
        //        {
        //            Status  = status,
        //            Details = result
        //        }
        //        });

        //    }
        //    catch (Exception ex)
        //    {
        //        var status = new ResultModel
        //        {
        //            Status = new HttpStatusCodeResult(500),
        //            Success = false,
        //            Message = ex.ToString()
        //        };

        //        return Json(new[] { new
        //        {
        //            Status  = status,
        //            Details = string.Empty
        //        }
        //        });
        //    }

        //}

    }

}
