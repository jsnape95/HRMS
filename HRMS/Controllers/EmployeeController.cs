using HRMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;

namespace HRMS.Controllers
{
    [Authorize (Roles = "Admin")]
    public class EmployeeController : Controller
    {

        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Employee
        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult EmployeeResults(string searchString)
        {
            //pull list of employees from database
            IQueryable<Employee> employees = db.Employees;

            //refine employees by search string
            employees = employees.Where(x => x.FirstName.Contains(searchString) || x.LastName.Contains(searchString));

            return PartialView(employees.ToList());
        }

        public ActionResult EmployeeDetails(int id)
        {
            var employee = db.Employees.FirstOrDefault(x => x.EmployeeId == id);
            return View(employee);
        }

        [HttpGet]
        public PartialViewResult AddEmployee()
        {
            //get all employees who are managers and convert to dropdown list
            var lineManagers = new SelectList(
                    db.Employees.Where(x => x.IsManager).OrderBy(x => x.FirstName).Select(x => new
                        {
                            ManagerId = x.EmployeeId,
                            Name = x.FirstName + " " + x.LastName
                        }
                    ).ToList(),"ManagerId","Name");
            

            //get all departments and convert to dropdown
            var departments = new SelectList(
                    db.Departments.OrderBy(x => x.Name),
                    "DepartmentId",
                    "Name"
                );

            //get all jobs and convert to dropdown
            var jobs = new SelectList(
                    db.Jobs.OrderBy(x => x.JobTitle),
                    "JobId",
                    "JobTitle"
                );

            //send lists to view via the viewbag
            ViewBag.LineManagers = lineManagers;
            ViewBag.Departments = departments;
            ViewBag.Jobs = jobs;

            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddEmployee(Employee employee, int jobId, HttpPostedFileBase profileImage)
        {
            var originalFilename = Path.GetFileName(profileImage.FileName);
            string guid = Guid.NewGuid().ToString().Replace("-", "");
            string userName = employee.FirstName.ToLower() + "_" + employee.LastName.ToLower();
            string newFileName = userName + "_" + guid;
            string newFileNameExt = userName + "_" + guid + Path.GetExtension(profileImage.FileName);

            var x = Server.MapPath(profileImage.FileName);
             
            //save locally
            var path = Path.Combine(Server.MapPath("~/Images/Employees/"), newFileNameExt);
            profileImage.SaveAs(path);

            //save to cloud
            var cloud = new Helpers.CloudStroage();
            var url = cloud.UploadImage(path, newFileName);

            employee.ProfileImageUrl = url;
            employee.JobId = jobId;
            db.Employees.Add(employee);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult AddDocument(int employeeId)
        {
            

            var link = new EmployeeEmployeeDocumentLink();
            link.EmployeeId = employeeId;

            return PartialView(link);
        }

        [HttpPost]
        public ActionResult AddDocument(EmployeeEmployeeDocumentLink link, int documentList, HttpPostedFileBase documentUpload)
        {
            var employee = db.Employees.FirstOrDefault(y => y.EmployeeId == link.EmployeeId);
            var document = db.EmployeeDocuments.FirstOrDefault(y => y.EmployeeDocumentId == documentList);

            var originalFilename = Path.GetFileName(documentUpload.FileName);
            string guid = Guid.NewGuid().ToString().Replace("-", "");
            string userName = employee.FirstName.ToLower() + "_" + employee.LastName.ToLower() + "_" + Regex.Replace(document.Name, @"\s+", "").ToLower();
            string newFileName = userName + "_" + guid;
            string newFileNameExt = userName + "_" + guid + Path.GetExtension(documentUpload.FileName);

            var x = Server.MapPath(documentUpload.FileName);

            //save locally
            var path = Path.Combine(Server.MapPath("~/Files/Employees/"), newFileNameExt);
            documentUpload.SaveAs(path);

            //save to cloud
            var cloud = new Helpers.CloudStroage();
            var url = "";

            if (documentUpload.ContentType.Contains("image"))
            {
                url = cloud.UploadImage(path, newFileName);
            }
            else
            {
                url = cloud.UploadFile(path, newFileName);
            } 

            
            link.EmployeeDocument = null;
            link.EmployeeDocumentId = documentList;
            link.DateUploaded = DateTime.Now;
            link.URL = url;

            db.EmployeeEmployeeDocumentLinks.Add(link);
            db.SaveChanges();

            return RedirectToAction("");
        }


        public JsonResult GetDepartmentJobs(int departmentId)
        {
            //database call to pull all the jobs for the chose department
            var allJobs = db.Jobs.Where(x => x.DepartmentId == departmentId).OrderBy(x => x.JobTitle).ToList();

            //converts to an object that can be used with json
            var jsonJobs = allJobs.Select(x => new Job { JobId = x.JobId, JobTitle = x.JobTitle });

            return Json(jsonJobs, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCategoryDocuments(int categoryType)
        {
            //database call to pull all the jobs for the chose department
            var docs = db.EmployeeDocuments.Where(x => x.DocumentCategory == (DocumentCategory)categoryType).OrderBy(x => x.Name).ToList();

            //converts to an object that can be used with json
            var jsonDocs = docs.Select(x => new EmployeeDocument { EmployeeDocumentId = x.EmployeeDocumentId, Name = x.Name});

            return Json(jsonDocs, JsonRequestBehavior.AllowGet);
        }


    }
}