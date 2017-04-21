using HRMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HRMS.Controllers
{
    [Authorize]
    public class HolidayController : Controller
    {
        
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Holiday
        public ActionResult Index()
        {
            var name = User.Identity.Name;
            var user = db.Users.FirstOrDefault(x => x.UserName == name);

            if (user.EmployeeId != null)
            {
                int approvedDays = user.Employee.EmployeeHolidayLinks
                .Where(x => x.Approved == ApprovedStatus.Approved)
                .Sum(x => x.Holiday.EndDate.Subtract(x.Holiday.StartDate).Days + 1);

                int pendingDays = user.Employee.EmployeeHolidayLinks
                    .Where(x => x.Approved == ApprovedStatus.Pending)
                    .Sum(x => x.Holiday.EndDate.Subtract(x.Holiday.StartDate).Days + 1);

                int currentRemaining = user.Employee.HolidayEntitlement - approvedDays;
                int provisionalRemaining = currentRemaining - pendingDays;

                ViewBag.CurrentRemaining = currentRemaining;
                ViewBag.ProvisionalRemaining = provisionalRemaining;
            }

            return View(user);
        }

        public ActionResult ViewCalendar()
        {
            return View();
        }

        public JsonResult GetUsersHolidays(int? employeeId)
        {
            var allHolidays = new List<EmployeeHolidayLink>();

            if (employeeId == null)
            {
                //get current user
                var user = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);

                //get list of holidays for user
                allHolidays = db.EmployeeHolidayLinks.Where(x =>
                        x.EmployeeId == user.EmployeeId &&
                        x.Approved != ApprovedStatus.Rejected
                    ).ToList();
            }
            else
            {
                allHolidays = db.EmployeeHolidayLinks.Where(x =>
                       x.EmployeeId == employeeId &&
                       x.Approved != ApprovedStatus.Rejected
                   ).ToList();
            }
            

            //turn list into JSON
            var jsonHols = allHolidays.Select(x => new HolidayJsonViewModel {
                    id = x.EmployeeHolidayLinkId,
                    title = x.Approved == ApprovedStatus.Approved ? "Approved" : "Pending",
                    start = x.Holiday.StartDate.ToString("yyyy-MM-dd"),
                    end = x.Holiday.EndDate.ToString("yyyy-MM-dd"),
                    color = x.Approved == ApprovedStatus.Approved ? "#00ff00" : "#ffcc00",
                    textColor = "#000000"
            });
            
            return Json(jsonHols, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult RequestHoliday(DateTime start, DateTime end, string provisionalDays)
        {

            var holidayLinks = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name).Employee.EmployeeHolidayLinks.ToList();
            var sameDay = holidayLinks.Where(x => 
                    (x.Holiday.StartDate.Date <= start && x.Holiday.EndDate.Date >= start) || 
                    (x.Holiday.StartDate.Date >= start && x.Holiday.EndDate.Date <= end) ||
                    (x.Holiday.StartDate.Date <= end && x.Holiday.EndDate.Date >= end)
                ).Count();

            if (Convert.ToInt32(provisionalDays) == 0)
            {
                return Json(new { status = 0 }, JsonRequestBehavior.AllowGet); //ran out of holidays
            }
            else if ((end.Subtract(start).Days + 1) > Convert.ToInt32(provisionalDays))
            {
                return Json(new { status = 1 }, JsonRequestBehavior.AllowGet); //requested amount is above provisional amount
            }
            else if (sameDay > 0)
            {
                return Json(new { status = 2 }, JsonRequestBehavior.AllowGet); //can not double book holidays
            }


            if (start != end)
            {
                start = start.AddHours(9);
                end = end.AddHours(17);
            }

            var holidayRequest = new Holiday
            {
                StartDate = start,
                EndDate = end 
            };

            return PartialView(holidayRequest);
        }

        [HttpPost]
        public ActionResult RequestHoliday(Holiday holiday, int? time)
        {
            //get user info
            var user = db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);

            switch (time)
            {
                case 1:
                    holiday.StartDate = holiday.StartDate.AddHours(9);
                    holiday.EndDate = holiday.EndDate.AddHours(17);
                    break;
                case 2:
                    holiday.StartDate = holiday.StartDate.AddHours(9);
                    holiday.EndDate = holiday.EndDate.AddHours(13);
                    break;
                case 3:
                    holiday.StartDate = holiday.StartDate.AddHours(13);
                    holiday.EndDate = holiday.EndDate.AddHours(17);
                    break;
                case null:
                    break;
            }


            //see if holiday already exsists in db
            var holidayExists = db.Holidays.FirstOrDefault(x => x.StartDate == holiday.StartDate && x.EndDate == holiday.EndDate);

            int newHolId;
            if (holidayExists == null)
            {
                db.Holidays.Add(holiday);
                db.SaveChanges();
                newHolId = holiday.HolidayId;
            }
            else
            {
                newHolId = holidayExists.HolidayId;
            }

            var holidayLink = new EmployeeHolidayLink
            {
                EmployeeId = (int)user.EmployeeId,
                HolidayId = newHolId,
                Approved = ApprovedStatus.Pending
            };

            db.EmployeeHolidayLinks.Add(holidayLink);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult HolidayInformation(int linkId)
        {

            var link = db.EmployeeHolidayLinks.FirstOrDefault(x => x.EmployeeHolidayLinkId == linkId);

            return PartialView(link);
        }

        [HttpPost]
        public ActionResult CancelRequest(EmployeeHolidayLink link)
        {
            db.EmployeeHolidayLinks.Attach(link);
            db.EmployeeHolidayLinks.Remove(link);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public PartialViewResult HolidayDecision(int linkId)
        {
            var link = db.EmployeeHolidayLinks.FirstOrDefault(x => x.EmployeeHolidayLinkId == linkId);
            return PartialView(link);
        }

        [HttpPost]
        public ActionResult HolidayDecision(EmployeeHolidayLink link)
        {

            var dbLink = db.EmployeeHolidayLinks.FirstOrDefault(x => x.EmployeeHolidayLinkId == link.EmployeeHolidayLinkId);

            if (Request.Form["accept"] != null)
            {
                dbLink.Approved = ApprovedStatus.Approved;
            }
            else if (Request.Form["reject"] != null)
            {
                dbLink.Approved = ApprovedStatus.Rejected;
            }
            else if(Request.Form["cancel"] != null)
            {
                CancelRequest(dbLink);
            }

            db.SaveChanges();

            return RedirectToAction("EmployeeDetails", "Employee", new { id = link.EmployeeId });
        }
    }
}