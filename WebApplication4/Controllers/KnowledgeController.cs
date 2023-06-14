using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using WebApplication5Test.Models;

namespace WebApplication5Test.Controllers
{
    public class KnowledgeController : Controller
    {
        private readonly string sqlConn = ConfigurationManager.ConnectionStrings["Project"].ToString();
        // GET: Knowledge
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create_file()
        {
            return View();
        }

        public ActionResult Test()
        {
            return View();
        }



        // GET: Knowledge
        public ActionResult KM(int id)
        {
            
            ViewBag.Id = id;
            
            var result = new List<FileModel>();
            DataTable dataTableaCreateFile = new DataTable();
            var getCreateFile = "SELECT * FROM [CreateFiles] WHERE folder_id = @folderId ";
            using (SqlConnection con = new SqlConnection(sqlConn))
            {               
                con.Open();
                using (var cmd = new SqlCommand(getCreateFile, con))
                {
                    cmd.Parameters.AddWithValue("@folderId", id);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        result.Add(TodDataFileListModel(reader));
                    }
                    con.Close();
                }
            }
            return View(result);
        }

        public FileModel TodDataFileListModel(SqlDataReader dataReader)
        {
            System.Globalization.CultureInfo _cultureInfo = new System.Globalization.CultureInfo("en-GB");

            FileModel model = new FileModel();
            model.id = Convert.ToInt32(dataReader["id"] == DBNull.Value ? null : dataReader["id"].ToString());
            model.file_name = dataReader["file_name"] == DBNull.Value ? null : dataReader["file_name"].ToString();
            model.keyword = dataReader["keyword"] == DBNull.Value ? null : dataReader["keyword"].ToString();
            model.genre = dataReader["genre"] == DBNull.Value ? null : dataReader["genre"].ToString();
            model.releaseDate = Convert.ToDateTime(dataReader["releaseDate"] == DBNull.Value ? null : dataReader["releaseDate"].ToString());
            string FormatDate = model.releaseDate.ToString("dd/MM/yyyy");
            model.dayReleaseDate = FormatDate;

            object editDateValue = dataReader["editDate"];
            if (editDateValue == DBNull.Value)
            {
                model.dayEditDate = string.Empty;
            }
            else
            {
                model.editDate = (DateTime)editDateValue;
                model.dayEditDate = model.editDate.ToString("dd/MM/yyyy");
            }

            model.uploadFile = dataReader["uploadFile"] == DBNull.Value ? null : dataReader["uploadFile"].ToString();

            return model;
        }

        
        //[HttpGet]
        //public ActionResult pass(int folderID)
        //{
        //    return RedirectToAction("Create_file", new { folderId = folderID });
        //}

        [HttpGet]
        public ActionResult Create_file(int folderId, string genre)
        {
            ViewBag.Id = folderId;
            List<string> documentTypes = GetDocumentTypesFromDatabase();
            IEnumerable<SelectListItem> dropdownDocumentTypes = documentTypes.Select(d => new SelectListItem
            {
                Value = d,
                Text = d
            });
            ViewBag.DocumentTypes = dropdownDocumentTypes;

            return View();
        }
        private List<string> GetDocumentTypesFromDatabase()
        {
            List<string> documentTypes = new List<string>();
            using (SqlConnection con = new SqlConnection(sqlConn))
            {
                string query = "SELECT folder_name FROM CreateFolders";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string document = reader.GetString(0);
                    documentTypes.Add(document);
                }
                reader.Close();
            }
            return documentTypes;
        }

        [HttpPost]
        public ActionResult Create_file(HttpPostedFileBase fileUpload, FileModel model)
        {
            int folderId = model.folder_id;
            if (fileUpload != null && fileUpload.ContentLength > 0)
            {
                byte[] fileBytes;
                using (BinaryReader br = new BinaryReader(fileUpload.InputStream))
                {
                    fileBytes = br.ReadBytes(fileUpload.ContentLength);
                }

                string fileName = Path.GetFileName(fileUpload.FileName);
                string fileExtension = Path.GetExtension(fileUpload.FileName);
                
                int view = 0;
                int download = 0;

                // ตรวจสอบนามสกุลของไฟล์และกำหนดโฟลเดอร์เก็บไฟล์ตามประเภท
                string folderPath = "";
                switch (fileExtension.ToLower())
                {
                    case ".doc":
                    case ".docx":
                        folderPath = Server.MapPath("~/Uploads/Word");
                        break;
                    case ".pdf":
                        folderPath = Server.MapPath("~/Uploads/PDF");
                        break;
                    case ".jpg":
                    case ".jpeg":
                    case ".png":
                        folderPath = Server.MapPath("~/Uploads/Images");
                        break;
                    case ".ppt":
                    case ".pptx":
                        folderPath = Server.MapPath("~/Uploads/PowerPoint");
                        break;
                    default:
                        folderPath = Server.MapPath("~/Uploads/Other");
                        break;
                }

                // สร้างโฟลเดอร์ถ้ายังไม่มี
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // สร้างเส้นทางที่เก็บไฟล์
                string filePath = Path.Combine(folderPath, fileName);

                // เซฟไฟล์ลงในเครื่องแม่ข่ายเว็บ
                System.IO.File.WriteAllBytes(filePath, fileBytes);

                // เซฟข้อมูลเกี่ยวกับไฟล์ลงในฐานข้อมูล
                using (SqlConnection con = new SqlConnection(sqlConn))
                {
                    string query = "INSERT INTO CreateFiles ( file_name, releaseDate, keyword, genre, uploadFile,view_count, download_count, folder_id) VALUES (@file_name, @releaseDate, @keyword, @genre, @uploadFile,@view, @download, @folderId)";
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con;

                        cmd.Parameters.AddWithValue("@file_name", model.file_name);
                        cmd.Parameters.AddWithValue("@releaseDate", model.releaseDate);
                        cmd.Parameters.AddWithValue("@keyword", model.keyword);
                        cmd.Parameters.AddWithValue("@genre", model.genre);
                        cmd.Parameters.AddWithValue("@uploadFile", filePath);
                        cmd.Parameters.AddWithValue("@view", model.view_count);
                        cmd.Parameters.AddWithValue("@download", model.download_count);
                        cmd.Parameters.AddWithValue("@folderId", folderId); 

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return RedirectToAction("KM", new { id = folderId });
            }

            // กรณีไม่มีไฟล์ที่อัปโหลด
            ModelState.AddModelError("fileUpload", "กรุณาเลือกไฟล์ที่ต้องการอัปโหลด");
            return View();
        }

        [HttpPost]
        public ActionResult DownloadFile(int id)
        {
            UpdateDownloadCount(id);

            // ดึงข้อมูลไฟล์จากฐานข้อมูล
            FileModel file = GetFileFromDatabase(id);

            if (file != null)
            {
                // ตรวจสอบประเภทของไฟล์และกำหนด Content-Type
                string contentType;
                switch (Path.GetExtension(file.uploadFile).ToLower())
                {
                    case ".pdf":
                        contentType = "application/pdf";
                        break;
                    case ".jpg":
                    case ".jpeg":
                        contentType = "image/jpeg";
                        break;
                    case ".png":
                        contentType = "image/png";
                        break;
                    case ".ppt":
                    case ".pptx":
                        contentType = "application/vnd.ms-powerpoint";
                        break;
                    case ".doc":
                    case ".docx":
                        contentType = "application/vnd.ms-word";
                        break;
                    default:
                        contentType = "application/octet-stream";
                        break;
                }

                // กำหนดชื่อไฟล์ที่จะดาวน์โหลด
                string fileName = file.file_name + Path.GetExtension(file.uploadFile);

                // ดาวน์โหลดไฟล์โดยใช้ File() และกำหนด Content-Type
                return File(file.uploadFile, contentType, fileName);
            }
            // หากไม่พบไฟล์ สามารถกำหนดการจัดการผิดพลาดตามที่คุณต้องการได้
            // ตัวอย่างเช่นแสดงข้อความผิดพลาดหรือเปลี่ยนเส้นทางไปยังหน้าข้อผิดพลาด
            return RedirectToAction("Error");
        }

        private void UpdateDownloadCount(int id)
        {
            string updateQuery = "UPDATE CreateFiles SET download_count = download_count + 1 WHERE id = @documentId";

            using (SqlConnection con = new SqlConnection(sqlConn))
            {
                using (SqlCommand command = new SqlCommand(updateQuery, con))
                {
                    command.Parameters.AddWithValue("@documentId", id);
                    con.Open();
                    command.ExecuteNonQuery();
                    con.Close();
                }
            }
        }


        private FileModel GetFileFromDatabase(int id)
        {
            using (SqlConnection con = new SqlConnection(sqlConn))
            {
                con.Open();

                string query = "SELECT * FROM CreateFiles WHERE id = @id";
                using (var command = new SqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // สร้าง FileModel จากข้อมูลที่อ่านได้
                            FileModel file = new FileModel
                            {
                                id = (int)reader["id"],
                                file_name = (string)reader["file_name"],
                                keyword = (string)reader["keyword"],
                                genre = (string)reader["genre"],
                                releaseDate = (DateTime)reader["releaseDate"],
                                uploadFile = (string)reader["uploadFile"]
                            };

                            return file;
                        }
                    }
                }
            }

            return null; // หากไม่พบไฟล์ในฐานข้อมูล
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var deleteSql = "DELETE FROM CreateFiles WHERE id = @id";

            using (SqlConnection connection = new SqlConnection(sqlConn))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(deleteSql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }

            return RedirectToAction("KM");
        }
        public ActionResult Edit(int id, int folderId, string genre)
        {
            FileModel model = new FileModel();
            using (SqlConnection con = new SqlConnection(sqlConn))
            {
                string query = "SELECT * FROM CreateFiles WHERE id = @id";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        model.id = Convert.ToInt32(reader["id"] == DBNull.Value ? null : reader["id"].ToString());
                        model.file_name = reader["file_name"] == DBNull.Value ? null : reader["file_name"].ToString();
                        model.keyword = reader["keyword"] == DBNull.Value ? null : reader["keyword"].ToString();
                        model.genre = reader["genre"].ToString();
                        model.releaseDate = Convert.ToDateTime(reader["releaseDate"] == DBNull.Value ? null : reader["releaseDate"].ToString());
                        model.editDate = Convert.ToDateTime(reader["editDate"] == DBNull.Value ? null : reader["editDate"].ToString());
                        model.uploadFile = reader["uploadFile"] == DBNull.Value ? null : reader["uploadFile"].ToString();
                    }

                    reader.Close();
                }
            }
            List<string> documentTypes = GetDocumentTypesFromDatabase();
            IEnumerable<SelectListItem> dropdownDocumentTypes = documentTypes.Select(d => new SelectListItem
            {
                Value = d,
                Text = d
            });
            ViewBag.DocumentTypes = dropdownDocumentTypes;
            ViewBag.Id = folderId;
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(HttpPostedFileBase fileUpload, FileModel model)
        {
            int folderId = model.folder_id;
            if (fileUpload != null && fileUpload.ContentLength > 0)
            {
                byte[] fileBytes;
                using (BinaryReader br = new BinaryReader(fileUpload.InputStream))
                {
                    fileBytes = br.ReadBytes(fileUpload.ContentLength);
                }

                string fileName = Path.GetFileName(fileUpload.FileName);
                string fileExtension = Path.GetExtension(fileUpload.FileName);

                string folderPath = "";
                switch (fileExtension.ToLower())
                {
                    case ".doc":
                    case ".docx":
                        folderPath = Server.MapPath("~/Uploads/Word");
                        break;
                    case ".pdf":
                        folderPath = Server.MapPath("~/Uploads/PDF");
                        break;
                    case ".jpg":
                    case ".jpeg":
                    case ".png":
                        folderPath = Server.MapPath("~/Uploads/Images");
                        break;
                    case ".ppt":
                    case ".pptx":
                        folderPath = Server.MapPath("~/Uploads/PowerPoint");
                        break;
                    default:
                        folderPath = Server.MapPath("~/Uploads/Other");
                        break;
                }

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath = Path.Combine(folderPath, fileName);

                System.IO.File.WriteAllBytes(filePath, fileBytes);

                using (SqlConnection con = new SqlConnection(sqlConn))
                {
                    string query = "UPDATE CreateFiles SET file_name = @file_name, releaseDate = @releaseDate, genre = @genre, keyword = @keyword, uploadFile = @uploadFile, editDate = @editDate, folder_id = @folderId WHERE id = @id";
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@id", model.id);
                        cmd.Parameters.AddWithValue("@file_name", model.file_name);
                        cmd.Parameters.AddWithValue("@releaseDate", model.releaseDate);
                        cmd.Parameters.AddWithValue("@editDate", model.editDate);
                        cmd.Parameters.AddWithValue("@keyword", model.keyword);
                        cmd.Parameters.AddWithValue("@genre", model.genre);
                        cmd.Parameters.AddWithValue("@uploadFile", filePath);
                        cmd.Parameters.AddWithValue("@folderId", folderId);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
                return RedirectToAction("KM", new { id = folderId });
                //return RedirectToAction("KM", new { id = model.id});
            }
            ModelState.AddModelError("fileUpload", "กรุณาเลือกไฟล์ที่ต้องการอัปโหลด");
            return View();
        }


        public ActionResult OpenFile(int id)
        {
            // ดึงข้อมูลไฟล์เอกสารจากฐานข้อมูลโดยใช้ id
            string filePath = GetFilePathFromDatabase(id);

            // ตรวจสอบว่ามีไฟล์ในตำแหน่งที่ระบุหรือไม่
            if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
            {
                // อ่านไฟล์และแปลงเป็น byte array
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

                // ตรวจสอบนามสกุลไฟล์
                string fileExtension = Path.GetExtension(filePath);
                string contentType = GetContentType(fileExtension);

                // อัปเดตค่าการดูเอกสาร
                UpdateViewCount(id);
                // เปิดไฟล์เอกสารในเบราว์เซอร์
                return File(fileBytes, contentType);
            }

            // กรณีไม่พบไฟล์
            return HttpNotFound();
        }
        private void UpdateViewCount(int id)
        {
            // สร้างคำสั่ง SQL สำหรับอัปเดตค่าการดูเอกสาร
            string updateQuery = "UPDATE CreateFiles SET view_count = view_count + 1 WHERE id = @documentId";

            // สร้าง SqlConnection และ SqlCommand
            using (SqlConnection con = new SqlConnection(sqlConn))
            using (SqlCommand command = new SqlCommand(updateQuery, con))
            {
                command.Parameters.AddWithValue("@documentId", id);
                con.Open();
                int rowsAffected = command.ExecuteNonQuery();
                con.Close();

                if (rowsAffected > 0)
                {
                    // อัปเดตสำเร็จ
                    Console.WriteLine("Successfully updated view count for document with ID: " + id);
                }
                else
                {
                    // ไม่พบเอกสารที่ต้องการอัปเดต
                    Console.WriteLine("Document with ID: " + id + " not found.");
                }
            }       
        }

        public ActionResult GetViewCount(int id)
        {
            // ดึงค่า view_count จากฐานข้อมูลโดยใช้ id
            int viewCount = GetViewCountFromDatabase(id);

            // ส่งค่า view_count เป็น JSON response
            return Json(viewCount, JsonRequestBehavior.AllowGet);
        }

        private int GetViewCountFromDatabase(int id)
        {
            int viewCount = 0;

            string query = "SELECT view_count FROM CreateFiles WHERE id = @id";

            using (SqlConnection con = new SqlConnection(sqlConn))
            {
                using (SqlCommand command = new SqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@id", id);
                    con.Open();
                    object result = command.ExecuteScalar();
                    con.Close();

                    if (result != null && result != DBNull.Value)
                    {
                        viewCount = Convert.ToInt32(result);
                    }
                }
            }

            return viewCount;
        }


        private string GetContentType(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case ".pdf":
                    return "application/pdf";
                case ".doc":
                case ".docx":
                    return "application/msword";
                case ".ppt":
                case ".pptx":
                    return "application/vnd.ms-powerpoint";
                case ".txt":
                    return "text/plain";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                // เพิ่มประเภทไฟล์อื่นๆ ตามต้องการ
                default:
                    return "application/octet-stream";
            }
        }
        private string GetFilePathFromDatabase(int id)
        {
            string query = "SELECT uploadFile FROM CreateFiles WHERE id = @id";

            using (SqlConnection con = new SqlConnection(sqlConn))
            {
                using (SqlCommand command = new SqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@id", id);
                    con.Open();
                    object result = command.ExecuteScalar();
                    con.Close();

                    if (result != null && result != DBNull.Value)
                    {
                        return result.ToString();
                    }
                }
            }

            return null; // หากไม่พบเอกสารที่เกี่ยวข้องกับ id ที่ระบุ
        }
       
        public ActionResult ViewDownload_count()
        {
            var result = new List<FileModel>();
            DataTable dataTableaCreateFile = new DataTable();
            var getCreateFile = "SELECT id, file_name, genre, view_count, download_count FROM [CreateFiles] ";
            using (SqlConnection con = new SqlConnection(sqlConn))
            {
                con.Open();
                using (var cmd = new SqlCommand(getCreateFile, con))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        result.Add(DataFileListModel(reader));
                    }
                    con.Close();
                }
            }
            return View(result);
        }

        public FileModel DataFileListModel(SqlDataReader dataReader)
        {
            System.Globalization.CultureInfo _cultureInfo = new System.Globalization.CultureInfo("en-GB");

            FileModel model = new FileModel();
            model.id = Convert.ToInt32(dataReader["id"] == DBNull.Value ? null : dataReader["id"].ToString());
            model.file_name = dataReader["file_name"] == DBNull.Value ? null : dataReader["file_name"].ToString();
            model.genre = dataReader["genre"] == DBNull.Value ? null : dataReader["genre"].ToString();
            model.view_count = dataReader["view_count"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["view_count"]);
            model.download_count = dataReader["download_count"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["download_count"]);
            return model;
        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //สร้าง Folders

        public ActionResult CreateFoldersKM()
        {
            var result = new List<FolderModel>();
            DataTable dataTableaCreateFolder = new DataTable();
            var getCreateFolder = "SELECT * FROM [CreateFolders] ";
            using (SqlConnection con = new SqlConnection(sqlConn))
            {
                con.Open();
                using (var cmd = new SqlCommand(getCreateFolder, con))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        result.Add(TodDataFolderListModel(reader));
                    }
                    con.Close();
                }
            }
            return View(result);
        }
        public FolderModel TodDataFolderListModel(SqlDataReader dataReader)
        {
            FolderModel model = new FolderModel();
            model.folder_id = dataReader.GetInt32(dataReader.GetOrdinal("folder_id"));
            model.folder_name = dataReader.GetString(dataReader.GetOrdinal("folder_name"));
            model.description = dataReader.GetString(dataReader.GetOrdinal("description"));
            return model;
        }

        [HttpPost]
        public ActionResult SaveFolderToDatabase(string folderName, string description)
        {
            using (SqlConnection con = new SqlConnection(sqlConn))
            {
                con.Open();
                string query = "INSERT INTO CreateFolders (folder_name, description) VALUES (@folder_name, @description)";
                using (SqlCommand command = new SqlCommand(query, con))
                {
                    command.Parameters.AddWithValue("@folder_name", folderName);
                    command.Parameters.AddWithValue("@description", description);
                    command.ExecuteNonQuery();
                }
            }
            return RedirectToAction("CreateFoldersKM");
        }

        [HttpPost]
        public ActionResult DeleteFolder(int id)
        {
            var deleteDocumentsSql = "DELETE FROM CreateFiles WHERE folder_id = @folder_id";
            var deleteSql = "DELETE FROM CreateFolders WHERE folder_id = @folder_id";

            using (SqlConnection connection = new SqlConnection(sqlConn))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(deleteDocumentsSql, connection))
                {
                    command.Parameters.AddWithValue("@folder_id", id);
                    command.ExecuteNonQuery();
                }
                using (SqlCommand command = new SqlCommand(deleteSql, connection))
                {
                    command.Parameters.AddWithValue("@folder_id", id);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }

            return RedirectToAction("CreateFoldersKM");
        }

        //////////////////////////////////////
        //Charts
        public ActionResult Charts_Dashborad()
        {
            int totalViews = 0;
            int totalDownloads = 0;
            using (SqlConnection con = new SqlConnection(sqlConn))
            {
                string query = "SELECT SUM(view_count) FROM CreateFiles";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    cmd.CommandTimeout = 60000;
                    totalViews = (int)cmd.ExecuteScalar();
                }
                query = "SELECT SUM(download_count) FROM CreateFiles";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    totalDownloads = (int)cmd.ExecuteScalar();
                }
            }

            ViewBag.TotalViews = totalViews;
            ViewBag.TotalDownloads = totalDownloads;
            return View();
            //return View();
        }

        public JsonResult HorizontalBarChart()
        {
            List<string> categories = new List<string>();
            List<int> downloadCounts = new List<int>();

            using (SqlConnection con = new SqlConnection(sqlConn))
            {
                con.Open();
                string query = "SELECT SUM(download_count) as downloadCount, genre FROM CreateFiles GROUP BY genre";
                SqlCommand command = new SqlCommand(query, con);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    categories.Add(reader["genre"].ToString());
                    downloadCounts.Add(Convert.ToInt32(reader["downloadCount"]));
                }
            }

            var chartData = new
            {
                categories = categories,
                downloadCounts = downloadCounts
            };

            return Json(chartData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DoughnutChartData()
        {
            List<int> documentCounts = new List<int>();
            List<string> genres = new List<string>();

            using (SqlConnection con = new SqlConnection(sqlConn))
            {
                string query = "SELECT COUNT(file_name) as documentCount, genre FROM CreateFiles GROUP BY genre";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        documentCounts.Add(reader.GetInt32(0));
                        genres.Add(reader.GetString(1));
                    }
                }
            }
            var doughnutChartData = new
            {
                documentCounts = documentCounts,
                genres = genres
            };
            return Json(doughnutChartData, JsonRequestBehavior.AllowGet);
        }


    }
}