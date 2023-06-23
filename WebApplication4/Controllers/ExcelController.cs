using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Configuration;
using WebApplication4.Models;



namespace WebApplication4.Controllers
{
    public class ExcelController : Controller
    {
        //private string sqlConn;
        
        private readonly string sqlConn = ConfigurationManager.ConnectionStrings["Project1"].ToString();
        public ActionResult UploadExcel()
        {
            return View();
        }


        [HttpPost] //Add
        public ActionResult AddFile(DataHoliday model)
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
                    Status = new HttpStatusCodeResult(500),
                    Success = false,
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
        public ActionResult UploadExcel(HttpPostedFileBase fileUpload, DataHoliday model)/*, int folderId*/
        {
            System.Globalization.CultureInfo _cultureInfo = new System.Globalization.CultureInfo("en-GB");

           
            if (fileUpload != null && fileUpload.ContentLength > 0)
            {
                string fileName = Path.GetFileName(fileUpload.FileName);
                string filePath = Path.Combine(Server.MapPath("~/App_Data"), fileName);

               
                // กรณีตั้งชื่อไฟล์ใหม่
                fileUpload.SaveAs(filePath);

                //string path = @"D:\project\วันหยุดประจำปี.xlsx";
                var getNameSheet = CheckSheetsName(filePath); // get ชื่อ sheet name excel
                var getDataExcel = ReadExcelData(filePath, getNameSheet); //อ่านข้อมูลภายใน excel

                int rowcount = 0;

                var processedRows = new List<DataHoliday>();
                //// เช็คไฟล์ที่มีอยู่
                //var existingData = GetExistingDataFromDatabase();

                // เซฟข้อมูลเกี่ยวกับไฟล์ลงในฐานข้อมูล

                using (SqlConnection con = new SqlConnection(sqlConn))
                {
                    string query = @"delete from Dtb_Holiday where day_holiday = @Holiday; insert into Dtb_Holiday values(@Holiday, @Details)";
                    //delete from Dtb_Holiday where day_holiday = @Holiday;
                    con.Open();
                    
                    foreach (DataRow row in getDataExcel.Rows)
                    {
                        //// ตรวจสอบว่ามีข้อมูลอยู่ในฐานข้อมูลหรือไม่
                        //bool dataExists = CheckIfDataExists(row, existingData);
                        //if (!dataExists)
                        if (row["Day_Holiday"].ToString() == "") continue;
                        using (var cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@Holiday", Convert.ToDateTime(row["Day_Holiday"] == DBNull.Value ? null : row["Day_Holiday"].ToString()));
                           
                            cmd.Parameters.AddWithValue("@Details", row["Details"] == DBNull.Value ? null : row["Details"].ToString());

                            cmd.ExecuteNonQuery();
                            rowcount++;
                        }
                    }

                }

                // ...

                var status = new ResultModel
                {
                    Status = new HttpStatusCodeResult(200),
                    Success = true,
                    Message = "อัปโหลดไฟล์สำเร็จ.",
                    RowCount = rowcount /*+ rowcount.ToString() + " รายการ"*/
                };

                
                return Json(new[]
                {
                    new
                    {
                        Status = status,
                        Details = string.Empty
                    }
                });

                // ...


            }
            // กรณีไม่มีไฟล์ที่อัปโหลด
            ModelState.AddModelError("fileUpload", "กรุณาเลือกไฟล์ที่ต้องการอัปโหลด");
            return View();
        }

        public DataHoliday TodDataHolidayListModel(DataRow dataRow)
        {
            System.Globalization.CultureInfo _cultureInfo = new System.Globalization.CultureInfo("en-GB");
            DataHoliday model = new DataHoliday();
            model.ID = Convert.ToInt16(dataRow["ID"].ToString());
            model.Holiday = Convert.ToDateTime(dataRow["Day_Holiday"] == DBNull.Value ? null : dataRow["Day_Holiday"].ToString());
            string formattedDate = model.Holiday.ToString("dd/MM/yyyy");
            model.Day_Holiday = formattedDate;

            model.Details = dataRow["Details"] == DBNull.Value ? null : dataRow["Details"].ToString();
            return model;
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
        public string CheckSheetsName(string FilePath)
        {
            string sheetName = "";
            try
            {
                string strImport = Path.GetExtension(FilePath);

                //System.Web.HttpContext.Current.Server.MapPath("~/" + FilePath) ถ้าต้องการอ่านไฟล์ที่อัพโหลดแล้วมาอยู่ในโฟร์เดอร์ ใน server ของเราให้มาใช้อันนี้
                FileStream stream = System.IO.File.Open(FilePath, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader = null;
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern = @"dd/MM/yyyy";
                if (strImport.ToUpper() == ".XLSX")
                {
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                else if (strImport.ToUpper() == ".XLS")
                {
                    excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                sheetName = excelReader.Name;

                excelReader.Dispose();
                excelReader.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"CheckSheetsName Function : {ex.Message}");
            }
            return sheetName;
        }
        public DataTable ReadExcelData(string strFilePathServer, string strSheetName)
        {
            //string connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + System.Web.HttpContext.Current.Server.MapPath("~/" + strFilePathServer + fileName) + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
            string connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + strFilePathServer + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
            OleDbConnection oledbConn = new OleDbConnection(connString);
            DataTable dt = new DataTable();
            try
            {
                oledbConn.Open();
                using (OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + strSheetName + "$]", oledbConn))
                {
                    OleDbDataAdapter oleda = new OleDbDataAdapter();
                    oleda.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                    oleda.Fill(ds);
                    dt = ds.Tables[0];
                    oledbConn.Close();
                    foreach (DataColumn dc in dt.Columns)
                    {
                        //if (Day_Holiday.DataType == typeof(DateTime))
                        //{
                        //    foreach (DataRow row in dt.Rows)
                        //    {
                        //        DateTime dateValue;
                        //        if (DateTime.TryParse(row[Day_Holiday].ToString(), out dateValue))
                        //        {
                        //            row[Day_Holiday] = dateValue.ToString("dd/MM/yyyy");
                        //        }
                        //    }
                        //}
                        //for loop ข้อมูล ถ้าไม่ต้องตรวจสอบอะไร ลบ foreach นี้ก่อนได้
                        //เพราะด้านล่าง return datatable ให้แล้ว
                    }
                }

            }
            catch (Exception ex)
            {
                int aa = 2;
            }
            return dt;
        }


    }

}