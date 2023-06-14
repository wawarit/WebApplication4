using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication5Test.Models;


namespace WebApplication5Test.Controllers
{
    public class OrganizationalController : Controller
    {
        private readonly string sqlConn = ConfigurationManager.ConnectionStrings["Project"].ToString();

        // GET: Organizational
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult organizationChart()
        {
            var result = new List<EmployeeModel>();
            DataTable dataTableaEmployee = new DataTable();
            var getCreateFolder = "SELECT * FROM [EmployeeInformation2] ";
            using (SqlConnection con = new SqlConnection(sqlConn))
            {
                con.Open();
                using (var cmd = new SqlCommand(getCreateFolder, con))
                {
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        result.Add(TodDataEmployeestModel(reader));
                    }
                    con.Close();
                }
            }
            return View(result);
        }

        private EmployeeModel TodDataEmployeestModel(SqlDataReader reader)
        {
            EmployeeModel model = new EmployeeModel();
            model.id_number = reader.GetInt32(reader.GetOrdinal("id_number"));
            model.name = reader.IsDBNull(reader.GetOrdinal("name")) ? "-" : reader.GetString(reader.GetOrdinal("name"));
            model.position = reader.IsDBNull(reader.GetOrdinal("position")) ? "-" : reader.GetString(reader.GetOrdinal("position"));
            model.department = reader.IsDBNull(reader.GetOrdinal("department")) ? "-" : reader.GetString(reader.GetOrdinal("department"));
            model.division = reader.IsDBNull(reader.GetOrdinal("division")) ? "-" : reader.GetString(reader.GetOrdinal("division"));
            model.status = reader.IsDBNull(reader.GetOrdinal("status")) ? "-" : reader.GetString(reader.GetOrdinal("status"));
            model.image = reader.IsDBNull(reader.GetOrdinal("image")) ? "-" : reader.GetString(reader.GetOrdinal("image"));
            return model;
        }

        public ActionResult AddOrganizational()
        {
            List<EmployeeModel> employees = GetEmployeesFromDatabase();

            List<string> positions = GetDataPositionFromSQL();
            IEnumerable<SelectListItem> dropdownPositions = positions.Select(p => new SelectListItem
            {
                Value = p,
                Text = p
            });

            List<string> divisions = GetDataDivisionFromSQL();
            IEnumerable<SelectListItem> dropdownDivisions = divisions.Select(p => new SelectListItem
            {
                Value = p,
                Text = p
            }).ToList();

            var viewModel = new AddOrganizationalViewModel
            {
                Employees = employees,
                Positions = dropdownPositions,
                Divisions = dropdownDivisions
            };
            return View("AddOrganizational", viewModel);
        }

        private List<EmployeeModel> GetEmployeesFromDatabase()
        {

            List<EmployeeModel> employees = new List<EmployeeModel>();

            using (SqlConnection con = new SqlConnection(sqlConn))
            {
                string query = "SELECT * FROM EmployeeInformation2"; 
                SqlCommand command = new SqlCommand(query, con);

                con.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    EmployeeModel employee = new EmployeeModel();
                    employee.id_number = reader.GetInt32(reader.GetOrdinal("id_number"));
                    employee.name = reader.IsDBNull(reader.GetOrdinal("name")) ? "-" : reader.GetString(reader.GetOrdinal("name"));
                    employee.position = reader.IsDBNull(reader.GetOrdinal("position")) ? "-" : reader.GetString(reader.GetOrdinal("position"));
                    employee.department = reader.IsDBNull(reader.GetOrdinal("department")) ? "-" : reader.GetString(reader.GetOrdinal("department"));
                    employee.division = reader.IsDBNull(reader.GetOrdinal("division")) ? "-" : reader.GetString(reader.GetOrdinal("division"));
                    employee.status = reader.IsDBNull(reader.GetOrdinal("status")) ? "-" : reader.GetString(reader.GetOrdinal("status"));
                    employee.image = reader.IsDBNull(reader.GetOrdinal("image")) ? "-" : reader.GetString(reader.GetOrdinal("image"));

                    employees.Add(employee);
                }

                reader.Close();
            }

            return employees;
        }

        //เพิ่มข้อมูลบุคลากรใหม่
        [HttpGet]
        public ActionResult AddEmployee()
        {
            EmployeeModel model = new EmployeeModel();
            List<string> positions = GetDataPositionFromSQL();
            model.ItemsPosition = positions.Select(p => new SelectListItem
            {
                Value = p,
                Text = p
            }).ToList();

            List<string> departments = GetDataDepartmentFromSQL();
            model.ItemsDepartment = departments.Select(p => new SelectListItem
            {
                Value = p,
                Text = p
            }).ToList();

            List<string> divisions = GetDataDivisionFromSQL();
            model.ItemsDivision = divisions.Select(p => new SelectListItem
            {
                Value = p,
                Text = p
            }).ToList();

            return View(model);
        }
        private List<string> GetDataPositionFromSQL()
        {
            List<string> positions = new List<string>();
            using (SqlConnection con = new SqlConnection(sqlConn))
            {                
                string query = "SELECT * FROM AddPosition";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();                
                while (reader.Read())
                {
                    string position = reader.GetString(0);
                    positions.Add(position);
                }
                reader.Close();
            }
            return positions;
        }
        private List<string> GetDataDepartmentFromSQL()
        {
            List<string> departments = new List<string>();
            using (SqlConnection con = new SqlConnection(sqlConn))
            {
                string query = "SELECT * FROM AddDepartment";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string department = reader.GetString(0);
                    departments.Add(department);
                }
                reader.Close();
            }
            return departments;
        }
        private List<string> GetDataDivisionFromSQL()
        {
            List<string> divisions = new List<string>();
            using (SqlConnection con = new SqlConnection(sqlConn))
            {
                string query = "SELECT * FROM AddDivision";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string division = reader.GetString(0);
                    divisions.Add(division);
                }
                reader.Close();
            }
            return divisions;
        }

        [HttpPost]
        public ActionResult AddEmployee(EmployeeModel model, HttpPostedFileBase ImageFile)
        {
            if (ImageFile != null && ImageFile.ContentLength > 0)
            {
                string fileExtension = Path.GetExtension(ImageFile.FileName);
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };

                if (allowedExtensions.Contains(fileExtension.ToLower()))
                {
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string folderPath = Server.MapPath("~/Uploads/EmployeeImages");
                    string filePath = Path.Combine(folderPath, fileName);
                    string storageFile = @"Uploads/EmployeeImages/" + fileName;


                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    ImageFile.SaveAs(filePath);

                    using (SqlConnection con = new SqlConnection(sqlConn))
                    {
                        string query = "INSERT INTO EmployeeInformation2 (name, position, department, division, status, image) VALUES (@name, @position, @department, @division, @status, @image)";
                        using (SqlCommand cmd = new SqlCommand(query))
                        {
                            cmd.Connection = con;
                            cmd.Parameters.AddWithValue("@name", model.name);
                            cmd.Parameters.AddWithValue("@position", model.position);
                            cmd.Parameters.AddWithValue("@department", model.department);
                            cmd.Parameters.AddWithValue("@division", model.division);
                            cmd.Parameters.AddWithValue("@status", model.status);
                            cmd.Parameters.AddWithValue("@image", storageFile);

                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                    model.ImagePath = filePath;
                    return RedirectToAction("AddOrganizational", "Organizational");
                }
                else
                {
                    ModelState.AddModelError("ImageFile", "กรุณาเลือกไฟล์ภาพที่ถูกต้อง (สกุล .jpg, .jpeg, .png)");
                }
            }

            // กรณีไม่มีไฟล์ที่อัปโหลด
            ModelState.AddModelError("ImageFile", "กรุณาเลือกรูปภาพที่ต้องการอัปโหลด");
            return View();
        }


        //ลบข้อมูลออก
        [HttpPost]
        public ActionResult DeleteEmployee(int id)
        {
            var deleteSql = "DELETE FROM EmployeeInformation2 WHERE id_number = @id_number";

            using (SqlConnection connection = new SqlConnection(sqlConn))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(deleteSql, connection))
                {
                    command.Parameters.AddWithValue("@id_number", id);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }

            return RedirectToAction("AddOrganizational");
        }

        //แก้ไขข้อมูลพนักงาน
        public ActionResult EditEmployee(int id)
        {
            EmployeeModel model = new EmployeeModel();
            using (SqlConnection con = new SqlConnection(sqlConn))
            {
                string query = "SELECT * FROM EmployeeInformation2 WHERE id_number = @id_number";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id_number", id);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        model.id_number = id; 
                        model.name = reader.IsDBNull(reader.GetOrdinal("name")) ? "-" : reader.GetString(reader.GetOrdinal("name"));
                        model.position = reader.IsDBNull(reader.GetOrdinal("position")) ? "-" : reader.GetString(reader.GetOrdinal("position"));
                        model.department = reader.IsDBNull(reader.GetOrdinal("department")) ? "-" : reader.GetString(reader.GetOrdinal("department"));
                        model.division = reader.IsDBNull(reader.GetOrdinal("division")) ? "-" : reader.GetString(reader.GetOrdinal("division"));
                        model.status = reader.IsDBNull(reader.GetOrdinal("status")) ? "-" : reader.GetString(reader.GetOrdinal("status"));
                        model.image = reader.IsDBNull(reader.GetOrdinal("image")) ? "-" : reader.GetString(reader.GetOrdinal("image"));
                    }

                    reader.Close();
                }
            }
            List<string> positions = GetDataPositionFromSQL();
            model.ItemsPosition = positions.Select(p => new SelectListItem
            {
                Value = p,
                Text = p
            }).ToList();
            List<string> departments = GetDataDepartmentFromSQL();
            model.ItemsDepartment = departments.Select(p => new SelectListItem
            {
                Value = p,
                Text = p
            }).ToList();

            List<string> divisions = GetDataDivisionFromSQL();
            model.ItemsDivision = divisions.Select(p => new SelectListItem
            {
                Value = p,
                Text = p
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult EditEmployee(EmployeeModel model, HttpPostedFileBase ImageFile)
        {
            if (ImageFile != null && ImageFile.ContentLength > 0)
            {
                // ตรวจสอบประเภทไฟล์ภาพ
                string fileExtension = Path.GetExtension(ImageFile.FileName);
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };

                if (allowedExtensions.Contains(fileExtension.ToLower()))
                {
                    // กำหนดชื่อไฟล์และเส้นทางในเครื่องแม่ข่ายเว็บ
                    string fileName = Path.GetFileName(ImageFile.FileName);
                    string folderPath = Server.MapPath("~/Uploads/Images");
                    string filePath = Path.Combine(folderPath, fileName);

                    // บันทึกไฟล์ลงในเครื่องแม่ข่ายเว็บ
                    ImageFile.SaveAs(filePath);

                    using (SqlConnection con = new SqlConnection(sqlConn))
                    {
                        string query = "UPDATE EmployeeInformation2 SET name = @name, position = @position, department = @department, division = @division, status = @status, image = @image WHERE id_number = @id_number";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Connection = con;
                            cmd.Parameters.AddWithValue("@id_number", model.id_number);
                            cmd.Parameters.AddWithValue("@name", model.name);
                            cmd.Parameters.AddWithValue("@position", model.position);
                            cmd.Parameters.AddWithValue("@department", string.IsNullOrEmpty(model.department) ? DBNull.Value : (object)model.department);
                            cmd.Parameters.AddWithValue("@division", string.IsNullOrEmpty(model.division) ? DBNull.Value : (object)model.division);
                            cmd.Parameters.AddWithValue("@status", model.status);
                            cmd.Parameters.AddWithValue("@image", filePath);

                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                        }

                    }
                    return RedirectToAction("AddOrganizational");
                }
                else
                {
                    ModelState.AddModelError("ImageFile", "กรุณาเลือกไฟล์ภาพที่ถูกต้อง (สกุล .jpg, .jpeg, .png)");
                }
            }
            ModelState.AddModelError("ImageFile", "กรุณาเลือกรูปภาพที่ต้องการอัปโหลด");
            return View();
        }


       



    }             
}