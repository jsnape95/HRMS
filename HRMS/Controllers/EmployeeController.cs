﻿using HRMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.IO;

namespace HRMS.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {

        HRMS.Models.ApplicationDbContext db = new Models.ApplicationDbContext();

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
            string newFileName = userName + "_" + guid + Path.GetExtension(profileImage.FileName);

            var path = Path.Combine(Server.MapPath("~/Images/Employees/"), userName);
            profileImage.SaveAs(path);

            employee.ProfileImageUrl = path;
            employee.JobId = jobId;
            db.Employees.Add(employee);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        
        
        public JsonResult GetDepartmentJobs(int departmentId)
        {
            var allJobs = db.Jobs.OrderBy(x => x.JobTitle).ToList();
            var topJobs = db.Jobs.Where(x => x.Employees.Select(y => y.DepartmentId == departmentId).FirstOrDefault()).OrderBy(x => x.JobTitle).ToList();
            var otherJobs = allJobs.Except(topJobs).ToList();
            allJobs = topJobs.Concat(otherJobs).ToList();

            var json = allJobs.Select(x => new Job { JobId = x.JobId, JobTitle = x.JobTitle });

            return Json(json, JsonRequestBehavior.AllowGet);
        }
    }
}