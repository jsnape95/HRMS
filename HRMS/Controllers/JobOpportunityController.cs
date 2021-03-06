﻿using HRMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class JobOpportunityController : Controller
    {
        HRMS.Models.ApplicationDbContext db = new Models.ApplicationDbContext();

        // GET: JobOpportunity
        public ActionResult Index()
        {

            var currentVacancies = db.Vacancies.Where(x => x.OpenDate <= DateTime.Now && x.CloseDate >= DateTime.Now);
            var futureVacancies = db.Vacancies.Where(x => x.OpenDate >= DateTime.Now);
            var vacancies = currentVacancies.Concat(futureVacancies);

            return View(vacancies.ToList());
        }

        public ActionResult VacancyDetails(int vacancyId)
        {
            //make call to db to get selected vacancy
            var vacancy = db.Vacancies.FirstOrDefault(x => x.VacancyId == vacancyId);
            //make call to db to get all links
            var vacancyApplicantLinks = db.VacancyApplicantLinks.Where(x => x.VacancyId == vacancyId).ToList();

            //split the applicant links into 2 lists ie open and closed (accepted and rejected)
            var openApplications = vacancyApplicantLinks.Where(x => x.ApplicantStatus == Status.Pending).ToList();
            var closedApplications = vacancyApplicantLinks.Except(openApplications).OrderBy(x => x.ApplicantStatus == Status.Accepted).ToList();

            //add these lists to the viewbag to pass through
            ViewBag.Open = openApplications;
            ViewBag.Closed = closedApplications;

            return View(vacancy);
        }

        [HttpGet]
        public PartialViewResult AddVacancy()
        {
            //get all jobs and convert to dropdown
            var departments = new SelectList(
                    db.Departments.OrderBy(x => x.Name),
                    "DepartmentId",
                    "Name"
                );

            //send lists to view via the viewbag
            ViewBag.Departments = departments;

            return PartialView();
        }

        [HttpPost]
        public ActionResult AddVacancy(Vacancy vacancy, int jobId)
        {
            //add the jobId into the data model
            vacancy.JobId = jobId;

            //add the vacancy to the db and saves
            db.Vacancies.Add(vacancy);
            db.SaveChanges();

            //redirect to index view
            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult AddApplicant(int vacancyId)
        {
            ViewBag.vacancyId = vacancyId;
            return PartialView();
        }

        [HttpPost]
        public ActionResult AddApplicant(Applicant applicant, HttpPostedFileBase cvUpload, int vacancyId)
        {
            applicant.DateCreated = DateTime.Now;

            /*
             * Add applicant cv to the cloud
             *
             */

            db.Applicants.Add(applicant);
            db.SaveChanges();

            var vacancyApplicantLink = new VacancyApplicantLink
            {
                ApplicantId = applicant.ApplicantId,
                VacancyId = vacancyId,
                ApplicantStatus = Status.Pending
            };

            db.VacancyApplicantLinks.Add(vacancyApplicantLink);
            db.SaveChanges();

            return RedirectToAction("VacancyDetails", new { vacancyId = vacancyId });
        }

        [HttpGet]
        public PartialViewResult AcceptApplicant(int vacancyId, int applicantId)
        {

            var lineManagers = new SelectList(
                db.Employees.Where(x => x.IsManager).OrderBy(x => x.FirstName).Select(x => new
                {
                    ManagerId = x.EmployeeId,
                    Name = x.FirstName + " " + x.LastName
                }
            ).ToList(), "ManagerId", "Name");

            //send lists to view via the viewbag
            ViewBag.LineManagers = lineManagers;
            ViewBag.VacancyId = vacancyId;
            ViewBag.ApplicantId = applicantId;

            return PartialView();
        }

        [HttpPost]
        public ActionResult AcceptApplicant(Employee employee, int vacancyId, int applicantId)
        {
            var applicantLink = db.VacancyApplicantLinks.FirstOrDefault(x => x.ApplicantId == applicantId && x.VacancyId == vacancyId);
            applicantLink.ApplicantStatus = Status.Accepted;

            var applicant = db.Applicants.FirstOrDefault(x => x.ApplicantId == applicantId);

            var newEmployee = new Employee
            {
                DateOfBirth = applicant.DateOfBirth,
                Title = applicant.Title,
                FirstName = applicant.FirstName,
                LastName = applicant.LastName,
                EmailAddress = applicant.Email,
                IsManager = employee.IsManager,
                JobId = applicantLink.Vacancy.JobId,
                LineManagerId = employee.LineManagerId,
                ProfileImageUrl = "",
                Salary = employee.Salary,
                StartDate = employee.StartDate,
                //WORK OUT HOLIDAY ENTITLEMENT - JS
                HolidayEntitlement = 28



            };

            db.Employees.Add(newEmployee);

            db.SaveChanges();

            return RedirectToAction("VacancyDetails", new { vacancyId = vacancyId} );
        }

        [HttpPost]
        public ActionResult RejectApplicant(int vacancyId, int applicantId)
        {
            var applicantLink = db.VacancyApplicantLinks.FirstOrDefault(x => x.ApplicantId == applicantId && x.VacancyId == vacancyId);
            applicantLink.ApplicantStatus = Status.Rejected;

            db.SaveChanges();

            return RedirectToAction("");
        }
        

        public ActionResult ViewApplicants()
        {
            return View();
        }

    }
}